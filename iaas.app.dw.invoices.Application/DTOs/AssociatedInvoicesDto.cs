namespace iaas.app.dw.invoices.Application.DTOs
{
    public class AssociatedInvoicesDto
    {
        /// <summary>
        /// Tipo de comprobante
        /// </summary>
        public int TypeProof { get; set; }

        /// <summary>
        /// Punto de venta
        /// </summary>
        public int PointOfSale { get; set; }

        /// <summary>
        /// Número de recibo AFIP
        /// </summary>
        public long Receipt { get; set; }

        /// <summary>
        /// Número de CUIT que emite el comprobante
        /// </summary>
        public long? Cuit { get; set; }

        /// <summary>
        /// Fecha de emisión
        /// </summary>
        public DateTime? IssueDate { get; set; }
    }
}
