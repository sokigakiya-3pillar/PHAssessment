using NUnit.Framework;
using Propeller.Models;
using System.Net;
using TechTalk.SpecFlow;

namespace Propeller.Integration.Tests.StepDefinitions
{

    public class StepDefinitionsBase
    {

        protected readonly FeatureContext _featureContext;
        protected readonly ScenarioContext _scenarioContext;

        public StepDefinitionsBase(FeatureContext featureContext, ScenarioContext scenarioContext)
        {
            _featureContext = featureContext;
            _scenarioContext = scenarioContext;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="contact"></param>
        protected void SetScenarioCurrentContact(ContactDto contact)
        {
            Assert.IsNotNull(contact);

            _scenarioContext.Set(contact, ContextKeys.CurrentContact);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected ContactDto GetScenarioCurrentContact()
        {
            ContactDto contact;

            if (_scenarioContext.TryGetValue<ContactDto>(ContextKeys.CurrentContact, out contact))
            {
                return contact;
            }

            Assert.Fail("Unable to retrieve Current Contact");
            return contact;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="customer"></param>
        protected void SetScenarioCurrentCustomer(CustomerDto customer)
        {
            Assert.IsNotNull(customer);

            _scenarioContext.Set(customer, ContextKeys.CurrentCustomerX);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected CustomerDto GetScenarioCurrentCustomer()
        {
            CustomerDto customer;

            if (_scenarioContext.TryGetValue<CustomerDto>(ContextKeys.CurrentCustomerX, out customer))
            {
                return customer;
            }

            Assert.Fail("Unable to retrieve Current Customer");
            return customer;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="statusCode"></param>
        protected void SetScenarioLatestStatusCode(HttpStatusCode statusCode)
        {
            Assert.IsNotNull(statusCode);

            _scenarioContext.Set(statusCode, ContextKeys.LastReturnedStatusCodeX);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected HttpStatusCode GetScenarioLatestStatusCode()
        {
            HttpStatusCode statusCode;

            if (!_scenarioContext.TryGetValue<HttpStatusCode>(ContextKeys.LastReturnedStatusCodeX, out statusCode))
            {
                Assert.Fail($"Unable to Retrieve {ContextKeys.LastReturnedStatusCodeX}");
            }

            return statusCode;
        }

    }
}
