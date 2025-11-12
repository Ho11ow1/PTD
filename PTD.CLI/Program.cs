using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;

using PTD.Core.Config;

namespace PTD.CLI
{
    public class Program
    {
        public static void Main(string[] args)
        {
            SetGlobalConfig();

            Processing.Decompiler decompiler = new Processing.Decompiler();
            decompiler.DecompileCS("", "");
        }

        private static void SetGlobalConfig()
        {
            GlobalConfig.AllowLogging(true);
            GlobalConfig.AddEOFLines(true);
            GlobalConfig.PlaceBracketsOnNewLines(true);

            Console.WriteLine($"Global Configuration:\n" +
                              $" - Logging preference: {GlobalConfig.Logging}\n" +
                              $" - End of File Lines preference: {GlobalConfig.EndOfFileLines}\n" +
                              $" - Brackets on new Lines preference: {GlobalConfig.BracketsOnNewLines}\n");
        }


    }
}
