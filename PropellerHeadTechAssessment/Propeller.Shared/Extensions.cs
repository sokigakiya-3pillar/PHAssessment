namespace Propeller.Shared
{
    public static class Extensions
    {

        public static string Obfuscate(this int value)
        {
            return Obfuscator.ObfuscateId(value);
        }


        public static int Deobfuscate(this string value)
        {
            return Obfuscator.DeobfuscateId(value);
        }

    }
}
