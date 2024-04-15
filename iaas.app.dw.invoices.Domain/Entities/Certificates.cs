namespace iaas.app.dw.invoices.Domain.Entities
{
    public class Certificates
    {
        public long Id { get; set; }

        public string ClientId { get; set; }

        public string FilePfx { get; set; }

        public string CertificateKey { get; set; }

        public long Cuit { get; set; }

        public int? ExpirationMinutes { get; set; }

        public bool IsActive { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime CreateDate { get; set; }

        public string CreateUser { get; set; }

        public DateTime? UpdateDate { get; set; }

        public string UpdateUser { get; set; }
    }
}
