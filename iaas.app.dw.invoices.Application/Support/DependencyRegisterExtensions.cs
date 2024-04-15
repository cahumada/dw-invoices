using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using iaas.app.dw.invoices.Application.Services.Interfaces;
using iaas.app.dw.invoices.Application.Services;
using iaas.app.dw.invoices.Application.Base;

namespace iaas.app.dw.invoices.Application.Support
{
    public static class DependencyRegisterExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            RegisterServices(services, configuration);
            return services;
        }

        private static void RegisterServices(IServiceCollection services, IConfiguration configuration)
        {

            services.AddSingleton<MemoryCacher>();

            services.AddScoped(typeof(IInvoicesService), typeof(InvoicesService));

            services.AddAutoMapper(AutoMapperConfig.RegisterMappings());
        }

    }
}
