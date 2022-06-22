using System.Linq;
using System;
using Xunit;
using System.Collections.Generic;

namespace KanbanizeCardTest
{
    public class CardTest
    {
        // ==========================================================================================
        #region Properties and Fields
        readonly Client Client;

        public static IEnumerable<object[]> Tasks =>
        new List<object[]>
        {
            new object[] { new { title = "CardDemo3", position = "1" }, 0 },
            new object[] { new { title = "CardDemo4", position = "3", color = "#34a97b", priority = "Low" }, 1 },
            new object[] { new { title = "CardDemo2", position = "2", deadline = "2023-01-13", color = "#0015bb", priority = "High" }, 2 },
            new object[] { new { title = "CardDemo1", position = "0" }, 3 },
        };
        #endregion


        // ==========================================================================================
        public CardTest()
        {
            // Gets the api key and subdomain values from the environment variables in the system
            var apiKey = Environment.GetEnvironmentVariable("KANBANIZE_API_KEY");
            var subdomain = Environment.GetEnvironmentVariable("KANBANIZE_SUBDOMAIN");

            // Base url which each request will starts with
            var baseUrl = $"https://{subdomain}.kanbanize.com/index.php/api/kanbanize/";

            // The client who will call the requests
            Client = new Client(apiKey, baseUrl);
        }


        // ==========================================================================================
        #region Tests
        [Theory]
        [MemberData(nameof(Tasks))]
        public void TaskTest(object task, object iteration)
        {
            // Creates the task
            var createResponse = Client.CreateTask(task);


            // Get's the task by id
            var getResponse = Client.GetTask(createResponse.Data.Id);


            // Checks if the current created task and the task that was uploaded to the api
            // have same properties and values
            int i = Convert.ToInt32(iteration);
            var task1 = Tasks.ToArray()[i][0]; // Task from the code
            var task2 = getResponse.Data;      // Task from the API

            var task1Properties = task1.GetType().GetProperties();
            var task2Properties = task2.GetType().GetProperties();

            foreach (var prop1 in task1Properties)
            {
                var prop2 = task2Properties.FirstOrDefault(p => p.Name == prop1.Name.ToLower());

                if (prop2 != null)
                {
                    var expected = prop1.GetValue(task1);
                    var actual = prop2.GetValue(task2);

                    // deadline format from the API is different from the one we have given (don't know why?)
                    // so we need to get the other property deadlineoriginalformat with the correct format
                    if (prop2.Name == "deadline")
                    {
                        var newProp = task2Properties.FirstOrDefault(p => p.Name == "deadlineoriginalformat");
                        actual = newProp.GetValue(getResponse.Data);
                    }

                    Assert.Equal(expected, actual);
                }
            }

            // TODO: Update Position


            // Deletes the created task
            var deleteResponse = Client.DeleteTask(createResponse.Data.Id);
            Assert.True(deleteResponse.Data);

            // Checks if the task still exist (this checking is costing us few seconds!)
            deleteResponse = Client.DeleteTask(createResponse.Data.Id);
            Assert.False(deleteResponse.Data);
        }
        #endregion
    }
}
