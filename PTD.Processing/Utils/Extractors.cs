using System;
using System.Text;
using System.Linq;
using System.Reflection;

namespace PTD.Processing.Utils
{
    internal static class Extractors
    {
        internal static class Object
        {
            public static Type? ExtractBaseClass(Type? cls)
            {
                return cls?.BaseType;
            }

            public static Type?[] ExtractInheritedInterfaces(Type? cls)
            {
                return cls?.GetInterfaces().Except(cls.BaseType?.GetInterfaces() ?? Type.EmptyTypes).ToArray() ?? Array.Empty<Type>();
            }

            public static string ExtractObjectType(Type? type)
            {
                Type? t = type;
                if (t == null)
                {
                    return string.Empty;
                }

                if (t.IsInterface)
                {
                    return "interface";
                }
                else if (t.IsEnum)
                {
                    return "enum";
                }
                else if (t.IsValueType)
                {
                    return "struct";
                }
                else if (t.IsClass)
                {
                    return "class";
                }

                return string.Empty;
            }
        }

        internal static class Common
        {
            public static string ExtractAccessModifiers(Type? cls)
            {
                Type? type = cls;
                if (type == null)
                {
                    return string.Empty;
                }

                StringBuilder sb = new StringBuilder();

                // --- Access Modifiers ---
                if (type.IsNestedPublic)
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
                else if (type.IsNestedFamORAssem)
                {
                    sb.Append("protected internal ");
                }
                else if (type.IsNestedFamANDAssem)
                {
                    sb.Append("private protected ");
                }
                else if (type.IsNestedAssembly)
                {
                    sb.Append("internal ");
                }
                else if (type.IsPublic)
                {
                    sb.Append("public ");
                }
                else if (type.IsNotPublic)
                {
                    sb.Append("internal ");
                }

                if (type.IsClass)
                {
                    if (type.IsAbstract && type.IsSealed)
                    {
                        sb.Append("static ");
                    }
                    else if (type.IsAbstract && !type.IsInterface)
                    {
                        sb.Append("abstract ");
                    }
                    else if (type.IsSealed && !type.IsValueType && !type.IsEnum)
                    {
                        sb.Append("sealed ");
                    }
                }

                return sb.ToString().TrimEnd();
            }

            public static string ExtractAccessModifiers(MethodInfo? methodInfo)
            {
                MethodInfo? type = methodInfo;
                if (type == null)
                {
                    return string.Empty;
                }

                StringBuilder sb = new StringBuilder();

                if (type.IsPublic)
                {
                    sb.Append("public ");
                }
                else if (type.IsPrivate)
                {
                    sb.Append("private ");
                }
                else if (type.IsFamily)
                {
                    sb.Append("protected ");
                }
                else if (type.IsAssembly)
                {
                    sb.Append("internal ");
                }
                else if (type.IsFamilyOrAssembly)
                {
                    sb.Append("protected internal ");
                }
                else if (type.IsFamilyAndAssembly)
                {
                    sb.Append("private protected ");
                }

                if (type.IsStatic)
                {
                    sb.Append("static ");
                }

                if (type.IsAbstract)
                {
                    sb.Append("abstract ");
                }
                else if (type.IsVirtual && !type.IsFinal)
                {
                    sb.Append("virtual ");
                }

                if (type.IsFinal && type.IsVirtual)
                {
                    sb.Append("sealed override ");
                }

                if (type.IsConstructor)
                {
                    return type.DeclaringType?.Name;
                }

                return sb.ToString().TrimEnd();
            }

            public static string ExtractAccessModifiers(FieldInfo? fieldInfo)
            {
                FieldInfo? type = fieldInfo;
                if (type == null)
                {
                    return string.Empty;
                }

                StringBuilder sb = new StringBuilder();

                // --- Access modifiers ---
                if (type.IsPublic)
                {
                    sb.Append("public ");
                }
                else if (type.IsPrivate)
                {
                    sb.Append("private ");
                }
                else if (type.IsFamily)
                {
                    sb.Append("protected ");
                }
                else if (type.IsAssembly)
                {
                    sb.Append("internal ");
                }
                else if (type.IsFamilyOrAssembly)
                {
                    sb.Append("protected internal ");
                }
                else if (type.IsFamilyAndAssembly)
                {
                    sb.Append("private protected ");
                }

                // --- Other modifiers ---
                if (type.IsStatic)
                {
                    sb.Append("static ");
                }

                if (type.IsLiteral && !type.IsInitOnly)
                {
                    sb.Append("const ");
                }
                else if (type.IsInitOnly)
                {
                    sb.Append("readonly ");
                }

                return sb.ToString().TrimEnd();
            }

            //
            //
            // Probably best to rework this but works fine for now
            //
            //
            public static string[] ExtractAccessModifiers(PropertyInfo? propInfo)
            {
                PropertyInfo? type = propInfo;
                if (type == null)
                {
                    return Array.Empty<string>();
                }

                MethodInfo? getter = type.GetGetMethod(true);
                MethodInfo? setter = type.GetSetMethod(true);

                MethodInfo mainAccessor = getter ?? setter;

                if (mainAccessor == null)
                {
                    return Array.Empty<string>();
                }

                string mainAccess = ExtractAccessModifiers(mainAccessor);
                if (getter != null && setter != null)
                {
                    string getAccess = ExtractAccessModifiers(getter);
                    string setAccess = ExtractAccessModifiers(setter);

                    if (mainAccess == getAccess && mainAccess == setAccess)
                    {
                        return new string[2]
                        {
                            $"{mainAccess}",
                            $"{{ get; set; }}"
                        };
                    }

                    if (getAccess != setAccess)
                    {
                        if (getAccess != mainAccess)
                        {
                            return new string[2]
                            {
                                $"{mainAccess}",
                                $"{{ {getAccess} get; {setAccess} set; }}"
                            };
                        }

                        return new string[2]
                            {
                                $"{mainAccess}",
                                $"{{ get; {setAccess} set; }}"
                            };
                    }
                }

                return new string[1]
                {
                    $"{mainAccess}"
                };
            }
        }

        internal static class Method
        {
            public static string ExtractMethodParams(ParameterInfo[] paramInfo)
            {
                StringBuilder sb = new StringBuilder();

                foreach (var param in paramInfo)
                {
                    if (param.ParameterType.IsByRef)
                    {
                        if (param.IsOut)
                        {
                            sb.Append("out ");
                        }
                        else if (param.IsIn)
                        {
                            sb.Append("in ");
                        }
                        else
                        {
                            sb.Append("ref ");
                        }

                        sb.Append($"{param.ParameterType.Name.TrimEnd('&')} ");
                    }
                    else
                    {
                        sb.Append($"{param.ParameterType.Name} ");
                    }

                    sb.Append(param.Name);

                    if (param.HasDefaultValue)
                    {
                        if (param.DefaultValue == null)
                        {
                            sb.Append(" = null");
                        }
                        else if (param.DefaultValue is string)
                        {
                            sb.Append($" = \"{param.DefaultValue}\"");
                        }
                        else if (param.DefaultValue is bool)
                        {
                            sb.Append($" = {param.DefaultValue.ToString().ToLower()}");
                        }
                        else
                        {
                            sb.Append($" = {param.DefaultValue}");
                        }
                    }

                    sb.Append(", ");
                }

                return sb.ToString().TrimEnd(' ', ',');
            }
        }
    }
}
