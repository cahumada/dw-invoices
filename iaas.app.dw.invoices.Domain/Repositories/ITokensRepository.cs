using iaas.app.dw.invoices.Domain.Entities;

namespace iaas.app.dw.invoices.Domain.Repositories
{
    public interface ITokensRepository
    {
        Task<Tokens> Get(string clientId, int typeWebService = 1);

        Task<Tokens> CreUpdToken(Tokens tokens, string userId);
    }
}
