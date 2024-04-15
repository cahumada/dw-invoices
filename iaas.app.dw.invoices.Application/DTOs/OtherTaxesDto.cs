namespace iaas.app.dw.invoices.Application.DTOs
{
    /// <summary>
    /// Otros impuestos
    /// </summary>
    public class OtherTaxesDto
    {
        /// <summary>
        /// Código asociado al impuesto
        /// </summary>
        public short Id { get; set; }

        /// <summary>
        /// Descripción del impuesto
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Base imponible sobre la que se aplica el impuesto
        /// </summary>
        public decimal BaseAmount { get; set; }

        /// <summary>
        /// Alicuota aplicada
        /// </summary>
        public decimal Aliquot { get; set; }

        /// <summary>
        /// Monto con impuesto aplicado
        /// </summary>
        public decimal Amount { get; set; }
    }
}
