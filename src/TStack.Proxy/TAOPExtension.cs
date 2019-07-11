using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TStack.Proxy.Aspects;
using TStack.Proxy.Models;

namespace TStack.Proxy
{
    public static class TAOPExtension
    {
        public static IServiceCollection AddScopedProxy<TService, TImplementation>(this IServiceCollection services)
           where TService : class
           where TImplementation : class, TService
        {
            services.AddScoped<TService, TImplementation>();
            var service = services.BuildServiceProvider().GetService<TService>();
            services.AddScoped<TService>(serviceProvider => TProxy<TService, TImplementation>.Create((TImplementation)service));
            return services;
        }
    }
    internal static class Extensions
    {
        internal static void AddToExecutionItem(this List<ExecutionItem> executions, ExecutionArgs executionArgs, IAspect aspect)
        {
            executions.Add(new ExecutionItem() { ExecutionArgs = executionArgs, Aspect = aspect });
        }
        internal static ExecutionArgs GetExistsExecutionArgs(this List<ExecutionItem> executions, IAspect aspect)
        {
            if (executions == null)
                return null;
            else
                return executions.FirstOrDefault(x => x.Aspect.GetType().GUID == aspect.GetType().GUID)?.ExecutionArgs;
        }
    }
}
