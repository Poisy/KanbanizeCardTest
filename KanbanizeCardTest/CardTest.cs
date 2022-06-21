using System;
using Xunit;
using System.Net;
using System.IO;

namespace KanbanizeCardTest
{
    public class CardTest
    {
        readonly Client Client;

        public CardTest()
        {
            var apiKey = Environment.GetEnvironmentVariable("KANBANIZE_API_KEY");
            var subdomain = Environment.GetEnvironmentVariable("KANBANIZE_SUBDOMAIN");

            var baseUrl = $"https://{subdomain}.kanbanize.com/index.php/api/kanbanize/";

            Client = new Client(apiKey, baseUrl);
        }

        [Fact]
        public void CreatingTest()
        {
            dynamic projects = Client.Get("get_projects_and_boards");

            string result = (string)projects.projects[0].boards[0].name;

            Assert.Equal("CardTest", result);
        }


    }
}
