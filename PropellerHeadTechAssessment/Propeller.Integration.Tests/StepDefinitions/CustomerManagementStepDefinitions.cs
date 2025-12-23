using FluentAssertions;
using NUnit.Framework;
using Propeller.Integration.Tests.Drivers;
using Propeller.Models;
using Propeller.Models.Requests;
using System.Net;
using TechTalk.SpecFlow;

namespace Propeller.Integration.Tests.StepDefinitions
{
    [Binding]
    public class CustomerManagementStepDefinitions : StepDefinitionsBase
    {

        private readonly CustomerDriver _customerDriver;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public CustomerManagementStepDefinitions(ScenarioContext scenarioContext,
            FeatureContext featureContext)
            : base(featureContext, scenarioContext)
        {
            //_scenarioContext = scenarioContext;
            //_featureContext = featureContext;
            _customerDriver = new CustomerDriver(scenarioContext, featureContext);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerName"></param>
        /// <param name="statusCode"></param>
        /// <returns></returns>
        [Given(@"I try to create a Customer with name: ""([^""]*)"" and Status: ""([^""]*)""")]
        [When(@"I try to create a Customer with name: ""([^""]*)"" and Status: ""([^""]*)""")]
        public async Task GivenITryToCreateACustomerWithNameAndStatus(string customerName, string status)
        {
            // Validate Customer Status
            var statuses = _featureContext.Get<List<CustomerStatusDto>>(ContextKeys.CustomerStatuses);

            var customerState = statuses.Where(x => x.State.ToUpper().Equals(status.ToUpper())).FirstOrDefault();

            if (customerState == null)
            {
                Assert.Fail($"Invalid Customer Status: {status}");
            }

            CreateCustomerRequest request = new()
            {
                Name = customerName,
                Status = customerState.ID
            };

            (ApiResponse ApiResponse, CustomerDto Customer) result = await _customerDriver.AddNewCustomer(request);

            if (result.ApiResponse.StatusCode == HttpStatusCode.Created)
            {
                Assert.IsNotNull(result.Customer);

                // Save the Id for the newly created Customer
                //_lastCustomerId = result.Customer.ID;
                // _currentCustomer = result.Customer; // TODO: Should I clean this up and use it like this or inside a context?
                _scenarioContext.Set(result.Customer.ID, ContextKeys.NewCustomerId);
                SetScenarioCurrentCustomer(result.Customer);
            }

            SetScenarioLatestStatusCode(result.ApiResponse.StatusCode);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [When(@"I retrieve the Customer with it's notes")]
        public async Task WhenIRetrieveTheCustomerWithItsNotes()
        {
            string customerId = _scenarioContext.Get<string>(ContextKeys.NewCustomerId);

            var result = await _customerDriver.RetrieveCustomer(customerId);

            Assert.AreEqual(HttpStatusCode.OK, result.statusCode);
            Assert.IsNotNull(result.customer);

            SetScenarioCurrentCustomer(result.customer);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerName"></param>
        /// <returns></returns>
        [Then(@"I delete the Customer with name: ""([^""]*)""")]
        public async Task ThenIDeleteTheCustomerWithName(string customerName)
        {
            var result = await _customerDriver.RetrieveCustomersBySearch(customerName, 0, 0);

            Assert.AreEqual(HttpStatusCode.OK, result.statusCode);
            Assert.IsNotNull(result.customers);

            var customer = result.customers.Where(x => x.Name.Equals(customerName)).FirstOrDefault();

            Assert.IsNotNull(customer);

            var statusCode = await _customerDriver.DeleteCustomer(customer.ID, false);

            Assert.AreEqual(HttpStatusCode.OK, statusCode);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Then(@"I Verify the Customer does not exist anymore")]
        public async Task ThenIVerifyTheCustomerDoesNotExistAnymore()
        {
            string customerId = _scenarioContext.Get<string>(ContextKeys.NewCustomerId);

            var result = await _customerDriver.RetrieveCustomer(customerId);

            Assert.AreEqual(HttpStatusCode.NotFound, result.statusCode);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="searchCriteria"></param>
        /// <returns></returns>
        [Then(@"I search for Customers with name: ""([^""]*)""")]
        public async Task ThenISearchForCustomersWithName(string searchCriteria)
        {
            (HttpStatusCode statusCode, List<CustomerDto> customers) result
                = await _customerDriver.RetrieveCustomersBySearch(searchCriteria, 0, 0);

            Assert.AreEqual(HttpStatusCode.OK, result.statusCode);

            _scenarioContext.Set(result.customers, ContextKeys.FoundCustomers);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="customerName"></param>
        /// <returns></returns>
        [Then(@"I retrieve a Single Customer with name: ""([^""]*)""")]
        public async Task ThenIRetrieveASingleCustomerWithName(string customerName)
        {
            (HttpStatusCode statusCode, List<CustomerDto> customers) result =
                await _customerDriver.RetrieveCustomersBySearch(customerName, 0, 0);

            Assert.AreEqual(HttpStatusCode.OK, result.statusCode);

            // Make sure the record is unique
            result.customers.Count.Should().Be(1);

            CustomerDto customer = result.customers[0];

            SetScenarioCurrentCustomer(customer);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="recordsFound"></param>
        [Then(@"I should have (.*) record\(s\) found")]
        public void ThenIShouldHaveRecordReturned(int recordsFound)
        {
            var customers = _scenarioContext.Get<List<CustomerDto>>(ContextKeys.FoundCustomers);

            Assert.AreEqual(recordsFound, customers.Count());
        }

        [Then(@"I change the Customer Status to: ""([^""]*)""")]
        public async Task ThenITryToChangeTheCustomerStatusTo(string status)
        {
            var statusDto = VerifyCustomerStatus(status);

            // Retrieve current Costumer
            CustomerDto customer = GetScenarioCurrentCustomer();

            HttpStatusCode result = await
                _customerDriver.ChangeCustomerStatus(customer.ID, statusDto.ID);

            SetScenarioLatestStatusCode(result);
            Assert.AreEqual(HttpStatusCode.OK, result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="status"></param>
        [Then(@"I verify the Customer Status is ""([^""]*)""")]
        public void ThenIVerifyTheCustomerStatusIs(string status)
        {
            var statusDto = VerifyCustomerStatus(status);

            // Retrieve current Costumer
            CustomerDto customer = GetScenarioCurrentCustomer();

            Assert.AreEqual(statusDto.ID, customer.Status);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <exception cref="PendingStepException"></exception>
        [When(@"I retrieve the newly created Customer")]
        public async Task ThenIRetrieveTheNewlyCreatedCustomer()
        {
            string recentlyCreatedCustomerId = _scenarioContext.Get<string>(ContextKeys.NewCustomerId);

            if (string.IsNullOrEmpty(recentlyCreatedCustomerId))
            {
                Assert.Fail("Unable to retrieve CustomerID");
            }

            var result = await _customerDriver.RetrieveCustomer(recentlyCreatedCustomerId);

            Assert.AreEqual(HttpStatusCode.OK, result.statusCode);
            Assert.IsNotNull(result.customer);

            SetScenarioCurrentCustomer(result.customer);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        [Then(@"I try to change the Customer Status to and Invalid value")]
        public async Task ThenITryToChangeTheCustomerStatusToAndInvalidValue()
        {
            // Retrieve current Costumer
            CustomerDto customer = GetScenarioCurrentCustomer();

            Assert.IsNotNull(customer);

            HttpStatusCode result = await _customerDriver.ChangeCustomerStatus(customer.ID, "MTExMTE=");

            SetScenarioLatestStatusCode(result);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>

        private CustomerStatusDto VerifyCustomerStatus(string status)
        {
            // Validate Customer Status
            var statuses = _featureContext.Get<List<CustomerStatusDto>>(ContextKeys.CustomerStatuses);

            var customerState = statuses.Where(x => x.State.ToUpper().Equals(status.ToUpper())).FirstOrDefault();

            if (customerState == null)
            {
                Assert.Fail($"Invalid Customer Status: {status}");
            }

            return customerState;
        }

    }
}
