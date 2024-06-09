using iaas.app.dw.invoices.Application.Base;
using iaas.app.dw.invoices.Application.DTOs;
using Microsoft.AspNetCore.Http;

namespace iaas.app.dw.invoices.Application.Services.Interfaces
{
    public interface IInvoicesService
    {
        Task<ApiResponseDto<PersonsDto>> GetClientInformation(string clientId, long cuit);

        Task<ApiResponseDto<bool>> SaveCertificate(string clientId, string userId, long cuit, string key, IFormFile file);

        Task<ApiResponseDto<List<DescriptionDto>>> GetCatalog(string clientId, TypeCatalogEnum catalog);

        Task<ApiResponseDto<List<PointOfSalesDto>>> GetPointOfSales(string clientId);

        Task<ApiResponseDto<int>> GetLastInvoice(string clientId, int pointOfSale, int typeProof);

        Task<ApiResponseDto<InvoicesDto>> GenerateInvoice(string clientId, InvoicesDto invoice);
    }
}
