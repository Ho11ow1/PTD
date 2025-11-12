using System;
using System.IO;
using System.Text;
using System.Reflection;

using PTD.Core.Interfaces;
using PTD.Core.Logging;

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
                sb.AppendLine($"{ExtractAccessModifier(cls)} {cls.FullName}");
                sb.AppendLine("{");

                foreach (var field in cls.GetFields(_bindingFlags))
                {
                    sb.AppendLine($"    Field: {ExtractAccessModifier(field)} {Parser.ParseSystemNames(field.FieldType.ToString())} {field.Name};");
                }
                sb.AppendLine();
                foreach (var prop in cls.GetProperties(_bindingFlags))
                {
                    sb.AppendLine($"    Property: {ExtractAccessModifier(prop)} {Parser.ParseSystemNames(prop.PropertyType.ToString())} {prop.Name};");
                }
                sb.AppendLine();

                foreach (var method in cls.GetMethods(_bindingFlags))
                {
                    sb.AppendLine($"    Method: {ExtractAccessModifier(method)} {Parser.ParseSystemNames(method.ReturnType.ToString())} {method.Name}({ExtractMethodParams(method.GetParameters())})");
                    sb.AppendLine("    {");
                    sb.AppendLine("        // Method body not available yet");
                    sb.AppendLine("    }");
                    sb.AppendLine();
                }

                sb.AppendLine();
                sb.AppendLine("}");
                sb.AppendLine();

                writer.Write(FileLogger.LogLevel.None, sb.ToString());
                sb.Clear();
            }
        }

        private string ExtractMethodParams(ParameterInfo[] paramInfo)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var param in paramInfo)
            {
                sb.Append($"{Parser.ParseSystemNames(param.ParameterType.ToString())} {param.Name}, ");
            }

            return sb.ToString().TrimEnd(' ', ',');
        }

        private string ExtractAccessModifier(Type type)
        {
            StringBuilder sb = new StringBuilder();

            // --- Access modifiers ---
            if (type.IsPublic)
            {
                sb.Append("public ");
            }
            else if (type.IsNotPublic)
            {
                sb.Append("internal ");
            }
            else if (type.IsNestedPublic)
            {
                sb.Append("public ");
            }
            else if (type.IsNestedPrivate)
            {
                sb.Append("private ");
            }
            else if (type.IsNestedFamily)
            {
                sb.Append("protected ");
            }
            else if (type.IsNestedAssembly)
            {
                sb.Append("internal ");
            }
            else if (type.IsNestedFamORAssem)
            {
                sb.Append("protected internal ");
            }
            else if (type.IsNestedFamANDAssem)
            {
                sb.Append("private protected ");
            }
            // --- Other modifiers ---
            if (type.IsAbstract && type.IsSealed)
            {
                sb.Append("static ");
            }
            else if (type.IsAbstract)
            {
                sb.Append("abstract ");
            }
            else if (type.IsSealed)
            {
                sb.Append("sealed ");
            }

            // --- Type checking ---
            if (type.IsInterface)
            {
                sb.Append("interface ");
            }
            else if (type.IsEnum)
            {
                sb.Append("enum ");
            }
            else if (type.IsValueType)
            {
                sb.Append("struct ");
            }
            else if (type.IsClass)
            {
                sb.Append("class ");
            }

            return sb.ToString().Trim();
        }


        private string ExtractAccessModifier(MethodInfo methodInfo)
        {
            StringBuilder sb = new StringBuilder();

            // --- Access modifiers ---
            if (methodInfo.IsPublic)
            {
                sb.Append("public ");
            }
            else if (methodInfo.IsPrivate)
            {
                sb.Append("private ");
            }
            else if (methodInfo.IsFamily)
            {
                sb.Append("protected ");
            }
            else if (methodInfo.IsAssembly)
            {
                sb.Append("internal ");
            }
            else if (methodInfo.IsFamilyOrAssembly)
            {
                sb.Append("protected internal ");
            }
            else if (methodInfo.IsFamilyAndAssembly)
            {
                sb.Append("private protected ");
            }
            // --- Other modifiers ---
            if (methodInfo.IsStatic)
            {
                sb.Append("static ");
            }
            if (methodInfo.IsAbstract)
            {
                sb.Append("abstract ");
            }
            if (methodInfo.IsVirtual && !methodInfo.IsAbstract && !methodInfo.IsFinal)
            {
                sb.Append("virtual ");
            }
            if (methodInfo.IsFinal && methodInfo.IsVirtual)
            {
                sb.Append("sealed override ");
            }
            
            // --- Constructor check ---
            if (methodInfo.IsConstructor)
            {
                sb.Append(methodInfo.DeclaringType?.Name);
            }

            return sb.ToString().Trim();
        }

        private string ExtractAccessModifier(FieldInfo fieldInfo)
        {
            StringBuilder sb = new StringBuilder();

            // --- Access modifiers ---
            if (fieldInfo.IsPublic)
            {
                sb.Append("public ");
            }
            else if (fieldInfo.IsPrivate)
            {
                sb.Append("private ");
            }
            else if (fieldInfo.IsFamily)
            {
                sb.Append("protected ");
            }
            else if (fieldInfo.IsAssembly)
            {
                sb.Append("internal ");
            }
            else if (fieldInfo.IsFamilyOrAssembly)
            {
                sb.Append("protected internal ");
            }
            else if (fieldInfo.IsFamilyAndAssembly)
            {
                sb.Append("private protected ");
            }

            // --- Other modifiers ---
            if (fieldInfo.IsStatic)
            {
                sb.Append("static ");
            }
            if (fieldInfo.IsLiteral && !fieldInfo.IsInitOnly)
            {
                sb.Append("const ");
            }
            else if (fieldInfo.IsInitOnly)
            {
                sb.Append("readonly ");
            }

            return sb.ToString().Trim();
        }

        private string ExtractAccessModifier(PropertyInfo propInfo)
        {
            MethodInfo getter = propInfo.GetGetMethod(true);
            MethodInfo setter = propInfo.GetSetMethod(true);

            MethodInfo mainAccessor = getter ?? setter;

            if (mainAccessor == null)
            {
                return propInfo.Name;
            }

            string mainAccess = ExtractAccessModifier(mainAccessor);
            if (getter != null && setter != null)
            {
                string getAccess = ExtractAccessModifier(getter);
                string setAccess = ExtractAccessModifier(setter);

                if (getAccess != setAccess)
                {
                    if (getAccess != mainAccess)
                    {
                        return $"{mainAccess} {{ {getAccess} get; {setAccess} set; }}";
                    }

                    return $"{mainAccess} {{ get; {setAccess} set; }}";
                }
            }

            return mainAccess;
        }

    }
}
