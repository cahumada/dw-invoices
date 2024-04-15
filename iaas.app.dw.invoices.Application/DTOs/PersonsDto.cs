namespace iaas.app.dw.invoices.Application.DTOs
{
    /// <summary>
    /// Datos de una persona vinculada a AFIP
    /// </summary>
    public class PersonsDto
    {
        /// <summary>
        /// Código de identificación ante la AFIP
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Nombre de la persona fisica
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        /// Apellido de la persona fisica
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        /// Nombre de la razón social de la persona juridica
        /// </summary>
        public string SocialReason { get; set; }

        /// <summary>
        /// Tipo de persona. Valores posible 'FISICA' o 'JURIDICA'
        /// </summary>
        public DescriptionDto TypePerson { get; set; }

        /// <summary>
        /// Tipo de documento de identificación
        /// </summary>
        public DescriptionDto TypeDocument { get; set; }

        /// <summary>
        /// Dirección Fiscal
        /// </summary>
        public string LegalAddress { get; set; }

        /// <summary>
        /// Indicador de estado ante la AFIP
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Tipo de IVA asociado a la persona: 2-MONOTRIBUTO 3-RESP. INSCRIPTO
        /// </summary>
        public DescriptionDto TypeIvaCondition { get; set; }
    }
}
