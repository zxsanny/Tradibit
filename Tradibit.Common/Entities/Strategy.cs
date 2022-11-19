using System.ComponentModel.DataAnnotations;
using Binance.Net.Enums;
using Tradibit.Common.DTO;

namespace Tradibit.Common.Entities;

public class Strategy
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public List<Step> Steps { get; set; }
}

public class Step
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public bool IsInitial { get; set; }
    
    public List<Transition> Transitions { get; set; }
}

public class Transition
{
    public List<Condition> Conditions { get; set; }
    public List<IOperation> SuccessOperations { get; set; }
    public Guid DestinationStepId { get; set; }

    public async Task<bool> TryTransit(ScenarioState state)
    {
        if (!Conditions.All(c => c.Meet(state)))
            return false;
        
        await Task.WhenAll(SuccessOperations.Select(x => x.Start(state)).ToArray());
        state.CurrentStepId = DestinationStepId;
        return true;
    }
        
}

public interface IOperation
{
    public Task Start(ScenarioState state);
}

public class OrderOperation : IOperation
{
    public OrderSide OrderSide { get; set; }
    public decimal Percent { get; set; }
    
    public async Task Start(ScenarioState state)
    {
        
    }
}

public class Condition
{
    public Operand Operand1 { get; set; }
    public Operand Operand2 { get; set; }
    public OperationEnum Operation { get; set; }

    public bool Meet(ScenarioState state)
    {
        var op1 = Operand1.GetValue(state);
        var op2 = Operand2.GetValue(state);
        if (!op1.HasValue || !op2.HasValue)
            return false;
        
        var result = Operation switch
        {
            OperationEnum.Less => op1 < op2,
            OperationEnum.LessOrEqual => op1 <= op2,
            OperationEnum.More => op1 > op2,
            OperationEnum.MoreOrEqual => op1 >= op2,
            OperationEnum.Equal => op1.Equals(op2),
            OperationEnum.None => false,
            _ => false
        };

        return result;
    }
}

public class Operand
{
    public decimal? NumValue { get; set; }
    public QuoteEnum? Quote { get; set; }
    public IndicatorEnum? Indicator { get; set; }
    public string UserVarName { get; set; }
    
    public Operand(decimal? numValue) => NumValue = numValue;
    public Operand(IndicatorEnum? indicator) => Indicator = indicator;
    public Operand(string userVarName) => UserVarName = userVarName;

    public decimal? GetValue(ScenarioState state)
    {
        if (NumValue.HasValue)
            return NumValue.Value;

        if (Quote.HasValue)
        {
            return Quote switch
            {
                QuoteEnum.Open => state.LastQuote.Open,
                QuoteEnum.High => state.LastQuote.High,
                QuoteEnum.Low => state.LastQuote.Low,
                QuoteEnum.Close => state.LastQuote.Close,
                QuoteEnum.Volume => state.LastQuote.Volume,
                _ => throw new ArgumentException("invalid Quote value")
            };
        }
        
        if (Indicator.HasValue)
            return state.LastIndicators[Indicator.Value];
    
        if (!string.IsNullOrWhiteSpace(UserVarName))
            return state.UserVars[UserVarName];
    
        return null;
    }
    
    public void SetValue(ScenarioState state, decimal? value)
    {
        if (string.IsNullOrEmpty(UserVarName))
            throw new ValidationException("Operand should have User Variable name in order to set it's value");

        state.UserVars[UserVarName] = value;
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