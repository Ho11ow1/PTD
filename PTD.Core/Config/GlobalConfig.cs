using System;
using System.IO;
using System.Text.Json;
using System.Reflection;

namespace PTD.Core.Config
{
    public static class GlobalConfig
    {
        public static class Files
        {
            internal static string ProjectRoot { get; private set; } = Path.Join(Directory.GetCurrentDirectory(), "..", "..", "..", "..");
            internal const string JSONConfigFilePath = "config.json";
            internal static string OutputDirectory { get; set; } = "PTD_Output";
            public static string OutputFile { get; internal set; } = "";
            public static string LogFile { get; internal set; } = "";
            public static string InputFileLocation { get; internal set; } = "";
        }

        public static class Formatting
        {
            public static bool AllowLogging { get; internal set; } = true;
            public static bool AddEOFLines { get; internal set; } = true;
            public static bool BracketsOnNewLines { get; internal set; } = true;
            public static int IndentSize { get; internal set; } = 4;
            public static bool UseSpaces { get; internal set; } = true;
        }

        public static class Performance
        {
            public static int RetryDelayMilliseconds { get; internal set; } = 100;
        }

        public static class Decompilation
        {
            public static bool IncludePrivateMembers { get; internal set; } = true;
            public static bool IncludeInternalMembers { get; internal set; } = true;
        }

        public static void LoadConfig()
        {
            UpdatePaths();

            string jsonPath = Path.Combine(Files.ProjectRoot, Files.JSONConfigFilePath);
            string jsonContent = File.ReadAllText(jsonPath);
            jsonContent = jsonContent.Replace(@"\", "/");

            using (var jsonDoc = JsonDocument.Parse(jsonContent))
            {
                ApplyJsonToProps(typeof(GlobalConfig), jsonDoc.RootElement);
            }
        }

        private static void UpdatePaths()
        {
            Files.OutputDirectory = Path.Combine(Files.ProjectRoot, Files.OutputDirectory);
            if (!Directory.Exists(Files.OutputDirectory))
            {
                Directory.CreateDirectory(Files.OutputDirectory);
            }
            Files.OutputFile = Path.Combine(Files.OutputDirectory, Files.OutputFile);
        }

        private static void ApplyJsonToProps(Type type, JsonElement jsonElement)
        {
            foreach (var prop in jsonElement.EnumerateObject())
            {
                string propName = ConvertToPropName(prop.Name);

                var nestedClass = type.GetNestedType(propName, BindingFlags.Public | BindingFlags.Static);
                if (nestedClass != null)
                {
                    ApplyJsonToProps(nestedClass, prop.Value);
                    continue;
                }

                var propertyInfo = type.GetProperty(propName, BindingFlags.Public | BindingFlags.Static);
                if (propertyInfo == null) { continue; }

                object? value = JsonSerializer.Deserialize(prop.Value.GetRawText(), propertyInfo.PropertyType);
                propertyInfo.SetValue(null, value);
            }
        }

        private static string ConvertToPropName(string jsonProp)
        {
            return char.ToUpper(jsonProp[0]) + jsonProp.Substring(1, jsonProp.Length - 1);
        }
    }
}
