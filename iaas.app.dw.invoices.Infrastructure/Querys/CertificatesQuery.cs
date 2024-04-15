namespace iaas.app.dw.invoices.Infrastructure.Querys
{
    internal static class CertificatesQuery
    {
        internal static string GetCertificate(string clientId) => $"SELECT * FROM CERTIFICATES WHERE CLIENTID = '{clientId}' AND ISNULL(ISDELETED,0) = 0 AND ISNULL(ISACTIVE,0) = 1";

        internal static string InsCertificate() => "INSCERTIFICATE";
    }
}
