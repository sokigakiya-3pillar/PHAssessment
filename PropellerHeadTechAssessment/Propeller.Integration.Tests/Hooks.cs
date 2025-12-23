using Microsoft.Extensions.Configuration;
using Propeller.Integration.Tests.Drivers;
using System.Net;
using Propeller.Models.Requests;
using NUnit.Framework;
using TechTalk.SpecFlow;

namespace Propeller.Integration.Tests
{
    [Binding]
    public class Hooks
    {
        private readonly ScenarioContext _scenarioContext;
        private readonly FeatureContext _featureContext;
        private readonly AuthenticationDriver _authenticationDriver;
        private IConfiguration _configuration;

        public Hooks(ScenarioContext scenarioContext,
            FeatureContext featureContext,
            AuthenticationDriver authenticationDriver)
        {
            _scenarioContext = scenarioContext ?? throw new ArgumentNullException(nameof(scenarioContext));
            _featureContext = featureContext ?? throw new ArgumentNullException(nameof(featureContext));
            _authenticationDriver = authenticationDriver ?? throw new ArgumentNullException(nameof(authenticationDriver));
        }

        /// <summary>
        /// 
        /// </summary>
        [BeforeScenario("RequiresAdminUser")]
        public async Task SetUpAdminUserForScenario()
        {
            Console.Out.WriteLine("-SetUpAdminUserForScenario");

            string userName = _featureContext.Get<string>(ContextKeys.AdminUserName);
            string password = _featureContext.Get<string>(ContextKeys.AdminPassword);

            (HttpStatusCode statusCode, string token) adminAuthResponse
                = await _authenticationDriver.Authenticate(new AuthRequest { UserId = userName, Password = password });

            if (adminAuthResponse.statusCode != HttpStatusCode.OK)
            {
                Assert.Fail("Unable to Auth Admin");
            }

            _featureContext.Set<string>(adminAuthResponse.token, ContextKeys.AdminBearerToken);

            Console.Out.WriteLine("-End SetUpAdminUserForScenario");
        }

        /// <summary>
        /// 
        /// </summary>
        [BeforeScenario("RequiresPowerUser")]
        public async Task SetUpPowerUserForScenario()
        {
            Console.Out.WriteLine("-SetUpPowerUserForScenario");

            string userName = _featureContext.Get<string>(ContextKeys.PowerUserName);
            string password = _featureContext.Get<string>(ContextKeys.PowerPassword);

            (HttpStatusCode statusCode, string token) powerAuthResponse
                = await _authenticationDriver.Authenticate(new AuthRequest { UserId = userName, Password = password });

            if (powerAuthResponse.statusCode != HttpStatusCode.OK)
            {
                Assert.Fail("Unable to Auth Admin");
            }

            _featureContext.Set<string>(powerAuthResponse.token, ContextKeys.PowerBearerToken);

            Console.Out.WriteLine("-End SetUpPowerUserForScenario");
        }

        /// <summary>
        /// 
        /// </summary>
        [BeforeScenario("RequiresRegularUser")]
        public async Task SetUpRegularUserForScenario()
        {
            //something to set up the user
            Console.Out.WriteLine("-SetUpRegularUserForScenario");

            string userName = _featureContext.Get<string>(ContextKeys.RegularUserName);
            string password = _featureContext.Get<string>(ContextKeys.RegularPassword);

            (HttpStatusCode statusCode, string token) userAuthResponse
                = await _authenticationDriver.Authenticate(new AuthRequest { UserId = userName, Password = password });

            if (userAuthResponse.statusCode != HttpStatusCode.OK)
            {
                Assert.Fail("Unable to Auth User");
            }

            _featureContext.Set<string>(userAuthResponse.token, ContextKeys.UserBearerToken);

            Console.Out.WriteLine("-End SetUpRegularUserForScenario");
        }

        /// <summary>
        /// 
        /// </summary>
        [Before(Order = 0)]
        public void BeforeScenarioSetup()
        {
            Console.Out.WriteLine("-BeforeScenarioSetup");

            if (_configuration == null)
            {
                var configBuilder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("specflow.json");

                _configuration = configBuilder.Build();
            }

            //var mongoDbUrl = new MongoUrl(_configuration["connectionStrings:MongoDb"]);
            //var mongoDbClient = new MongoClient(mongoDbUrl);
            //var mongoDatabase = mongoDbClient.GetDatabase(mongoDbUrl.DatabaseName);
            //_context.Set(mongoDatabase, ContextKeys.MongoDatabase);

            var apiBaseUrl = $"{_configuration["HTTPTYPE"]}{_configuration["BASEAPIURL"]}/api";
            _featureContext.Set(apiBaseUrl, ContextKeys.ApiBaseUrl);

            string adminUserName = _configuration["Auth:AdminUserName"] ?? string.Empty;
            string adminPassword = _configuration["Auth:AdminPassword"] ?? string.Empty;
            string powerUserName = _configuration["Auth:PowerUserName"] ?? string.Empty;
            string powerPassword = _configuration["Auth:PowerPassword"] ?? string.Empty;
            string userName = _configuration["Auth:RegularUserName"] ?? string.Empty;
            string password = _configuration["Auth:RegularPassword"] ?? string.Empty;

            _featureContext.Set(adminUserName, ContextKeys.AdminUserName);
            _featureContext.Set(adminPassword, ContextKeys.AdminPassword);

            _featureContext.Set(powerUserName, ContextKeys.PowerUserName);
            _featureContext.Set(powerPassword, ContextKeys.PowerPassword);

            _featureContext.Set(userName, ContextKeys.RegularUserName);
            _featureContext.Set(password, ContextKeys.RegularPassword);

        }

        //[BeforeFeature]
        //public void X()
        //{
        //    if (_configuration == null)
        //    {
        //        var configBuilder = new ConfigurationBuilder()
        //            .SetBasePath(Directory.GetCurrentDirectory())
        //            .AddJsonFile("specflow.json");

        //        _configuration = configBuilder.Build();
        //    }
        //}

        //[BeforeFeature]
        //public void BeforeFeatureSetup()
        //{
        //    if (_configuration == null)
        //    {
        //        var configBuilder = new ConfigurationBuilder()
        //            .SetBasePath(Directory.GetCurrentDirectory())
        //            .AddJsonFile("specflow.json");

        //        _configuration = configBuilder.Build();
        //    }

        //    //var mongoDbUrl = new MongoUrl(_configuration["connectionStrings:MongoDb"]);
        //    //var mongoDbClient = new MongoClient(mongoDbUrl);
        //    //var mongoDatabase = mongoDbClient.GetDatabase(mongoDbUrl.DatabaseName);
        //    //_context.Set(mongoDatabase, ContextKeys.MongoDatabase);

        //    var apiBaseUrl = $"{_configuration["HTTPTYPE"]}{_configuration["BASEAPIURL"]}/api";
        //    _featureContext.Set(apiBaseUrl, ContextKeys.ApiBaseUrl);

        //    string adminUserName = _configuration["Auth:AdminUserName"] ?? string.Empty;
        //    string adminPassword = _configuration["Auth:AdminPassword"] ?? string.Empty;
        //    string userName = _configuration["Auth:RegularUserName"] ?? string.Empty;
        //    string password = _configuration["Auth:RegularPassword"] ?? string.Empty;

        //    _featureContext.Set(adminUserName, ContextKeys.AdminUserName);
        //    _featureContext.Set(adminPassword, ContextKeys.AdminPassword);

        //    _featureContext.Set(userName, ContextKeys.RegularUserName);
        //    _featureContext.Set(password, ContextKeys.RegularPassword);

        //    // Authenticate Admin
        //    //(HttpStatusCode statusCode, string token) adminAuthResponse
        //    //    = await _authenticationDriver.Authenticate(new AuthRequest { UserId = adminUserName, Password = adminPassword });

        //    //if (adminAuthResponse.statusCode != HttpStatusCode.OK)
        //    //{
        //    //    Assert.Fail("Unable to Auth Admin");
        //    //}

        //    //_featureContext.Set<string>(adminAuthResponse.token, ContextKeys.AdminBearerToken);

        //    //(HttpStatusCode statusCode, string token) userAuthResponse
        //    //    = await _authenticationDriver.Authenticate(new AuthRequest { UserId = userName, Password = password });

        //    //if (userAuthResponse.statusCode != HttpStatusCode.OK)
        //    //{
        //    //    Assert.Fail("Unable to Auth User");
        //    //}

        //    //_featureContext.Set<string>(userAuthResponse.token, ContextKeys.UserBearerToken);
        //}


    }
}
