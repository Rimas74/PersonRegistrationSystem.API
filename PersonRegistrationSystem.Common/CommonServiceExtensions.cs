using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PersonRegistrationSystem.Common.Validators;


namespace PersonRegistrationSystem.Common
{
    public static class CommonServiceExtensions
    {
        public static IServiceCollection AddCommonServices(this IServiceCollection services)
        {

            services.AddSingleton<ILogger<PersonalCodeValidationAttribute>, Logger<PersonalCodeValidationAttribute>>();

            return services;
        }
    }
}
