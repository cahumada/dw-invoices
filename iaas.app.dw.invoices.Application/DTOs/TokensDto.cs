namespace iaas.app.dw.invoices.Application.DTOs
{
    public class TokensDto
    {
        public long Id { get; set; }

        public int TypeWebService { get; set; }

        public string ClientId { get; set; }

        public string Sign { get; set; }

        public string Token { get; set; }

        public DateTime Expiration { get; set; }

        public long Cuit { get; set; }
    }
}
