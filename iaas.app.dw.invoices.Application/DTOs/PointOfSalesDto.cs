namespace iaas.app.dw.invoices.Application.DTOs
{
    public class PointOfSalesDto
    {
        public int Number { get; set; }
        public string EmissionType { get; set; }
        public bool IsBlocked { get; set; }
        public string DeleteDate { get; set; }
    }
}
