using Diga.Domain.Service.DataContracts;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Domain = Diga.Domain;
using Svc = Diga.Domain.Service.DataContracts;

namespace Diga.Domain.Service
{
    public static class Converter
    {
        private static object Convert(object obj, string sourceAssemblyName, string sourceBaseNamespace, string targetTypeFormat)
        {
            if (obj == null) {
                return null;
            }
            var objType = obj.GetType();
            if (objType.Assembly.FullName != sourceAssemblyName) {
                return obj;
            }

            string typeName = objType.FullName.Substring(sourceBaseNamespace.Length + 1);
            var fullTypeName = string.Format(targetTypeFormat, typeName);
            Type type = Type.GetType(fullTypeName);
            var result = Activator.CreateInstance(type);

            // TODO ignore indexer
            foreach (var property in obj.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public)) {
                var domainObject = property.GetMethod.Invoke(obj, new object[0]);
                var dataContractObject = Converter.Convert(domainObject, sourceAssemblyName, sourceBaseNamespace, targetTypeFormat);

                var resultProperty = type.GetProperty(property.Name);
                resultProperty.SetMethod.Invoke(result, new[] { dataContractObject });
            }
            return result;
        }

        public static object ConvertFromDomainToService(object obj) {
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
