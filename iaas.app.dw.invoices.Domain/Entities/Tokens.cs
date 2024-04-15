namespace iaas.app.dw.invoices.Domain.Entities
{
    public class Tokens
    {
        public long Id { get; set; }

        public int TypeWebService { get; set; }

        public string ClientId { get; set; }

        public string Sign { get; set; }

        public string Token { get; set; }

        public DateTime Expiration { get; set; }

        public long Cuit { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreateDate { get; set; }

        public string CreateUser { get; set; }

        public DateTime? UpdateDate { get; set; }

        public string UpdateUser { get; set; }
    }
}
