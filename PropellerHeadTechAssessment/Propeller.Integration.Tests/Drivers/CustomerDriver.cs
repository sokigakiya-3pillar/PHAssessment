using System.Net;
using System.Text;
using Propeller.Models.Requests;
using System.Text.Json;
using Propeller.Models;
using System.Net.Http.Headers;
using Propeller.Integration.Tests.Support;
using TechTalk.SpecFlow;

namespace Propeller.Integration.Tests.Drivers
{
    public class CustomerDriver : DriverBase
    {
        private readonly HttpClient _httpClient;

        private readonly string customersRoute = "customers";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scenarioContext"></param>
        /// <param name="featureContext"></param>
        public CustomerDriver(ScenarioContext scenarioContext, FeatureContext featureContext)
            : base(scenarioContext, featureContext)
        {
            _httpClient = new HttpClient();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverAddress"></param>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public async Task<(HttpStatusCode statusCode, CustomerDto customer)>
            RetrieveCustomer(string customerId)
        {
            var baseUrl = _featureContext.Get<string>(ContextKeys.ApiBaseUrl);
            Uri uri = new($"{baseUrl}/{customersRoute}/{customerId}");

            string token = RetrieveCurrentUserToken();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var httpResponse = await _httpClient.GetAsync(uri);
            var statusCode = httpResponse.StatusCode;

            if (statusCode == HttpStatusCode.OK)
            {
                var responseContent = await httpResponse.Content.ReadAsStringAsync();
                var customer = JsonSerializer.Deserialize<CustomerDto>(responseContent);
                return (statusCode, customer);
            }

            return (statusCode, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="serverAddress"></param>
        /// <param name="searchCriteria"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        public async Task<(HttpStatusCode statusCode, List<CustomerDto> customers)> RetrieveCustomersBySearch(
            string searchCriteria, int pageNumber, int pageSize)
        {

            try
            {
                var baseUrl = _featureContext.Get<string>(ContextKeys.ApiBaseUrl);
                Uri uri = new($"{baseUrl}/{customersRoute}?q={searchCriteria}&pn={pageNumber}&ps={pageSize}");

                string token = RetrieveCurrentUserToken();
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var httpResponse = await _httpClient.GetAsync(uri);
                var statusCode = httpResponse.StatusCode;

                if (statusCode == HttpStatusCode.OK)
                {
                    var responseContent = await httpResponse.Content.ReadAsStringAsync();
                    var customers = JsonSerializer.Deserialize<List<CustomerDto>>(responseContent);
                    return (statusCode, customers);
                }

                return (statusCode, null);
            }
            catch (Exception ex)
            {

                throw;
            }


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="statusId"></param>
        /// <returns></returns>
        public async Task<HttpStatusCode> ChangeCustomerStatus(string customerId, string statusId)
        {
            try
            {
                var baseUrl = _featureContext.Get<string>(ContextKeys.ApiBaseUrl);
                Uri uri = new($"{baseUrl}/{customersRoute}/{customerId}/status/{statusId}");

                string token = RetrieveCurrentUserToken();
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                StringContent data = new StringContent(string.Empty, Encoding.UTF8, "application/json");
                var httpResponse = await _httpClient.PutAsync(uri, data);
                var statusC = httpResponse.StatusCode;
                return statusC;
            }
            catch (Exception)
            {

                throw;
            }
        }

        public async Task<HttpStatusCode> DeleteCustomer(string customerId, bool forceDelete)
        {

            try
            {
                var baseUrl = _featureContext.Get<string>(ContextKeys.ApiBaseUrl);

                Uri uri = new($"{baseUrl}/{customersRoute}/{customerId}?fd={(forceDelete ? "y" : "n")}");

                string token = RetrieveCurrentUserToken();
                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var httpResponse = await _httpClient.DeleteAsync(uri);
                return httpResponse.StatusCode;
            }
            catch (Exception ex)
            {

                throw;
            }


        }

        public async Task<(ApiResponse apiResponse, CustomerDto? customer)>
            AddNewCustomer(CreateCustomerRequest newCustomerRequest)
        {

            try
            {
                var baseUrl = _featureContext.Get<string>(ContextKeys.ApiBaseUrl);
                Uri uri = new($"{baseUrl}/{customersRoute}");

                string jsonString = JsonSerializer.Serialize(newCustomerRequest);
                string token = RetrieveCurrentUserToken();

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var httpResponse = await _httpClient.PostAsync(uri, new StringContent(jsonString, Encoding.UTF8, "application/json"));
                var statusCode = httpResponse.StatusCode;
                var responseContent = await httpResponse.Content.ReadAsStringAsync();

                var apiResponse = new ApiResponse
                {
                    StatusCode = statusCode,
                    ResponseBody = responseContent
                };

                if (statusCode == HttpStatusCode.Created)
                {

                    var customerCreated = JsonSerializer.Deserialize<CustomerDto>(responseContent);

                    // Hookup cleanup
                    _scenarioContext.AddCleanUpStep(async () =>
                    {
                        await DeleteCustomer(customerCreated.ID, true);
                    });

                    return (apiResponse, customerCreated);
                }

                return (apiResponse, null);

            }
            catch (Exception ex)
            {
                throw;
            }

        }
    }
}
