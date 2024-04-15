namespace iaas.app.dw.invoices.Application.DTOs
{
    public class CertificatesDto
    {
        public long Id { get; set; }

        public long ClientId { get; set; }

        public string Pfx { get; set; }

        public string Key { get; set; }

        public long Cuit { get; set; }

        public int? ExpirationMinutes { get; set; }
    }
}
