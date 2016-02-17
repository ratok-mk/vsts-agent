using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;
using Microsoft.VisualStudio.Services.Agent;
using Microsoft.VisualStudio.Services.Agent.Listener;
using Microsoft.VisualStudio.Services.Agent.Worker;

namespace Microsoft.VisualStudio.Services.Agent.Tests
{
    public sealed class ServiceInterfacesL0
    {
        [Fact]
        [Trait("Level", "L0")]
        [Trait("Category", "Agent")]
        public void AgentInterfacesSpecifyDefaultImplementation()
        {
            Validate(typeof(IMessageListener).GetTypeInfo().Assembly);
        }

        [Fact]
        [Trait("Level", "L0")]
        [Trait("Category", "Common")]
        public void CommonInterfacesSpecifyDefaultImplementation()
        {
            Validate(
                typeof(IHostContext).GetTypeInfo().Assembly, // assembly
                typeof(IExecutionContext), // whitelist params
                typeof(IHostContext),
                typeof(ITraceManager));
        }

        [Fact]
        [Trait("Level", "L0")]
        [Trait("Category", "Worker")]
        public void WorkerInterfacesSpecifyDefaultImplementation()
        {
            Validate(
                typeof(IStepRunner).GetTypeInfo().Assembly, // assembly
                typeof(IStep)); // whitelist params
        }

        private static void Validate(Assembly assembly, params Type[] whitelist)
        {
            IDictionary<TypeInfo, Type> w = whitelist.ToDictionary(x => x.GetTypeInfo());
            foreach (TypeInfo typeInfo in assembly.DefinedTypes)
            {
                // Skip non-interfaces and whitelisted types.
                if (!typeInfo.IsInterface || w.ContainsKey(typeInfo)) { continue; }

                // Assert the ServiceLocatorAttribute is defined properly on all non-whitelisted interfaces.
                CustomAttributeData attribute =
                    typeInfo
                    .CustomAttributes
                    .SingleOrDefault(x => x.AttributeType == typeof(ServiceLocatorAttribute));
                Assert.True(attribute != null, $"Missing ServiceLocatorAttribute for interface '{typeInfo.FullName}'. Add the attribute to the interface or whitelist the interface in the test.");
                CustomAttributeNamedArgument defaultArg =
                    attribute
                    .NamedArguments
                    .SingleOrDefault(x => String.Equals(x.MemberName, ServiceLocatorAttribute.DefaultPropertyName, StringComparison.Ordinal));
                Assert.True(defaultArg.TypedValue.Value as Type != null, $"Missing Default parameter on ServiceLocatorAttribute for the interface '{typeInfo.FullName}'.");
            }
        }
    }
}