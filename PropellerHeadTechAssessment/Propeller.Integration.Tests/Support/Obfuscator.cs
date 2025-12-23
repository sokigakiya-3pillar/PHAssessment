using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Propeller.Integration.Tests.Support
{
    public static class Obfuscator
    {
        public static string ObfuscateId(int value)
        {
            return Convert.ToBase64String(Encoding.UTF8.GetBytes(value.ToString()));
        }

        /// <summary>
        /// Returns a deobfuscated Id, will return -1 in case of any error
        /// Ids will never be negative numbers
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int DeobfuscateId(string value)
        {
            if (string.IsNullOrEmpty(value))
                return -1;

            try
            {
                var v = Encoding.UTF8.GetString(Convert.FromBase64String(value));

                if (!int.TryParse(v, out int result))
                {
                    return -1;
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
