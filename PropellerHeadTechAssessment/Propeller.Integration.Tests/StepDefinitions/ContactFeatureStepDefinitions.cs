using FluentAssertions;
using NUnit.Framework;
using Propeller.Integration.Tests.Drivers;
using Propeller.Integration.Tests.Support;
using Propeller.Models;
using Propeller.Models.Requests;
using System.Net;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Assist;

namespace Propeller.Integration.Tests.StepDefinitions
{
    [Binding]
    public class ContactFeatureStepDefinitions : StepDefinitionsBase
    {

        private readonly ContactDriver _contactDriver;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scenarioContext"></param>
        /// <param name="featureContext"></param>
        public ContactFeatureStepDefinitions(ScenarioContext scenarioContext,
            FeatureContext featureContext)
            : base(featureContext, scenarioContext)
        {
            _contactDriver = new ContactDriver(scenarioContext, featureContext);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        [When(@"I add a new Contact with data")]
        public async Task WhenIAddANewContactWithData(Table dataTable)
        {
            // TODO: Address this possible nullref
            CreateContactRequest newContact = dataTable.CreateSet<CreateContactRequest>().FirstOrDefault();

            if (newContact == null)
            {
                Assert.Fail("Invalid New Contact Data");
            }

            // Customer Id is set as an int on the table, but we need to obfuscate it to send
            if (!string.IsNullOrEmpty(newContact.CustomerID))
            {
                int cid;

                if (!int.TryParse(newContact.CustomerID, out cid))
                {
                    Assert.Fail("Invalid Customer ID");
                }

                newContact.CustomerID = cid.Obfuscate();
            }

            (ApiResponse ApiResponse, ContactDto? Contact) result = await _contactDriver.AddNewContact(newContact);

            if (result.ApiResponse.StatusCode == HttpStatusCode.Created)
            {
                SetScenarioCurrentContact(result.Contact);
            }

            SetScenarioLatestStatusCode(result.ApiResponse.StatusCode);
            _scenarioContext.Set(result.ApiResponse, ContextKeys.LastReturnedApiResponse);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p0"></param>
        /// <exception cref="PendingStepException"></exception>
        [When(@"I retrieve Contacts with Search Criteria: ""([^""]*)""")]
        public async Task WhenIRetrieveContactsWithSearchCriteria(string searchCriteria)
        {
            (HttpStatusCode statusCode, List<ContactDto>? contacts) result
                       = await _contactDriver.RetrieveContactsBySearch(searchCriteria, string.Empty, 0, 0);

            // Search should return Ok regardless of result
            Assert.AreEqual(HttpStatusCode.OK, result.statusCode);

            _scenarioContext.Set(result.contacts, ContextKeys.FoundContacts);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="recordsExpected"></param>
        [Then(@"I verify only (.*) record\(s\) were retrieved")]
        public void ThenIVerifyOnlyRecordSWereRetrieved(int recordsExpected)
        {
            List<ContactDto> foundContacts = new List<ContactDto>();

            if (!_scenarioContext.TryGetValue<List<ContactDto>>(ContextKeys.FoundContacts, out foundContacts))
            {
                Assert.Fail("Unable to retrieve Found Contacts");
            }

            foundContacts.Should().HaveCount(recordsExpected);
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="p0"></param>
        /// <exception cref="PendingStepException"></exception>
        [Then(@"I should have retrieved (.*) Contact\(s\)")]
        public void WhenIShouldHaveRetrievedContactS(int contactsExpected)
        {
            List<ContactDto> contacts = _scenarioContext.Get<List<ContactDto>>(ContextKeys.FoundContacts);

            if (contacts == null)
            {
                throw new Exception("No Contracts in Context");
            }

            contacts.Count.Should().Be(contactsExpected);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <exception cref="Exception"></exception>
        [Then(@"I check the Contact has First Name: ""([^""]*)"" and Last Name: ""([^""]*)""")]
        public void ThenICheckTheContactHasFirstNameAndLastName(string firstName, string lastName)
        {
            List<ContactDto> contacts = _scenarioContext.Get<List<ContactDto>>(ContextKeys.FoundContacts);

            if (contacts == null)
            {
                throw new Exception("No Contracts in Context");
            }

            if (!contacts.Any())
            {
                throw new Exception("No Contracts found in Context");
            }

            // Retrieve the First row and check
            ContactDto? contact = contacts.FirstOrDefault();

            Assert.IsNotNull(contact);

            contact.FirstName.Should().Be(firstName);
            contact.LastName.Should().Be(lastName);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dataTable"></param>
        /// <returns></returns>
        [Then(@"I add a new Contact with data for the recently created Customer")]
        public async Task ThenIAddANewContactWithDataForTheRecentlyCreatedCustomer(Table dataTable)
        {

            string recentlyCreatedCustomerId = _scenarioContext.Get<string>(ContextKeys.NewCustomerId);

            if (string.IsNullOrEmpty(recentlyCreatedCustomerId))
            {
                Assert.Fail("Unable to retrieve CustomerID");
            }

            CreateContactRequest newContact = dataTable.CreateSet<CreateContactRequest>().FirstOrDefault();

            if (newContact == null)
            {
                Assert.Fail("Invalid New Contact Data");
            }

            newContact.CustomerID = recentlyCreatedCustomerId;

            (ApiResponse ApiResponse, ContactDto? Contact) result = await _contactDriver.AddNewContact(newContact);

            if (result.ApiResponse.StatusCode == HttpStatusCode.Created)
            {
                SetScenarioCurrentContact(result.Contact);
            }

            SetScenarioLatestStatusCode(result.ApiResponse.StatusCode);
            _scenarioContext.Set(result.ApiResponse, ContextKeys.LastReturnedApiResponse);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        [Then(@"I verify it contains a Contact with Email: ""([^""]*)""")]
        public void ThenIVerifyItContainsAContactWithEmail(string email)
        {
            CustomerDto customer = GetScenarioCurrentCustomer();

            customer.Contacts.Should().HaveCountGreaterThanOrEqualTo(1);

            ContactDto contact = customer.Contacts.FirstOrDefault();

            Assert.AreEqual(email, contact.Email);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contactEmail"></param>
        /// <returns></returns>
        [When(@"I retrieve a Contact with Email: ""([^""]*)""")]
        public async Task WhenIRetrieveAContactWithEmail(string contactEmail)
        {

            (HttpStatusCode StatusCode, List<ContactDto>? Contacts) result
                = await _contactDriver.RetrieveContactsBySearch(contactEmail, "Email", 0, 0);

            if (result.StatusCode != HttpStatusCode.OK)
            {
                // Regardless of number of records fetched or parameters, Search should always return 200
                Assert.Fail("Error Retrieving Contacts");
            }

            result.Contacts.Should().HaveCountGreaterThanOrEqualTo(1);

            _scenarioContext.Set(result.Contacts, ContextKeys.FoundContacts);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [When(@"I forcefully remove the recently created Contact")]
        public async Task WhenIRemoveTheRecentlyCreatedContact()
        {
            ContactDto contact = GetScenarioCurrentContact();
            
            HttpStatusCode statusCode = await _contactDriver.DeleteContact(contact.Id, true);

            Assert.AreEqual(HttpStatusCode.NoContent, statusCode);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p0"></param>
        [Then(@"I verify it does not contain a Contact with Email: ""([^""]*)""")]
        public void ThenIVerifyItDoesNotContainAContactWithEmail(string p0)
        {
            CustomerDto customer = GetScenarioCurrentCustomer();

            // TODO: Missing search
            customer.Contacts.Should().HaveCount(0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="p0"></param>
        /// <exception cref="PendingStepException"></exception>
        [When(@"I try to Delete a Contact with Id: (.*)")]
        public async Task WhenITryToDeleteAContactWithId(int contactId)
        {
            HttpStatusCode statusCode = await _contactDriver.DeleteContact(contactId, true);
            SetScenarioLatestStatusCode(statusCode);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="PendingStepException"></exception>
        [When(@"I non forcefully try to remove the recently created Contact")]
        public async Task WhenINonForcefullyTryToRemoveTheRecentlyCreatedContact()
        {
            ContactDto contact = GetScenarioCurrentContact();

            HttpStatusCode result = await _contactDriver.DeleteContact(contact.Id, false);

            SetScenarioLatestStatusCode(result);
        }

    }
}
