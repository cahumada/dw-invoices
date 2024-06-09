using iaas.app.dw.invoices.Application.Base;

namespace iaas.app.dw.invoices.Application.DTOs
{
    public class InvoicesDto
    {
        /// <summary>
        /// Número de CUIT que emite el comprobante
        /// </summary>
        public long Cuit { get; set; }

        /// <summary>
        /// Tipo de comprobante
        /// </summary>
        public int TypeProof { get; set; }

        /// <summary>
        /// Tipo de documento del cliente
        /// </summary>
        public int TypeClientDocument { get; set; }

        /// <summary>
        /// Documento del cliente
        /// </summary>
        public long ClientDocument { get; set; }

        /// <summary>
        /// Punto de venta
        /// </summary>
        public int PointOfSale { get; set; }

        /// <summary>
        /// Tipo de concepto facturado
        /// </summary>
        public TypeConceptEnum TypeConcept { get; set; }

        /// <summary>
        /// Número de recibo AFIP
        /// </summary>
        public long Receipt { get; set; }

        /// <summary>
        /// Fecha de emisión
        /// </summary>
        public DateTime IssueDate { get; set; }

        /// <summary>
        /// Total bruto del comprobante
        /// </summary>
        public decimal Total { get; set; }

        /// <summary>
        /// Total neto no gravado
        /// </summary>
        public decimal TotalNetNoTax { get; set; }

        /// <summary>
        /// Total exento de impuestos
        /// </summary>
        public decimal TotalExemptTax { get; set; }

        /// <summary>
        /// Total neto del comprobante
        /// </summary>
        public decimal TotalNet { get; set; }

        /// <summary>
        /// Total IVA
        /// </summary>
        public decimal TotalTax { get; set; }

        /// <summary>
        /// Total otros impuestos
        /// </summary>
        public decimal TotalOtherTax { get; set; }

        /// <summary>
        /// Fecha de servicio desde
        /// </summary>
        public DateTime? FromService { get; set; }

        /// <summary>
        /// Fecha de servicio hasta
        /// </summary>
        public DateTime? ToService { get; set; }

        /// <summary>
        /// Vencimiento de pago
        /// </summary>
        public DateTime? ExpirationPay { get; set; }

        /// <summary>
        /// Moneda del comprobante
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// Factor de cambio
        /// </summary>
        public decimal Exchange { get; set; }

        /// <summary>
        /// Detalle de las alicuotas de IVA
        /// </summary>
        public List<TaxesDto> Taxes { get; set; }

        /// <summary>
        /// Detalle de otros impuestos
        /// </summary>
        public List<OtherTaxesDto> OtherTaxes { get; set; }

        /// <summary>
        /// Código de Autorización Eléctronica
        /// </summary>
        public string CAE { get; set; }

        /// <summary>
        /// Fecha de vencimiento del Código de Autorización Eléctronica
        /// </summary>
        public DateTime? ExpirCAE { get; set; }

        /// <summary>
        /// Digito verificador asociado al código de barras
        /// </summary>
        public uint VerifiedDigit { get; set; }

        /// <summary>
        /// Comprobantes asociados
        /// </summary>
        public List<AssociatedInvoicesDto> AssociatedInvoices { get; set; }

    }
}
