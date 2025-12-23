using Propeller.Integration.Tests.Support;
using Propeller.Models.Requests;
using Propeller.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;
using TechTalk.SpecFlow;

namespace Propeller.Integration.Tests.Drivers
{
    public class ContactDriver : DriverBase
    {
        private readonly HttpClient _httpClient;
        private string contactsRoute = "contacts";

        public ContactDriver(ScenarioContext scenarioContext, FeatureContext featureContext)
            : base(scenarioContext, featureContext)
        {
            _httpClient = new HttpClient();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="newCustomerRequest"></param>
        /// <returns></returns>
        public async Task<(ApiResponse apiResponse, ContactDto? customer)>
               AddNewContact(CreateContactRequest newContactRequest)
        {

            try
            {
                var baseUrl = _featureContext.Get<string>(ContextKeys.ApiBaseUrl);
                Uri uri = new($"{baseUrl}/{contactsRoute}");

                string jsonString = JsonSerializer.Serialize(newContactRequest);
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
                    var contactCreated = JsonSerializer.Deserialize<ContactDto>(responseContent);

                    // Put to avoid warnings
                    if (contactCreated == null)
                    {
                        throw new Exception("Invalid Contact");
                    }

                    // Hookup cleanup
                    _scenarioContext.AddCleanUpStep(async () =>
                    {
                        await DeleteContact(contactCreated.Id, true);
                    });

                    return (apiResponse, contactCreated);
                }

                return (apiResponse, null);

            }
            catch (Exception ex)
            {

                throw;
            }


        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contactId"></param>
        /// <param name="forceDelete"></param>
        /// <returns></returns>
        public async Task<HttpStatusCode> DeleteContact(int contactId, bool forceDelete)
        {

            try
            {
                var baseUrl = _featureContext.Get<string>(ContextKeys.ApiBaseUrl);
                Uri uri = new($"{baseUrl}/{contactsRoute}/{contactId}?fd={(forceDelete ? "y" : "n")}");

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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchCriteria"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        internal async Task<(HttpStatusCode statusCode, List<ContactDto>? contacts)>
            RetrieveContactsBySearch(string searchCriteria, string searchField, int pageNumber, int pageSize)
        {

            if (searchField.Equals("FirstName", StringComparison.InvariantCultureIgnoreCase))
            {
                searchField = "f";
            }
            else if (searchField.Equals("LastName", StringComparison.InvariantCultureIgnoreCase))
            {
                searchField = "l";
            }
            else if (searchField.Equals("Email", StringComparison.InvariantCultureIgnoreCase))
            {
                searchField = "e";
            }
            else if (searchField.Equals("Phone", StringComparison.InvariantCultureIgnoreCase))
            {
                searchField = "p";
            }
            else
            {
                searchField = string.Empty;
            }

            var baseUrl = _featureContext.Get<string>(ContextKeys.ApiBaseUrl);

            StringBuilder sb = new StringBuilder($"{baseUrl}/{contactsRoute}?q={searchCriteria}");

            if (pageNumber > 0)
            {
                sb.Append($"&pn={pageNumber}");
            }

            if (pageSize > 0)
            {
                sb.Append($"&ps={pageSize}");
            }

            if (!string.IsNullOrEmpty(searchField))
            {
                sb.Append($"&sf={searchField}");
            }

            Uri uri = new(sb.ToString());

            string token = RetrieveCurrentUserToken();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var httpResponse = await _httpClient.GetAsync(uri);
            var statusCode = httpResponse.StatusCode;

            if (statusCode == HttpStatusCode.OK)
            {
                var responseContent = await httpResponse.Content.ReadAsStringAsync();
                var contacts = JsonSerializer.Deserialize<List<ContactDto>>(responseContent);
                return (statusCode, contacts);
            }

            return (statusCode, null);
        }


    }
}
