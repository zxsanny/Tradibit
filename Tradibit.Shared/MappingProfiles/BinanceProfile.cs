using AutoMapper;
using AutoMapper.Extensions.EnumMapping;
using Binance.Net.Enums;
using Binance.Net.Interfaces;
using Tradibit.SharedUI.DTO.Primitives;
using SkenderQuote = Skender.Stock.Indicators.Quote;

namespace Tradibit.Shared.MappingProfiles;

public class BinanceProfile : Profile
{
    public BinanceProfile()
    {
        CreateMap<Interval, KlineInterval>()
            .ConvertUsingEnumMapping(o =>
            {
                o.MapValue(Interval.I_1_SEC, KlineInterval.OneSecond)
                    .MapValue(Interval.I_1_MIN, KlineInterval.OneMinute)
                    .MapValue(Interval.I_3_MIN, KlineInterval.ThreeMinutes)
                    .MapValue(Interval.I_5_MIN, KlineInterval.FiveMinutes)
                    .MapValue(Interval.I_15_MIN, KlineInterval.FifteenMinutes)
                    .MapValue(Interval.I_30_MIN, KlineInterval.ThirtyMinutes)
                    .MapValue(Interval.I_1_HOUR, KlineInterval.OneHour)
                    .MapValue(Interval.I_2_HOUR, KlineInterval.TwoHour)
                    .MapValue(Interval.I_4_HOUR, KlineInterval.FourHour)
                    .MapValue(Interval.I_6_HOUR, KlineInterval.SixHour)
                    .MapValue(Interval.I_8_HOUR, KlineInterval.EightHour)
                    .MapValue(Interval.I_12_HOUR, KlineInterval.TwelveHour)
                    .MapValue(Interval.I_1_DAY, KlineInterval.OneDay)
                    .MapValue(Interval.I_3_DAY, KlineInterval.ThreeDay)
                    .MapValue(Interval.I_1_WEEK, KlineInterval.OneWeek)
                    .MapValue(Interval.I_1_MONTH, KlineInterval.OneMonth);
            }).ReverseMap();

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
