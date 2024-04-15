using iaas.app.dw.invoices.Domain.Entities;

namespace iaas.app.dw.invoices.Domain.Repositories
{
    public interface ICertificatesRepository
    {
        Task<Certificates> Get(string clientId);

        Task<int> InsCertificate(Certificates certificate, string userId);
    }
}
