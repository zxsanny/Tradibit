namespace Tradibit.Api.Scenarios;

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
        if (result)
        {
            foreach (var meetOperation in MeetOperations)
                meetOperation.ApplyOperation(state);   
        }
        
        return result;
    }

    private void ApplyOperation(ScenarioState state)
    {
        Operand1.SetValue(state, Operand2.GetValue(state));
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