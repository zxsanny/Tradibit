using AutoMapper;
using AutoMapper.Extensions.EnumMapping;
using Binance.Net.Enums;
using Binance.Net.Interfaces;
using Tradibit.Shared.DTO.Primitives;
using SkenderQuote = Skender.Stock.Indicators.Quote;

namespace Tradibit.Shared.MappingProfiles;

public class BinanceProfile : Profile
{
    public BinanceProfile()
    {
        CreateMap<Quote, IBinanceStreamKline>()
            .ForMember(x => x.OpenTime, o => o.MapFrom(s => s.Date))
            .ForMember(x => x.OpenPrice, o => o.MapFrom(s => s.Open))
            .ForMember(x => x.HighPrice, o => o.MapFrom(s => s.High))
            .ForMember(x => x.LowPrice, o => o.MapFrom(s => s.Low))
            .ForMember(x => x.ClosePrice, o => o.MapFrom(s => s.Close))
            .ForMember(x => x.Volume, o => o.MapFrom(s => s.Volume))
            .ReverseMap();
        
        CreateMap<Quote, IBinanceKline>()
            .ForMember(x => x.OpenTime, o => o.MapFrom(s => s.Date))
            .ForMember(x => x.OpenPrice, o => o.MapFrom(s => s.Open))
            .ForMember(x => x.HighPrice, o => o.MapFrom(s => s.High))
            .ForMember(x => x.LowPrice, o => o.MapFrom(s => s.Low))
            .ForMember(x => x.ClosePrice, o => o.MapFrom(s => s.Close))
            .ForMember(x => x.Volume, o => o.MapFrom(s => s.Volume))
            .ReverseMap();

        CreateMap<Quote, SkenderQuote>().ReverseMap();
    }
}
