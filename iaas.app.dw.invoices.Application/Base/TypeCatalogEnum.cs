namespace iaas.app.dw.invoices.Application.Base
{
    /// <summary>
    /// Tipos de catálogos vinculados a AFIP
    /// </summary>
    public enum TypeCatalogEnum
    {
        /// <summary>
        /// Tipos de conceptos habilitados por AFIP
        /// </summary>
        concepts = 1,

        /// <summary>
        /// Tipos de comprobantes habilitados por AFIP
        /// </summary>
        proofs = 2,

        /// <summary>
        /// Tipos de IVA habilitadas por AFIP
        /// </summary>
        vats = 3,

        /// <summary>
        /// Tipos de documentos habilitados por AFIP
        /// </summary>
        documents = 4,

        /// <summary>
        /// Tipos de monedas habilitadas por AFIP
        /// </summary>
        currencies = 5,

        /// <summary>
        /// Otros tipos de impuestos habilitadas por AFIP
        /// </summary>
        othetTaxes = 6,
    }
}
