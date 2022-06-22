using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using Newtonsoft.Json;
using System.Linq;
using System.Text;
using KanbanizeCardTest.Models;

namespace KanbanizeCardTest
{
    public class Client
    {
        // ==========================================================================================
        #region Properties and Fields
        readonly string API_KEY;
        readonly string BASE_URL;
        const string METHOD = "POST";
        const string END_URL = "/format/json";

        int WorkspaceId { get; set; }
        int BoardId { get; set; }
        #endregion


        // ==========================================================================================
        public Client(string apiKey, string baseUrl)
        {
            API_KEY = apiKey;
            BASE_URL = baseUrl;

            Init();
        }


        // ==========================================================================================
        #region Methods
        private void Init()
        {
            // There need to be a workspace called "WorkspaceDemo" and board in it called "BoardDemo"

            var response = SendRequest<GetProjectsAndBoardsResponse>("get_projects_and_boards");

            var project = response.Data.Projects.FirstOrDefault(p => p.Name == "WorkspaceDemo");

            if (project != null)
            {
                var board = project.Boards.FirstOrDefault(b => b.Name == "BoardDemo");

                if (board != null)
                {
                    WorkspaceId = project.Id;
                    BoardId = board.Id;

                    return;
                }
            }
            
            throw new InvalidDataException("'WorkspaceDemo' workspace and/or 'BoardDemo' board do not exist!");
        }

        // ==========================================================================================
        public Response<T> SendRequest<T>(string url, string method = METHOD, string body = "", 
            Dictionary<string, string> headers = null)
        {
            // The response that is gonna be returned
            Response<T> result;

            // Creating basic http request
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(BASE_URL+url+END_URL);
            request.Method = method;
            request.Headers.Add("apikey", API_KEY);
            request.ContentType = "application/x-www-form-urlencoded";

            // Creating the body of the request
            ASCIIEncoding encoding = new ASCIIEncoding();
            byte[] bytes = encoding.GetBytes(body);
            request.ContentLength = bytes.Length;
            Stream newStream = request.GetRequestStream();
            newStream.Write(bytes, 0, bytes.Length);

            // Adding custom headers if theres any
            if (headers != null)
            {
                foreach (var header in headers)
                {
                    request.Headers.Add(header.Key, header.Value);
                }
            }

            request.AutomaticDecompression = DecompressionMethods.GZip;


            // Sends the request and gets the response
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                // Setting the status code of the response
                result = new Response<T> { StatusCode = response.StatusCode };

                if (response.StatusCode == HttpStatusCode.OK)
                {
                    using (Stream stream = response.GetResponseStream())
                    {
                        using (StreamReader reader = new StreamReader(stream))
                        {
                            // Converting the response body to our desired T object
                            string html = reader.ReadToEnd();
                            result.Data = JsonConvert.DeserializeObject<T>(html);
                        }
                    }
                }
            }

            return result;
        }


        // ==========================================================================================
        public Response<TaskDetails> GetTask(int taskId)
        {
            string function = "get_task_details";
            string body = $"{{\"boardid\":{BoardId},\"taskid\":{taskId}}}";

            return SendRequest<TaskDetails>(function, body: body);
        }


        // ==========================================================================================
        public Response<TaskDetails[]> GetAllTask()
        {
            string function = "get_all_tasks";
            string body = $"{{\"boardid\":{BoardId}}}";

            return SendRequest<TaskDetails[]>(function, body: body);
        }


        // ==========================================================================================
        public Response<CreateTaskResponse> CreateTask(object task)
        {
            string function = "create_new_task";
            string body = JsonConvert.SerializeObject(task);

            // Inserting the BoardId because only this class have access to it
            body = body.Insert(1, $"\"boardid\":{BoardId},");

            return SendRequest<CreateTaskResponse>(function, body: body);
        }


        // ==========================================================================================
        public Response<bool> DeleteTask(int taskId)
        {
            string function = "delete_task";
            string body = $"{{\"boardid\":{BoardId},\"taskid\":{taskId}}}";

            Response<bool> result = new Response<bool>();

            try
            {
                // If the task was deleted, the response will be 'true' otherwise will be an error
                result = SendRequest<bool>(function, body: body);
            }
            catch(WebException) {}

            return result;
        }


        // ==========================================================================================
        public Response<bool> MoveTask(int taskId, int position)
        {
            string function = "move_task";
            string body = $"{{\"boardid\":{BoardId},\"taskid\":{taskId},\"position\":{position}}}";

            Response<bool> result = new Response<bool>();

            try
            {
                // If the task was exist, the response will be 'true' otherwise will be an error
                result = SendRequest<bool>(function, body: body);
            }
            catch(WebException) {}

            return result;
        }
        #endregion
    }
}