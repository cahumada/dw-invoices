namespace iaas.app.dw.invoices.Application.DTOs
{
    /// <summary>
    /// Impuesto al valor agregado
    /// </summary>
    public class TaxesDto
    {
        /// <summary>
        /// Código asociado al IVA
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Base imponible sobre la que se aplica el impuesto
        /// </summary>
        public decimal BaseAmount { get; set; }

        /// <summary>
        /// Monto con impuesto aplicado
        /// </summary>
        public decimal Amount { get; set; }
    }
}
