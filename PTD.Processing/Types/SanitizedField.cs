using System;
using System.Reflection;

using PTD.Core.Interfaces;
using PTD.Processing.Utils;

namespace PTD.Processing.Types
{
    public class SanitizedField : ISanitizedType
    {
        private readonly MemberInfo? _type;
        private readonly string? _typeName;
        private readonly string? _name;
        private readonly string? _accessModifiers;
        public MemberInfo? Type => _type;
        public string? Name => _name;
        public string? TypeName => _typeName;
        public string? AccessModifiers => _accessModifiers;

        public SanitizedField(FieldInfo? field)
        {
            _type = field;
            _typeName = field?.FieldType.ToString();
            _name = field?.Name;
            _accessModifiers = Extractors.Common.ExtractAccessModifiers(field);
        }

        public override string ToString()
        {
            return $"{_accessModifiers} {_typeName} {_name}";
        }
    }
}
