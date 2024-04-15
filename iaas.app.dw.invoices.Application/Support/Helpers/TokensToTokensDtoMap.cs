using AutoMapper;
using iaas.app.dw.invoices.Application.DTOs;
using iaas.app.dw.invoices.Domain.Entities;

namespace iaas.app.dw.invoices.Application.Support.Helpers
{
    public class TokensToTokensDtoMap: Profile
    {
        public TokensToTokensDtoMap()
        {
            CreateMap<Tokens, TokensDto>();
        }
    }
}
