using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using OktaBoiler.Infrastructure;
using OktaBoiler.Internal;
using System.Net.Http.Headers;
using System.Text;

namespace OktaBoiler.Configuration
{
    public static class ConfigurationExtensions
    {
        public static IServiceCollection AddOktaTokenServiceWithInMemoryCache(this IServiceCollection services, HostBuilderContext context, string configurationSection)
        {
            //Config
            services.Configure<OktaSettings>(context.Configuration.GetSection(configurationSection));
            var oktaSettings = services.GetService<IOptionsSnapshot<OktaSettings>>().Value;

            if (oktaSettings.AuthenticationUrl == null) throw new MissingConfigurationException($"The configuration retrieved from section '{configurationSection}' did not include a value for AuthenticationUrl");
            if (oktaSettings.ClientId == null) throw new MissingConfigurationException($"The configuration retrieved from section '{configurationSection}' did not include a value for ClientId");
            if (oktaSettings.ClientSecret == null) throw new MissingConfigurationException($"The configuration retrieved from section '{configurationSection}' did not include a value for ClientSecret");

            //Cache
            services.AddMemoryCache();

            //Actual Service
            services.AddSingleton<IOktaTokenService, OktaTokenService>();

            //HttpClient
            services.AddHttpClient("OktaTokenService", client =>
            {
                client.BaseAddress = new Uri(oktaSettings.AuthenticationUrl);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"{oktaSettings.ClientId}:{oktaSettings.ClientSecret}")));
            });
            return services;
        }
    }
}
