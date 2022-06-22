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
            new object[] { new { title = "CardDemo3", position = "1", description = "2" }, 0 },
            new object[] { new { title = "CardDemo4", position = "3", description = "3", color = "#34a97b", priority = "Low" }, 1 },
            new object[] { new { title = "CardDemo2", position = "2", description = "1", deadline = "2023-01-13", color = "#0015bb", priority = "High" }, 2 },
            new object[] { new { title = "CardDemo1", position = "0", description = "0" }, 3 },
        };

        public static List<int> Ids { get; set; } = new List<int>();
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
        public void A_Create_Get_Compare_Task(object task, object iteration)
        {
            // Creates the task
            var createResponse = Client.CreateTask(task);
            Console.WriteLine($"Created Task [{createResponse.Data.Id}]!");
            Ids.Add(createResponse.Data.Id);
            

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
                    Console.WriteLine($"Task [{createResponse.Data.Id}] is correct!");
                }
            }
        }

        
        [Fact]
        public void B_Move_Task()
        {
            Console.WriteLine($"Tasks are unordered!");
            int task2Id = Ids[2];

            bool result = Client.MoveTask(task2Id, 1).Data;

            Assert.True(result);
            Console.WriteLine($"Task [{task2Id}] moved to possition 1!");

            var allTasks = Client.GetAllTask().Data;

            foreach (var id in Ids)
            {
                var task = allTasks.FirstOrDefault(t => Convert.ToInt32(t.Taskid) == id);

                if (task != null)
                {
                    int position = Convert.ToInt32(task.Description);
                    Assert.Equal(Convert.ToInt32(task.Position), position);
                }
            }
            Console.WriteLine($"Tasks are now ordered!");
        }


        [Fact]
        public void C_Delete_Task()
        {
            foreach (var id in Ids)
            {
                // Deletes the created task
                var deleteResponse = Client.DeleteTask(id);
                Assert.True(deleteResponse.Data);

                // Checks if the task still exist (this)
                deleteResponse = Client.DeleteTask(id);
                Assert.False(deleteResponse.Data); 

                Console.WriteLine($"Task [{id}] is deleted!"); 
            }
        }
        #endregion
    }
}
