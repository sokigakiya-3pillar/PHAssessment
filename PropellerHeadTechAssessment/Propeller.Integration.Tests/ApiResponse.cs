using System.Net;

namespace Propeller.Integration.Tests
{
    public class ApiResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public string ResponseBody { get; set; } = string.Empty;
    }
}
