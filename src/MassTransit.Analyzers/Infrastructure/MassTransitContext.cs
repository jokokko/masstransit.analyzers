using System;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace MassTransit.Analyzers.Infrastructure
{
    public sealed class MassTransitContext
    {        
        public readonly Version Version;
        public MassTransitContext(Compilation compilation)
        {                        
            Version = compilation.ReferencedAssemblyNames
                .FirstOrDefault(a => a.Name.Equals(Constants.MassTransitAssembly, StringComparison.OrdinalIgnoreCase))
                ?.Version;            
        }        
    }
}