using System;
using System.Reflection;

using PTD.Core.Interfaces;
using PTD.Processing.Utils;

namespace PTD.Processing.Types
{
    public class SanitizedProperty : ISanitizedType
    {
        private readonly MemberInfo? _type;
        private readonly string? _typeName;
        private readonly string? _name;
        private readonly string? _accessModifiers;
        private readonly bool? _useGetSet;
        private readonly string? _getterAndSetter;
        public MemberInfo? Type => _type;
        public string? TypeName => _typeName;
        public string? Name => _name;
        public string? AccessModifiers => _accessModifiers;

        public SanitizedProperty(PropertyInfo? prop)
        {
            _type = prop;
            _typeName = prop?.PropertyType.ToString();
            _name = prop?.Name;

            string[] accessers = Extractors.Common.ExtractAccessModifiers(prop);
            _accessModifiers = accessers[0];
            if (accessers.Length > 1)
            {
                _useGetSet = true;
                _getterAndSetter = accessers[1];
            }
        }

        public override string ToString()
        {
            if (_useGetSet.HasValue && _useGetSet.Value == true)
            {
                return $"{_accessModifiers} {_typeName} {_name} {_getterAndSetter}";
            }
            else
            {
                return $"{_accessModifiers} {_typeName} {_name}";
            }
        }
    }
}
