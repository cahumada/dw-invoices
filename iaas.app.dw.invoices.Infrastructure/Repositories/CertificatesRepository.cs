using Dapper;
using iaas.app.dw.invoices.Domain.Entities;
using iaas.app.dw.invoices.Domain.Repositories;
using iaas.app.dw.invoices.Infrastructure.Querys;
using iaas.app.dw.invoices.Infrastructure.Support;
using iaas.app.visualtime.process.Infrastructure.Repositories;
using System.Data;

namespace iaas.app.dw.invoices.Infrastructure.Repositories
{
    public class CertificatesRepository : BaseRepository, ICertificatesRepository
    {
        public CertificatesRepository(DapperContext context) : base(context)
        {
        }

        public async Task<Certificates> Get(string clientId)
        {
            try
            {
                Certificates result;

                var sql = CertificatesQuery.GetCertificate(clientId);

                using (var conn = _context.CreateConnection())
                {
                    result = await conn.QueryFirstOrDefaultAsync<Certificates>(sql);

                    return result;
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

        public async Task<int> InsCertificate(Certificates certificate, string userId)
        {
            var parameters = new DynamicParameters();

            try
            {
                string sql = CertificatesQuery.InsCertificate();

                parameters.Add("@ClientId", certificate.ClientId);
                parameters.Add("@FilePfx", certificate.FilePfx);
                parameters.Add("@CertificateKey", certificate.CertificateKey);
                parameters.Add("@Cuit", certificate.Cuit, DbType.Int64);
                parameters.Add("@ExpirationMinutes", certificate.ExpirationMinutes, DbType.Int32);
                parameters.Add("@UserId", userId);

                using (var conn = _context.CreateConnection())
                {
                    var result = await conn.ExecuteAsync(sql, parameters, commandType: CommandType.StoredProcedure);

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
