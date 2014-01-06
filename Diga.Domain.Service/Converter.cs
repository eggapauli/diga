using System;
using System.Configuration;
using System.Linq;
using System.Reflection;

namespace Diga.Domain.Service
{
    public static class Converter
    {
        private static object Convert(object obj, string sourceAssemblyName, string sourceBaseNamespace, string targetTypeFormat)
        {
            if (obj == null)
            {
                return null;
            }
            var objType = obj.GetType();
            if (objType.Assembly.FullName != sourceAssemblyName)
            {
                return obj;
            }

            string typeName = objType.FullName.Substring(sourceBaseNamespace.Length + 1);
            var fullTypeName = string.Format(targetTypeFormat, typeName);
            var type = Type.GetType(fullTypeName);
            var result = Activator.CreateInstance(type);

            var properties = from prop in obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)
                             where prop.GetMethod != null && prop.GetMethod.GetParameters().Length == 0 // ignore indexer
                             let resultProp = type.GetProperty(prop.Name)
                             where resultProp.SetMethod != null
                             select new { SourceProperty = prop, TargetProperty = resultProp };
            foreach (var pair in properties)
            {
                var domainObject = pair.SourceProperty.GetMethod.Invoke(obj, new object[0]);
                var dataContractObject = Converter.Convert(domainObject, sourceAssemblyName, sourceBaseNamespace, targetTypeFormat);

                pair.TargetProperty.SetMethod.Invoke(result, new[] { dataContractObject });
            }
            return result;
        }

        public static object ConvertFromDomainToService(object obj)
        {
            return Convert(obj,
                ConfigurationManager.AppSettings["domainAssembly"],
                ConfigurationManager.AppSettings["domainNamespace"],
                ConfigurationManager.AppSettings["dataContractFormat"]);
        }

        public static object ConvertFromServiceToDomain(object obj)
        {
            return Convert(obj,
                ConfigurationManager.AppSettings["serviceAssembly"],
                ConfigurationManager.AppSettings["serviceNamespace"],
                ConfigurationManager.AppSettings["domainFormat"]);
        }
    }
}
