namespace Propeller.Integration.Tests
{
    public static class ContextKeys
    {
        // Used in external packages
        public const string AuthenticationResponse = "AuthenticationResponse";
        public const string ApiBaseUrl = "apiBaseUrl";

        public const string AdminUserName = "AdminUserName";
        public const string AdminPassword = "AdminPassword";

        public const string PowerUserName = "PowerUserName";
        public const string PowerPassword = "PowerPassword";

        public const string RegularUserName = "RegularUserName";
        public const string RegularPassword = "RegularPassword";

        public const string CustomerID = "CustomerId";
        public const string NewNote = "NewNote";

        public const string AdminBearerToken = "AdminBearerToken";
        public const string PowerBearerToken = "PowerBearerToken";
        public const string UserBearerToken = "UserBearerToken";

        public const string NewCustomerId = "NewCustomerId";
        public const string CreatedNoteId = "CreatedNoteId";

        public const string FoundCustomers = "FoundCustomers";
        public const string FoundContacts = "FoundContacts";

        public const string LastReturnedStatusCodeX = "LastReturnedStatusCode";
        public const string LastReturnedApiResponse = "LastReturnedApiResponse";
        public const string CurrentCustomerX = "CurrentCustomer";
        public const string CurrentContact = "CurrentContact";

        public const string CustomerStatuses = "CustomerStatuses";
        public const string CleanUp = "CleanUp";
        public static string CurrentUserProfile = "CurrentUserProfile";
    }
}
