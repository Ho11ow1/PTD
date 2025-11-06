using System;

using PTD.Core.Config;

namespace PTD.Core
{
    public class Program
    {
        public static void Main(string[] args)
        {
            SetGlobalConfig();


        }

        private static void SetGlobalConfig()
        {
            GlobalConfig.EnableLogging(true);
            GlobalConfig.AddEOFLines(true);
            GlobalConfig.PlaceBracketsOnNewLines(true);

            Console.WriteLine($"Global Configuration:\n" +
                              $" - Logging preference: {GlobalConfig.Logging}\n" +
                              $" - End of File Lines preference: {GlobalConfig.EndOfFileLines}\n" +
                              $" - Brackets on new Lines preference: {GlobalConfig.BracketsOnNewLines}\n");
        }


    }
}
