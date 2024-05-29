using Microsoft.Extensions.DependencyInjection;
using PersonRegistrationSystem.BusinessLogic.Interfaces;
using PersonRegistrationSystem.BusinessLogic.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonRegistrationSystem.BusinessLogic
{
    public static class BusinessLogicServiceExtensions
    {
        public static IServiceCollection AddBusinessLogicServices(this IServiceCollection services)
        {
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IPersonService, PersonService>();
            services.AddScoped<IPlaceOfResidenceService, PlaceOfResidenceService>();
            //services.AddScoped<IFileManager>(provider =>
            //{
            //    var logger = provider.GetRequiredService<ILogger<FileManager>>();
            //    var basePath = Path.Combine(Directory.GetCurrentDirectory(), "images");
            //    return new FileManager(basePath, logger);
            //});
            services.AddScoped<ITokenService, TokenService>();
            return services;
        }
    }
}
