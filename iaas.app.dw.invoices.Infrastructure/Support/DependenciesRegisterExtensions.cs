using iaas.app.dw.invoices.Domain.Repositories;
using iaas.app.dw.invoices.Infrastructure.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace iaas.app.dw.invoices.Infrastructure.Support
{
    public static class DependenciesRegisterExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<DapperContext>();
            services.AddTransient<ICertificatesRepository, CertificatesRepository>();
            services.AddTransient<ITokensRepository, TokensRepository>();
            

            return services;
        }
    }
}
