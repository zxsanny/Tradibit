using System.ComponentModel.DataAnnotations;
using Skender.Stock.Indicators;
using Tradibit.Common.DTO;

namespace Tradibit.Api.Providers;

public class Scenario
{
    private readonly CandlesProviderResolver _candlesProviderResolver;
    
    public Strategy Strategy { get; set; }
    public ScenarioState State { get; set; }
    
    public bool IsActive { get; set; }
    public double DepositPercent { get; set; }

    public Scenario(CandlesProviderResolver candlesProviderResolver)
    {
        _candlesProviderResolver = candlesProviderResolver;
    }
    
    public async Task Start()
    {
        var candlesProvider = _candlesProviderResolver(CandlesResolverEnum.History);
        
    }

    public async Task TestHistory(double deposit, TimeSpan historySpan)
    {
        var candlesProvider = _candlesProviderResolver(CandlesResolverEnum.History)!;
        await candlesProvider.Subscribe(Handler);
    }

    private void Handler(Pair pair, Quote quote, Dictionary<IndicatorEnum, double?> indicators)
    {
        if (Strategy.BuyConditions.All(c => c.Meet(State)))
        {
            
        }
    }
}

public class ScenarioState
{
    public double DepositMoney { get; set; }
    public double PositionMoney { get; set; }
    public Dictionary<string, double?> UserVars { get; set; }
        
    public Pair ActivePair { get; set; }
    public Dictionary<IndicatorEnum, double?> LastQuote { get; set; }
}

public class Strategy
{
    public List<Condition> BuyConditions { get; set; }
    public List<Condition> SellConditions { get; set; }
}
    
public class Condition
{
    public Operand Operand1 { get; set; }
    public Operand Operand2 { get; set; }
    public OperationEnum Operation { get; set; }
    public List<Condition> MeetOperations { get; set; }
    
    public bool Meet(ScenarioState state)
    {
        var op1 = Operand1.GetValue(state);
        var op2 = Operand2.GetValue(state);
        if (!op1.HasValue || !op2.HasValue)
            return false;
        
        return Operation switch
        {
            OperationEnum.Less => op1 < op2,
            OperationEnum.LessOrEqual => op1 <= op2,
            OperationEnum.More => op1 > op2,
            OperationEnum.MoreOrEqual => op1 >= op2,
            OperationEnum.Equal => op1.Equals(op2),
            OperationEnum.None => false,
            _ => false
        };
    }

    public void ApplyOperation(ScenarioState state)
    {
        
    }
}
    
public enum OperationEnum
{
    None,
    Less,
    LessOrEqual,
    More,
    MoreOrEqual,
    Equal
}
    
public class Operand
{
    public double? NumValue { get; set; }
    public IndicatorEnum? Indicator { get; set; }
    public string UserVarName { get; set; }

    public Operand(double? numValue) => NumValue = numValue;
    public Operand(IndicatorEnum? indicator) => Indicator = indicator;
    public Operand(string userVarName) => UserVarName = userVarName;

    public double? GetValue(ScenarioState state)
    {
        if (NumValue.HasValue)
            return NumValue.Value;
    
        if (Indicator.HasValue)
            return state.LastQuote[Indicator.Value];
    
        if (!string.IsNullOrWhiteSpace(UserVarName))
            return state.UserVars[UserVarName];
    
        return null;
    }
    
    public void SetValue(ScenarioState state, double? value)
    {
        if (string.IsNullOrEmpty(UserVarName))
            throw new ValidationException("Operand should have User Variable name in order to set it's value");

        state.UserVars[UserVarName] = value;
    }
}