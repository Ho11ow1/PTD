using System;
using System.Linq;
using System.Reflection;

using PTD.Core.Interfaces;
using PTD.Processing.Utils;

namespace PTD.Processing.Types
{
    public class SanitizedObject : ISanitizedType
    {
        private readonly MemberInfo? _type;
        private readonly string? _typeName;
        private readonly string? _name;
        private readonly string? _accessModifiers;
        private readonly string? _namespace;
        private readonly Type? _baseClass;
        private readonly Type?[] _inheritedInterfaces;
        public MemberInfo? Type => _type;
        public string? TypeName => _typeName;
        public string? Name => _name;
        public string? AccessModifiers => _accessModifiers;
        public string? Namespace => _namespace;
        public Type? BaseClass => _baseClass;
        public Type?[] InheritedInterfaces => _inheritedInterfaces;

        public SanitizedObject(Type? cls)
        {
            _type = cls;
            _namespace = cls?.Namespace != null ? cls.Namespace : null;
            _typeName = Extractors.Object.ExtractObjectType(cls);
            _baseClass = Extractors.Object.ExtractBaseClass(cls);
            _inheritedInterfaces = Extractors.Object.ExtractInheritedInterfaces(cls);
            _name = cls?.Name;
            _accessModifiers = Extractors.Common.ExtractAccessModifiers(cls);
        }

        public override string ToString()
        {
            bool hasBaseClass = _baseClass != null && _baseClass is not object;
            bool hasInterfaces = _inheritedInterfaces.Length > 0;
            string inheritance = string.Empty;

            if (hasBaseClass)
            {
                inheritance = $": {_baseClass.Name}";
            }
            else if (hasInterfaces)
            {
                inheritance = ": ";
            }

            if (hasInterfaces)
            {
                inheritance += hasBaseClass ? $", {GetInheritedInterfacesAsList()}" : GetInheritedInterfacesAsList();
            }

            return $"{_accessModifiers} {_typeName} {_name} {inheritance}";
        }

        private string GetInheritedInterfacesAsList()
        {
            return string.Join(", ", 
                _inheritedInterfaces.Where(t => t != null)
                                    .Select(t => t!.Name));
        }


    }
}
