using Microsoft.AspNet.Mvc.ApiExplorer;
using Microsoft.AspNet.Mvc.ModelBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace TodoApp.ServiceGeneration
{
    public class ServiceGeneration
    {
        internal ServiceGeneration(IApiDescriptionGroupCollectionProvider apiDescriptionProvider)
        {
            if (apiDescriptionProvider == null)
                throw new ArgumentNullException(nameof(apiDescriptionProvider));

            ApiDescriptionProvider = apiDescriptionProvider;
        }

        private IApiDescriptionGroupCollectionProvider ApiDescriptionProvider { get; }

        public string Interfaces { get; private set; }

        public string ServiceClient { get; private set; }

        public void Generate(string relativeInterfacesPath)
        {
            var usedTypes = new HashSet<Type>();

            // Generate service client
            var serviceClient = new StringBuilder();
            serviceClient.AppendLine();
            serviceClient.AppendLine("// This file was generated.");
            serviceClient.AppendLine("// All changes will be lost.");
            serviceClient.AppendLine();
            serviceClient.AppendLine("export default class TodoService {");
            serviceClient.AppendLine("    constructor(private baseAddress: string) {}");

            var apiDescriptions = ApiDescriptionProvider.ApiDescriptionGroups.Items.SelectMany(x => x.Items);
            foreach (var apiDescription in apiDescriptions)
            {
                serviceClient.AppendLine();
                serviceClient.Append($"    public {CamelCase(apiDescription.GroupName)}{apiDescription.ActionDescriptor.Name}(");

                foreach (var parameter in apiDescription.ParameterDescriptions)
                {
                    if (parameter != apiDescription.ParameterDescriptions.First())
                        serviceClient.Append(", ");

                    serviceClient.Append($"{parameter.Name}: {TypeName(parameter.Type)}");
                    usedTypes.Add(parameter.Type);
                }

                if (apiDescription.ResponseType == null)
                    serviceClient.AppendLine("): Promise<void> {");
                else
                {
                    serviceClient.AppendLine($"): Promise<{TypeName(apiDescription.ResponseType)}> {{");
                    usedTypes.Add(apiDescription.ResponseType);
                }

                var bodyParameter = apiDescription.ParameterDescriptions.FirstOrDefault(x => x.Source == BindingSource.Body);

                var relPath = apiDescription.RelativePath;
                serviceClient.AppendLine($"        const relativePath: string = `{relPath.Replace("{", "${")}`;");
                serviceClient.AppendLine($"        return window.fetch(this.baseAddress + relativePath, {{");
                serviceClient.AppendLine($"            method: '{apiDescription.HttpMethod?.ToLower() ?? "get"}',");
                serviceClient.AppendLine($"            headers: {{");
                serviceClient.AppendLine($"                'Content-Type': 'application/json'");
                serviceClient.AppendLine($"            }}{(bodyParameter == null ? "" : ",")}");

                if (bodyParameter != null)
                {
                    serviceClient.AppendLine($"            body: JSON.stringify({bodyParameter.Name})");
                }

                serviceClient.AppendLine("        }).then(response => {");

                if (apiDescription.ResponseType == null)
                {
                    serviceClient.AppendLine("            return;");
                }
                else
                {
                    serviceClient.AppendLine("            return response.json();");
                    usedTypes.Add(apiDescription.ResponseType);
                }

                serviceClient.AppendLine("        });");
                serviceClient.AppendLine("    }");
            }

            serviceClient.AppendLine("}");
            serviceClient.AppendLine();

            // Adjust used types, e.g. instead of IEnumerable<Foo> we actually want Foo, so we know which types to
            // generate interfaces for.
            var adjustedUsedTypes = new HashSet<Type>();
            foreach (var type in usedTypes)
                AdjustTypes(adjustedUsedTypes, type);

            // Add imports for the service client
            var serviceClientImports = new StringBuilder();
            foreach (var type in adjustedUsedTypes)
                serviceClientImports.AppendLine($"import {{ {TypeName(type)} }} from '{relativeInterfacesPath}';");

            // Generate interfaces
            var interfaces = new StringBuilder();
            interfaces.AppendLine("// This file was generated.");
            interfaces.AppendLine("// All changes will be lost.");
            interfaces.AppendLine();
            foreach (var type in adjustedUsedTypes)
            {
                interfaces.AppendLine($"export interface {TypeName(type)} {{");
                foreach (var property in type.GetProperties())
                {
                    interfaces.AppendLine($"    {CamelCase(property.Name)}: {TypeName(property.PropertyType)};");
                }
                interfaces.AppendLine("}");
                interfaces.AppendLine();
            }

            ServiceClient = serviceClientImports.ToString() + serviceClient.ToString();
            Interfaces = interfaces.ToString();
        }

        private static void AdjustTypes(HashSet<Type> adjustedTypes, Type type)
        {
            var typeInfo = type.GetTypeInfo();
            if (typeInfo.IsGenericType && typeInfo.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                AdjustTypes(adjustedTypes, typeInfo.GenericTypeArguments[0]);
            }
            else if (typeInfo.Namespace?.StartsWith("System") ?? false)
            {
                // Do nothing with system types.
            }
            else
            {
                adjustedTypes.Add(type);
            }
        }

        private static string CamelCase(string value)
        {
            if (value == null)
                throw new ArgumentNullException(nameof(value));
            if (value == string.Empty)
                throw new ArgumentException("Value must be at least one character.", nameof(value));

            var result = value.Substring(0, 1).ToLower() + value.Substring(1);
            return result;
        }

        private static string TypeName(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            var typeInfo = type.GetTypeInfo();
            if (typeInfo.IsGenericType && typeInfo.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                return TypeName(typeInfo.GenericTypeArguments[0]) + "[]";
            }

            switch (type.FullName)
            {
                case "System.String":  return "string";
                case "System.Int32":   return "number";
                case "System.Boolean": return "boolean";
                default:               return "I" + type.Name;
            }
        }
    }
}
