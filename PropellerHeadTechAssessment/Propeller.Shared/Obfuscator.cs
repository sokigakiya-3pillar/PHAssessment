using System.Text;

namespace Propeller.Shared
{
    public static class Obfuscator
    {
        /// <summary>
        /// Obfuscate will always be used for ID's, an empty string is safe to be used since it'll never happen
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string ObfuscateId(int value)
        {
            try
            {
                return Convert.ToBase64String(Encoding.UTF8.GetBytes(value.ToString()));
            }
            catch (Exception)
            {
                return string.Empty;
            }

        }

        /// <summary>
        /// Deobfuscation is used for ID's, so a -1 value will never be valid, safe to use
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int DeobfuscateId(string value)
        {
            try
            {
                var v = Encoding.UTF8.GetString(Convert.FromBase64String(value));

                if (!int.TryParse(v, out int result))
                {
                    throw new Exception("INvalid Value");
                }

                return result;

            }
            catch (Exception)
            {
                return -1;
            }
        }

    }
}
