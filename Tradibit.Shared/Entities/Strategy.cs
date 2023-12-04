using System.ComponentModel.DataAnnotations;
using Tradibit.Shared.DTO;
using Tradibit.Shared.DTO.Primitives;
using Tradibit.SharedUI.DTO;

namespace Tradibit.Shared.Entities;

public class Strategy : BaseTrackableId
{
    public string Name { get; set; }
    public string ImageUrl { get; set; }
    
    public Guid InitialStepId { get; set; }
    
    public bool IsPublic { get; set; }
    
    public List<Step> Steps { get; set; }
    public List<Scenario> Scenarios { get; set; }
    
    public Guid OwnerUserId { get; set; }
    public User OwnerUser { get; set; }
}

public class Step : BaseTrackableId
{
    public string Name { get; set; }
    public ICollection<Transition> Transitions { get; set; }

    public Strategy Strategy { get; set; }
    public Guid StrategyId { get; set; }
}

public class Transition : BaseTrackableId
{
    public string Name { get; set; }
    public Guid DestinationStepId { get; set; }
    
    public Guid StepId { get; set; }
    public Step Step { get; set; }
    
    public ICollection<Condition> Conditions { get; set; }
    public ICollection<BaseOperation> SuccessOperations { get; set; }

    public bool MeetConditions(Scenario scenario, QuoteIndicator quoteIndicator) => 
        Conditions.All(c => c.Meet(scenario, quoteIndicator));
}

public class Condition : BaseTrackableId
{
    public Operand Operand1 { get; set; }
    public Operand Operand2 { get; set; }
    
    public OperationEnum Operation { get; set; }

    public Guid TransitionId { get; set; }
    public Transition Transition { get; set; }
    
    public bool Meet(Scenario scenario, QuoteIndicator quoteIndicator)
    {
        var op1 = Operand1.GetValue(scenario, quoteIndicator);
        var op2 = Operand2.GetValue(scenario, quoteIndicator);
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
    
    public decimal? GetValue(Scenario scenario, QuoteIndicator quoteIndicator)
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
            return scenario.UserVars[UserVarName];
    
        return null;
    }
    
    public void SetValue(Scenario scenario, decimal? value)
    {
        if (string.IsNullOrEmpty(UserVarName))
            throw new ValidationException("Operand should have User Variable name in order to set it's value");

        scenario.UserVars[UserVarName] = value;
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