namespace iaas.app.dw.invoices.Application.DTOs
{
    public class DescriptionDto
    {
        public DescriptionDto()
        {

        }

        public DescriptionDto(int id, string description)
        {
            Id = id;
            Description = description;
        }

        public int Id { get; set; }

        public string Description { get; set; }
    }
}
