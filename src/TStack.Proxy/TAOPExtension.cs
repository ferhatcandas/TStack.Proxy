using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

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
}
