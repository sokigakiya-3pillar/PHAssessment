using TechTalk.SpecFlow;

namespace Propeller.Integration.Tests.Drivers
{
    public class DriverBase
    {

        protected readonly ScenarioContext _scenarioContext;
        protected readonly FeatureContext _featureContext;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="scenarioContext"></param>
        /// <param name="featureContext"></param>
        /// <exception cref="ArgumentNullException"></exception>
        public DriverBase(ScenarioContext scenarioContext, FeatureContext featureContext)
        {
            _scenarioContext = scenarioContext ?? throw new ArgumentNullException(nameof(scenarioContext));
            _featureContext = featureContext ?? throw new ArgumentNullException(nameof(featureContext));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected string RetrieveCurrentUserToken()
        {
            UserProfile userProfile = _scenarioContext.Get<UserProfile>(ContextKeys.CurrentUserProfile);

            string key;
            switch (userProfile)
            {
                case UserProfile.Regular:
                    key = ContextKeys.UserBearerToken;
                    break;

                case UserProfile.Power:
                    key = ContextKeys.PowerBearerToken;
                    break;

                default:
                    key = ContextKeys.AdminBearerToken;
                    break;
            }

            string token = _featureContext.Get<string>(key);
            return token;
        }

    }
}
