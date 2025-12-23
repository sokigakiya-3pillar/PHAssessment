using TechTalk.SpecFlow;

namespace Propeller.Integration.Tests.Support
{
    public static class SpecflowExtensions
    {
        // Specflow
        public static void AddCleanUpStep(this ScenarioContext context, Func<Task> cleanUpAction)
        {
            if (!context.TryGetValue(ContextKeys.CleanUp, out List<Func<Task>> value))
            {
                value = new List<Func<Task>>();
            }

            value.Add(cleanUpAction);
            context[ContextKeys.CleanUp] = value;
        }
    }
}
