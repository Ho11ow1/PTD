using PTD.Core.Interfaces;
using PTD.Core.Logging;

namespace PTD.Processing
{
    public class Parser : IParser
    {
        private const int systemPrefixLength = 7;
        public void ParseCS(string inputFilePath, string outputFileName)
        {
            using var logger = new FileLogger("Parser.log");
            logger.Log($"Starting parsing of decompiled C# file {inputFilePath}");


        }

        public static string ParseSystemNames(string input)
        {
            return input.StartsWith("System.") ? input.Substring(systemPrefixLength, input.Length - systemPrefixLength) : input;
        }
    }
}
