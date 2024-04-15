using iaas.app.dw.invoices.Application.Support.Helpers;

namespace iaas.app.dw.invoices.Application.Support
{
    /// <summary>
    /// 
    /// </summary>
    public class AutoMapperConfig
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public static Type[] RegisterMappings()
        {
            return new[]
            {
                typeof(TokensToTokensDtoMap),
            };
        }
    }
}
