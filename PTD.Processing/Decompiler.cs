using System;
using System.IO;
using System.Text;
using System.Reflection;

using PTD.Core.Interfaces;
using PTD.Core.Logging;
using PTD.Processing.Types;

namespace PTD.Processing
{
    public class Decompiler : IDecompiler
    {
        private readonly BindingFlags _bindingFlags = BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance;
        
        public void DecompileCS(string inputFilePath, string outputFileName)
        {
            using var logger = new FileLogger("Decompiler.log");
            logger.Log($"Starting decompilation of C# file {Path.GetFileNameWithoutExtension(inputFilePath)}");

            Assembly assembly = Assembly.LoadFrom(inputFilePath);
            StringBuilder sb = new StringBuilder();
            using var writer = new FileLogger(outputFileName);

            foreach (var cls in assembly.GetTypes())
            {
                SanitizedObject sc = new SanitizedObject(cls);

                sb.AppendLine(sc.ToString());
                sb.AppendLine("{");

                foreach (var field in cls.GetFields(_bindingFlags))
                {
                    SanitizedField sf = new SanitizedField(field);
                    sb.AppendLine(sf.ToString());
                }

                foreach (var prop in cls.GetProperties(_bindingFlags))
                {
                    SanitizedProperty sp = new SanitizedProperty(prop);
                    sb.AppendLine(sp.ToString());
                }

                foreach (var method in cls.GetMethods(_bindingFlags))
                {
                    SanitizedMethod sm = new SanitizedMethod(method);
                    sb.AppendLine(sm.ToString());
                }

                sb.AppendLine("}");

                writer.Write(FileLogger.LogLevel.None, sb.ToString());
                sb.Clear();
            }
        }
    }
}
