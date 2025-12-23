using FluentAssertions;
using NUnit.Framework;
using Propeller.Integration.Tests.Drivers;
using Propeller.Integration.Tests.Support;
using Propeller.Models;
using Propeller.Models.Requests;
using System.Net;
using TechTalk.SpecFlow;

namespace Propeller.Integration.Tests.StepDefinitions
{
    [Binding]
    public class NoteManagementStepDefinitions : StepDefinitionsBase
    {
        private readonly NotesDriver _notesDriver;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scenarioContext"></param>
        /// <param name="featureContext"></param>
        public NoteManagementStepDefinitions(ScenarioContext scenarioContext,
            FeatureContext featureContext)
            : base(featureContext, scenarioContext)
        {
            _notesDriver = new NotesDriver(scenarioContext, featureContext);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="noteText"></param>
        /// <returns></returns>
        [Given(@"I add a new Note with text: ""([^""]*)""")]
        [Then(@"I add a new Note with text: ""([^""]*)""")]
        public async Task WhenIAddANewNoteWithId(string noteText)
        {
            string customerId = _scenarioContext.Get<string>(ContextKeys.NewCustomerId);
            var result = await _notesDriver.CreateNewNote(customerId, noteText);

            Assert.AreEqual(HttpStatusCode.Created, result.apiResponse.StatusCode);
            Assert.IsNotNull(result.note);

            _scenarioContext.Add(ContextKeys.NewNote, result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="noteText"></param>
        [Then(@"I verify the note with text: ""([^""]*)"" exists")]
        public void ThenIVerifyTheNoteWithTextExists(string noteText)
        {
            CustomerDto customer = GetScenarioCurrentCustomer();
            var note = customer.Notes.Where(x => x.Text.Equals(noteText)).FirstOrDefault();

            Assert.IsNotNull(note);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="noteText"></param>
        /// <returns></returns>
        [Then(@"I delete the Note with text: ""([^""]*)""")]
        public async Task ThenIDeleteTheNoteWithText(string noteText)
        {
            CustomerDto customer = GetScenarioCurrentCustomer();

            var note = customer.Notes.Where(x => x.Text.Equals(noteText)).FirstOrDefault();

            Assert.IsNotNull(note);

            var statusCode = await _notesDriver.DeleteNote(customer.ID, note.ID);

            Assert.AreEqual(HttpStatusCode.OK, statusCode);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Then(@"I verify the Customer does not have any Notes")]
        public async Task ThenIVerifyTheCustomerDoesNotHaveAnyNotes()
        {
            var customerId = _scenarioContext.Get<string>(ContextKeys.NewCustomerId);

            var result = await _notesDriver.RetrieveCustomerNotes(customerId);

            // TODO: This might not be useful since it's a tuple
            result.Should().NotBeNull();

            Assert.AreEqual(HttpStatusCode.OK, result.statusCode);
            Assert.IsEmpty(result.notes);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="noteText"></param>
        /// <param name="customerId"></param>
        /// <returns></returns>
        [When(@"I try to add a new Note with Text: ""([^""]*)"" to a Client with Id: (.*)")]
        public async Task WhenITryToAddANewNoteWithTextToAClientWithId(string noteText, int customerId)
        {
            string cid = Obfuscator.ObfuscateId(customerId);
            (ApiResponse ApiResponse, NoteDto? Note) result = await _notesDriver.CreateNewNote(cid, noteText);

            SetScenarioLatestStatusCode(result.ApiResponse.StatusCode);

            _scenarioContext.Set<ApiResponse>(result.ApiResponse, ContextKeys.LastReturnedApiResponse);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="noteText"></param>
        /// <returns></returns>
        [Then(@"I try to add a New Note with Text: ""([^""]*)""")]
        public async Task ThenITryToAddANewNoteWithText(string noteText)
        {
            (ApiResponse ApiResponse, NoteDto? Note) result = await AddNote(noteText);

            // TODO: Maybe remove this next one and just use the ApiResponse
            SetScenarioLatestStatusCode(result.ApiResponse.StatusCode);
            _scenarioContext.Set<ApiResponse>(result.ApiResponse, ContextKeys.LastReturnedApiResponse);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="stringLength"></param>
        [Then(@"I try to add a New Note with (.*) characters")]
        public async Task ThenITryToAddANewNoteWithCharacters(int stringLength)
        {
            string longString = new string('*', stringLength);
            (ApiResponse ApiResponse, NoteDto? Note) result = await AddNote(longString);

            // TODO: Maybe remove this next one and just use the ApiResponse
            SetScenarioLatestStatusCode(result.ApiResponse.StatusCode);
            _scenarioContext.Set<ApiResponse>(result.ApiResponse, ContextKeys.LastReturnedApiResponse);
        }

        [Then(@"I verify the Response is ""([^""]*)""")]
        public void ThenIVerifyTheResponseIs(string expectedRespoonse)
        {
            var latestReturnedApiResponse = _scenarioContext.Get<ApiResponse>(ContextKeys.LastReturnedApiResponse);
            Assert.AreEqual(latestReturnedApiResponse.ResponseBody, expectedRespoonse);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="noteText"></param>
        /// <returns></returns>
        private async Task<(ApiResponse apiResponse, NoteDto? note)> AddNote(string noteText)
        {
            string customerId = _scenarioContext.Get<string>(ContextKeys.NewCustomerId);
            return await _notesDriver.CreateNewNote(customerId, noteText);
        }

    }
}
