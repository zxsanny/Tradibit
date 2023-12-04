using System.Diagnostics;
using Xunit;
using Xunit.Abstractions;

namespace Tradibit.Api.Test;

public class Test
{
    private readonly ITestOutputHelper _testOutputHelper;

    public Test(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    [Theory]
    [InlineData(new []{"sdv","sdvsd","sdvsdvsdfv","sd","ewf","34sdvdfvasdv","sdvs","sdv","sdvsd", "dfgdfdf", "sdfv", "sdvsdvsd", "sdvgbds","sdvsdsa"}, 20)]
    public void TestJoinPerformance(string[] values, int max)
    {
        var tests = 1000000;
        var list = values.ToList();
        var res = PerformanceTest(() => values.ToList().JoinValues(max), tests);
        var res2 = PerformanceTest(() => values.ToList().JoinValues2(max), tests);
        Console.WriteLine($"res1: {res}{Environment.NewLine}res2: {res2}");
    }
    
    static TimeSpan PerformanceTest(Action action, int repeats)
    {
        var sw = new Stopwatch();
        sw.Start();
        for (int i = 0; i < repeats; i++) 
            action();
        sw.Stop();
        return sw.Elapsed;
    }
}

public static class Joiner
{
    public static string JoinValues(this List<string> values, int maxLength)
    {
        string joinedString = string.Join(", ", values);
        if (joinedString.Length > maxLength)
        {
            while (joinedString.Length > maxLength && values.Count > 0)
            {
                values.RemoveAt(values.Count - 1); // Remove the last element from
                joinedString = string.Join(", ", values);
            }
        }
        return joinedString;
    }

    public static string JoinValues2(this List<string>? values, int maxLength)
    {
        if (values == null || !values.Any())
            return "";
        var first = values.FirstOrDefault()!;
        var sum = first.Length;
        if (sum > maxLength)
            return $"{first[..^3]}...";
        var take = 1;
        foreach (var val in values.Skip(1))
        {
            sum += val.Length + 2;
            if (sum > maxLength)
                break;
            take++;
        }
        return string.Join(", ", values.Take(take));
    }
}