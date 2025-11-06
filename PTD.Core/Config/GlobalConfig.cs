namespace PTD.Core.Config
{
    public static class GlobalConfig
    {
        // Configuration constants
        public const string OutputDirectory = "PTD_Output";
        public const int RetryDelayMilliseconds = 100;

        // Configuration flags
        private static bool _logging = true;
        private static bool _eofLines = true;
        private static bool _bracketsOnNewLines = true;
        public static bool Logging => _logging;
        public static bool EndOfFileLines => _eofLines;
        public static bool BracketsOnNewLines => _bracketsOnNewLines;

        public static void EnableLogging(bool enableLogging) { _logging = enableLogging; }
        public static void AddEOFLines(bool addNewLines) { _eofLines = addNewLines; }
        public static void PlaceBracketsOnNewLines(bool bracketsOnNewLines) { _bracketsOnNewLines = bracketsOnNewLines; }
    }
}
