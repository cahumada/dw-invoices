using System.Text.Json.Serialization;

namespace iaas.app.dw.invoices.Application.Base
{
    /// <summary>
    /// 
    /// </summary>
    public class PageOptions
    {
        /// <summary>
        /// 
        /// </summary>
        public PageOptions()
        {
            MarginBottom = 10;
            MarginLeft = 3;
            MarginRight = 3;
            MarginTop = 10;
            PageHeight = "279";
            PageWidth = "216";
            HeaderSpacing = 45;
        }

        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName("margin-top")]
        public double? MarginTop { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName("margin-left")]
        public double? MarginLeft { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName("margin-right")]
        public double? MarginRight { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName("margin-bottom")]
        public double? MarginBottom { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName("page-width")]
        public string PageWidth { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName("page-height")]
        public string PageHeight { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonPropertyName("header-spacing")]
        public int? HeaderSpacing { get; set; }
    }
}
