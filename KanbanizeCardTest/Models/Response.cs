using System.Net;
namespace KanbanizeCardTest.Models
{
    public class Response<T>
    {
        public HttpStatusCode StatusCode { get; set; }

        public T Data { get; set; }
    }
}