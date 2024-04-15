namespace iaas.app.dw.invoices.Infrastructure.Querys
{
    internal static class TokensQuery
    {
        internal static string GetTokens(string clientId, int typeWebService) => $"SELECT * FROM TOKENS WHERE CLIENTID = '{clientId}' AND TYPEWEBSERVICE={typeWebService} AND ISNULL(ISDELETED,0) = 0 AND ISNULL(ISACTIVE,0) = 1";

        internal static string InsUpdToken() => "INSUPDTOKEN";
    }
}
