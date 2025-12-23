namespace Propeller.Models
{
    public class PropellerUser
    {
        public string Name { get; set; } = string.Empty;
        public int Role { get; set; }
        public string Locale { get; set; } = string.Empty;
        public string CountryCode { get; set; } = string.Empty;

    }
}
