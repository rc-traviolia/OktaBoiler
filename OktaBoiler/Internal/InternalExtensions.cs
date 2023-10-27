using Microsoft.Extensions.DependencyInjection;

namespace OktaBoiler.Internal
{
    public static class InternalExtensions
    {
        public static TServiceType GetService<TServiceType>(this IServiceCollection services)
        {
            var service = services.BuildServiceProvider().GetService<TServiceType>();
            if (service == null)
            {
                throw new InvalidOperationException($"Service type was not found in the IServicesCollection");
            }

            return service;
        }
    }
}
