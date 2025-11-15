using System;
using System.Reflection;

using PTD.Core.Interfaces;
using PTD.Processing.Utils;

namespace PTD.Processing.Types
{
    public class SanitizedMethod : ISanitizedType
    {
        private readonly MemberInfo? _type;
        private readonly string? _typeName;
        private readonly string? _name;
        private readonly string? _accessModifiers;
        public MemberInfo? Type => _type;
        public string? TypeName => _typeName;
        public string? Name => _name;
        public string? AccessModifiers => _accessModifiers;

        public SanitizedMethod(MethodInfo? method)
        {
            _type = method;
            _typeName = method?.ReturnType.ToString();
            _name = method?.Name;
            _accessModifiers = Extractors.Common.ExtractAccessModifiers(method);
        }

        public override string ToString()
        {
            return $"{_accessModifiers} {_typeName} {_name}({Extractors.Method.ExtractMethodParams((_type as MethodInfo).GetParameters())})";
        }
    }
}
