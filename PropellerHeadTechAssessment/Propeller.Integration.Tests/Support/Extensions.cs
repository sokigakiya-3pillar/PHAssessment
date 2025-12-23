using System.Text;

namespace Propeller.Integration.Tests.Support
{
    public static class Extensions
    {
        public static string Obfuscate(this int value)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(value.ToString()));
        }

        public static int Deobfuscate(this string value)
        {
            var v = Encoding.UTF8.GetString(Convert.FromBase64String(value));

            if (!int.TryParse(v, out int result))
            {
                throw new Exception("Invalid Value");
            }
            return result;

        }

    }
}
