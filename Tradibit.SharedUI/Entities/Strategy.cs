using System.ComponentModel.DataAnnotations;
using System.Security.AccessControl;
using Binance.Net.Enums;
using Tradibit.Common.DTO;
using Tradibit.Common.Interfaces;
using Tradibit.SharedUI.DTO;
using Tradibit.SharedUI.Primitives;

namespace Tradibit.Common.Entities;

public class Strategy : BaseTrackableId
{
    public string Name { get; set; }
    public string ImageUrl { get; set; }
    public List<Step> Steps { get; set; }
    public bool IsPublic { get; set; }
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
    public string Name { get; set; }
    public List<Condition> Conditions { get; set; }
    public List<IOperation> SuccessOperations { get; set; }
    public Guid DestinationStepId { get; set; }

    public async Task<bool> TryTransit(Scenario scenario, QuoteIndicator quoteIndicator)
    {
        if (!Conditions.All(c => c.Meet(scenario.State, quoteIndicator)))
            return false;
        
        await Task.WhenAll(SuccessOperations.Select(x => x.Start(scenario)).ToArray());
        scenario.State.CurrentStepId = DestinationStepId;
        return true;
    }
        
}

public interface IOperation
{
    public Task Start(Scenario state);
}

public class OrderOperation : IOperation
{
    public OrderSide OrderSide { get; set; }
    public decimal Percent { get; set; }
    
    public async Task Start(Scenario scenario)
    {
        
    }
}

public class Condition
{
    public Operand Operand1 { get; set; }
    public Operand Operand2 { get; set; }
    public OperationEnum Operation { get; set; }

    public bool Meet(ScenarioState state, QuoteIndicator quoteIndicator)
    {
        var op1 = Operand1.GetValue(state, quoteIndicator);
        var op2 = Operand2.GetValue(state, quoteIndicator);
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

    public decimal? GetValue(ScenarioState state, QuoteIndicator quoteIndicator)
    {
        if (NumValue.HasValue)
            return NumValue.Value;

        if (Quote.HasValue)
        {
            return Quote switch
            {
                QuoteEnum.Open => quoteIndicator.Quote.Open,
                QuoteEnum.High => quoteIndicator.Quote.High,
                QuoteEnum.Low => quoteIndicator.Quote.Low,
                QuoteEnum.Close => quoteIndicator.Quote.Close,
                QuoteEnum.Volume => quoteIndicator.Quote.Volume,
                _ => throw new ArgumentException("invalid Quote value")
            };
        }
        
        if (Indicator.HasValue)
            return quoteIndicator.Indicators[Indicator.Value];
    
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