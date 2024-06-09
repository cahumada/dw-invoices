using DinkToPdf;
using DinkToPdf.Contracts;
using HandlebarsDotNet;
using iaas.app.dw.invoices.Application.Base;
using iaas.app.dw.invoices.Application.DTOs;
using iaas.app.dw.invoices.Application.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Text;

namespace iaas.app.dw.invoices.Application.Services
{
    /// <summary>
    /// 
    /// </summary>
    public class PdfServices: IPdfServices
    {
        private readonly IConfiguration _config;
        private readonly IConverter _converter;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="config"></param>
        /// <param name="converter"></param>
        public PdfServices(IConfiguration config, IConverter converter)
        {
            _config = config;
            _converter = converter;
        }

        /// <summary>
        /// Lee de un path configurado un HTML y lo deja guardado en una variable string 
        /// </summary>
        /// <param name="templateReport"></param>
        /// <returns></returns>
        private async Task<TemplateReportDto> ReadFromFile(TemplateReportDto templateReport)
        {
            var folderPath = _config.GetSection("Path:TemplatesPDF").Value;

            var filePath = Environment.CurrentDirectory;
            filePath += folderPath;

            var file = $"{templateReport.FileName}.html";

            if (File.Exists(Path.Combine(filePath, file)))
            {
                templateReport.Html += await File.ReadAllTextAsync(Path.Combine(filePath, file), Encoding.UTF8);
            }

            return templateReport;
        }

        /// <summary>
        /// Remplaza los valores del json por los del template
        /// </summary>
        /// <param name="template"></param>
        /// <param name="reeplacer"></param>
        /// <returns></returns>
        private TemplateReportDto ReeplaceTags(TemplateReportDto template, object reeplacer)
        {

            Handlebars.RegisterHelper("DateFormat", (output, context, data) =>
            {
                DateTime.TryParse(data[0].ToString(), out DateTime date);

                output.WriteSafeString(date.ToString("dd/MM/yyyy"));
            });

            Handlebars.RegisterHelper("DateTimeFormat", (output, context, data) =>
            {
                DateTime.TryParse(data[0].ToString(), out DateTime date);

                output.WriteSafeString(date.ToString("dd/MM/yyyy HH:mm"));
            });

            var templateHtml = Handlebars.Compile(template.Html);

            var templateFinal = templateHtml(reeplacer);
            template.Html = NormalizeTemplate(templateFinal);

            return template;
        }

        /// <summary>
        /// Genera el reporte a partir del html
        /// </summary>
        /// <param name="template"></param>
        /// <param name="reeplacer"></param>
        /// <returns></returns>
        public async Task<TemplateReportDto> GenerateReportFromHtml(TemplateReportDto template, object reeplacer)
        {
            var fileTemplate = await ReadFromFile(template);
            var fileFinal = ReeplaceTags(fileTemplate, reeplacer);

            return fileFinal;
        }

        private string NormalizeTemplate(string template)
        {
            var result = template.Replace("\r\n", string.Empty);
            result = result.Replace("&amp;", "&");
            result = result.Replace("&lt;", "<");
            result = result.Replace("&gt;", ">");
            return result;
        }

        /// <summary>
        /// Genera el PDF y lo convierte a base64
        /// </summary>
        /// <param name="Html"></param>
        /// <param name="pageOptions"></param>
        /// <returns></returns>
        public async Task<FileDto> GeneratePDF(string Html, PageOptions pageOptions)
        {
            var pdf = await GenerateToBytesAsync(Html, pageOptions);
            var file = Convert.ToBase64String(pdf) ?? string.Empty;

            if (pdf != null)
                return new FileDto() { Content = file, ContentLength = file.Length.ToString(), ContentType = "application/pdf" };
            else
                throw new Exception("Error al invocar servicio wkhtmlPDF");
        }

        /// <summary>
        /// Retorna el PDF en byte[]
        /// </summary>
        /// <param name="Html"></param>
        /// <param name="pageOptions"></param>
        /// <returns></returns>
        private async Task<byte[]> GenerateToBytesAsync(string Html, PageOptions pageOptions)
        {
            HtmlToPdfDocument doc = null;
            string fileOutput = Path.Combine(Environment.CurrentDirectory, "Temp", Guid.NewGuid().ToString() + ".pdf");
            try
            {
                doc = new HtmlToPdfDocument()
                {
                    GlobalSettings = {
                        Margins = new MarginSettings() {
                            Left = pageOptions.MarginLeft ?? 10,
                            Right = pageOptions.MarginRight ?? 10,
                            Bottom = pageOptions.MarginBottom ?? 10,
                            Top = pageOptions.MarginTop ?? 10
                        },
                        PaperSize = PaperKind.A4,
                        Orientation = Orientation.Portrait,
                        Out = fileOutput
                    },
                    Objects = {
                        new ObjectSettings() {
                            PagesCount = true,
                            HtmlContent = Html,
                            WebSettings = {  DefaultEncoding = "utf-8", MinimumFontSize = 12 },
                            HeaderSettings = { FontSize = 6, Right = "Página [page] de [toPage]", Line = false, Spacing = 2.812 }
                        }
                    }
                };

                _converter.Convert(doc);

                Byte[] bytes = await File.ReadAllBytesAsync(doc.GlobalSettings.Out);

                if (bytes != null)
                    return bytes;

                throw new Exception("Error al invocar servicio wkhtmlPDF");

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            finally
            {
                if (doc != null)
                    if (File.Exists(fileOutput))
                        File.Delete(fileOutput);
            }
        }
    }
}
