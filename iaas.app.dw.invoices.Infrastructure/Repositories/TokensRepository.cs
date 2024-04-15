using Dapper;
using iaas.app.dw.invoices.Domain.Entities;
using iaas.app.dw.invoices.Domain.Repositories;
using iaas.app.dw.invoices.Infrastructure.Querys;
using iaas.app.dw.invoices.Infrastructure.Support;
using iaas.app.visualtime.process.Infrastructure.Repositories;
using System.Data;

namespace iaas.app.dw.invoices.Infrastructure.Repositories
{
    public class TokensRepository : BaseRepository, ITokensRepository
    {
        public TokensRepository(DapperContext context) : base(context)
        {
        }

        public async Task<Tokens> Get(string clientId, int typeWebService = 1)
        {
            try
            {
                string sql = TokensQuery.GetTokens(clientId, typeWebService);

                using (var conn = _context.CreateConnection())
                {
                    var result = await conn.QueryFirstOrDefaultAsync<Tokens?>(sql);

                    return result;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<Tokens> CreUpdToken(Tokens tokens, string userId)
        {
            var parameters = new DynamicParameters();

            try
            {
                string sql = TokensQuery.InsUpdToken();

                parameters.Add("@Id", tokens.Id, DbType.Int64);
                parameters.Add("@TypeWebService", tokens.TypeWebService, DbType.Int32);
                parameters.Add("@ClientId", tokens.ClientId);
                parameters.Add("@Sign", tokens.Sign);
                parameters.Add("@Token", tokens.Token);
                parameters.Add("@Expiration", tokens.Expiration, DbType.DateTime);
                parameters.Add("@Cuit", tokens.Cuit, DbType.Int64);
                parameters.Add("@UserId", userId);

                using (var conn = _context.CreateConnection())
                {
                    var result = await conn.QueryFirstAsync<Tokens>(sql, parameters, commandType: CommandType.StoredProcedure);

                    return result;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
    }
}
