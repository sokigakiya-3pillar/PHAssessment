using System.Net.Http.Headers;
using System.Net;
using Propeller.Models;
using System.Text.Json;
using Propeller.Integration.Tests.Support;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace Propeller.Integration.Tests.Drivers
{
    public class NotesDriver: DriverBase
    {
        private readonly HttpClient _httpClient;

        private readonly string notesRoute = "notes";

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scenarioContext"></param>
        /// <param name="featureContext"></param>
        public NotesDriver(ScenarioContext scenarioContext, FeatureContext featureContext)
            : base(scenarioContext, featureContext)
        {
            _httpClient = new HttpClient();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="noteId"></param>
        /// <returns></returns>
        public async Task<HttpStatusCode> DeleteNote(string customerId, int noteId)
        {
            var baseUrl = _featureContext.Get<string>(ContextKeys.ApiBaseUrl);
            Uri uri = new($"{baseUrl}/{notesRoute}/{customerId}/{noteId}");

            string token = RetrieveCurrentUserToken();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var httpResponse = await _httpClient.DeleteAsync(uri);
            return httpResponse.StatusCode;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public async Task<(HttpStatusCode statusCode, IEnumerable<NoteDto> notes)> RetrieveCustomerNotes(
            string customerId)
        {
            var baseUrl = _featureContext.Get<string>(ContextKeys.ApiBaseUrl);
            Uri uri = new($"{baseUrl}/{notesRoute}/{customerId}");

            string token = RetrieveCurrentUserToken();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var httpResponse = await _httpClient.GetAsync(uri);
            var responseContent = await httpResponse.Content.ReadAsStringAsync();

            var notes = JsonSerializer.Deserialize<List<NoteDto>>(responseContent);

            var statusC = httpResponse.StatusCode;

            return (statusC, notes);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="noteText"></param>
        /// <returns></returns>
        public async Task<(ApiResponse apiResponse, NoteDto? note)> 
            CreateNewNote(string customerId, string noteText)
        {
            var baseUrl = _featureContext.Get<string>(ContextKeys.ApiBaseUrl);
            Uri uri = new($"{baseUrl}/{notesRoute}/{customerId}");

            string token = RetrieveCurrentUserToken();
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var formContent = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("noteText", noteText)
            });

            var httpResponse = await _httpClient.PostAsync(uri, formContent);
            var statusC = httpResponse.StatusCode;

            var responseContent = await httpResponse.Content.ReadAsStringAsync();

            var apiResponse = new ApiResponse
            {
                StatusCode = statusC,
                ResponseBody = responseContent
            };

            if (statusC == HttpStatusCode.Created)
            {
                NoteDto? createdNote = JsonSerializer.Deserialize<NoteDto>(responseContent);

                if (createdNote == null)
                {
                    Assert.Fail("Invalid Note");
                }

                // Hookup cleanup
                _scenarioContext.AddCleanUpStep(async () =>
                {
                    await DeleteNote(customerId, createdNote.ID);
                });

                return (apiResponse, createdNote);
            }

            return (apiResponse, null);
        }

    }
}
