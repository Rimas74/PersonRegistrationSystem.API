using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PersonRegistrationSystem.DataAccess
{
    public static class DataAccessServiceExtensions
    {
        public static IServiceCollection AddDataAccessServices(this IServiceCollection services, IConfiguration configuration) //string connectionString
        {

            services.AddDbContext<PersonRegistrationContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));


            //services.AddScoped<IUserRepository, UserRepository>();
            //services.AddScoped<IPersonRepository, PersonRepository>();
            //services.AddScoped<IPlaceOfResidenceRepository, PlaceOfResidenceRepository>();

            return services;
        }
    }
}
