using Propeller.Models.Requests;
using System.Net;
using System.Text;
using System.Text.Json;
using TechTalk.SpecFlow;

namespace Propeller.Integration.Tests.Drivers
{
    public class AuthenticationDriver : DriverBase
    {
        private readonly HttpClient _httpClient;

        private string authRoute = "auth";

        public AuthenticationDriver(ScenarioContext scenarioContext, FeatureContext featureContext)
            : base(scenarioContext, featureContext)
        {
            _httpClient = new HttpClient();
        }

        public async Task<(HttpStatusCode statusCode, string token)> Authenticate(
            AuthRequest request)
        {

            try
            {
                var baseUrl = _featureContext.Get<string>(ContextKeys.ApiBaseUrl);
                Uri uri = new($"{baseUrl}/{authRoute}/authenticate");

                string jsonString = JsonSerializer.Serialize(request);

                var httpResponse = await _httpClient.PostAsync(uri, new StringContent(jsonString, Encoding.UTF8, "application/json"));
                var statusCode = httpResponse.StatusCode;

                if (statusCode == HttpStatusCode.OK)
                {
                    var responseContent = await httpResponse.Content.ReadAsStringAsync();
                    return (statusCode, responseContent);
                }

                return (statusCode, null);

            }
            catch (Exception ex)
            {
                Console.Out.WriteLine($"Exception when Authenticating: {ex.Message}");
                throw;
            }

        }

    }
}
