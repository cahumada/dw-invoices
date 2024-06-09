using iaas.app.dw.invoices.Application.Base;
using iaas.app.dw.invoices.Application.DTOs;

namespace iaas.app.dw.invoices.Application.Services.Interfaces
{
    /// <summary>
    /// 
    /// </summary>
    public interface IPdfServices
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="template"></param>
        /// <param name="reeplacer"></param>
        /// <returns></returns>
        Task<TemplateReportDto> GenerateReportFromHtml(TemplateReportDto template, object reeplacer);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Html"></param>
        /// <param name="pageOptions"></param>
        /// <returns></returns>
        Task<FileDto> GeneratePDF(string Html, PageOptions pageOptions);
    }
}
