using Microsoft.Extensions.DependencyInjection;

namespace SaiphIamRolesAnywhere.DI
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddRolesAnywhere(this ServiceCollection services, RolesAnywhereServiceParams serviceParams)
        {
            services.AddTransient<IRolesAnywhereService>(provider => new RolesAnywhereService()
            {
                Params = serviceParams
            });
            return services;
        }
    }
}
