using Xunit;

namespace Tradibit.Api.Test;

public interface ITwoNumbersFinder
{
    int[] FindNumbers(int[] arr, int sum);
}

// bruteforce, compl: O(n^2), space: O(1)
public class NaiveTwoNumbersFinder : ITwoNumbersFinder
{
    public int[] FindNumbers(int[] arr, int sum)
    {
        for (int i = 0; i < arr.Length; i++)
            for (int j = i + 1; j < arr.Length; j++)
                if (arr[i] + arr[j] == sum)
                    return new[] {arr[i], arr[j]};
        return new int[]{};
    }
}

//hashmap, compl: O(n), space: O(n)
public class HashmapTwoNumbersFinder : ITwoNumbersFinder
{
    public int[] FindNumbers(int[] arr, int sum)
    {
        var hash = new HashSet<int>();
        foreach (var elem in arr)
        {
            var searchElem = sum - elem;
            if (hash.Contains(searchElem))
                return new[] {searchElem, elem};
            hash.Add(elem);
        }

        return new int[] { };
    }
}

//binary search, compl: O(n*log(n)), space: O(1)
public class BinarySearchFinder : ITwoNumbersFinder
{
    public int[] FindNumbers(int[] arr, int sum)
    {
        for (int i = 0; i < arr.Length; i++)
        {
            var searchElem = sum - arr[i];
            var l = i + 1;
            var r = arr.Length - 1;
            while (l <= r)
            {
                var mid = (l + r) / 2;
                if (arr[mid] == searchElem)
                    return new[] {arr[i], searchElem};
                if (searchElem < arr[mid])
                    r = mid - 1;
                else
                    l = mid + 1;
            }
        }

        return new int[] { };
    }
}

//two pointers, compl: O(n), space: O(1)
public class TwoPointersSearchFinder : ITwoNumbersFinder
{
    public int[] FindNumbers(int[] arr, int sum)
    {
        var l = 0;
        var r = arr.Length - 1;
        while (l < r)
        {
            var curSum = arr[l] + arr[r];
            if (curSum == sum)
                return new[] {arr[l], arr[r]};
            
            if (curSum < sum) 
                l++;
            else
                r--;
        }

        return new int[] { };
    }
}

public class InterviewTask
{
    [Theory]
    [InlineData(new[] { -1, 0, 1, 2, 3 }, 5)]
    [InlineData(new[] { -1, 2, 5, 8 }, 7)]
    [InlineData(new[] { -3, -1, 0, 2, 6 }, 6)]
    [InlineData(new[] { 2, 4, 5 }, 8, true)]
    [InlineData(new[] { -2, -1, 1, 2 }, 0)]
    public void FindNumbers(int[] arr, int sum, bool empty = false)
    {
        //act
        //var res = new NaiveTwoNumbersFinder().FindNumbers(arr, sum);
        //var res = new HashmapTwoNumbersFinder().FindNumbers(arr, sum); 
        //var res = new BinarySearchFinder().FindNumbers(arr, sum);
        var res = new TwoPointersSearchFinder().FindNumbers(arr, sum);
        
        //assert
        if (empty)
        {
            Assert.Equal(0, res.Length);
            return;
        }
        
        Assert.Equal(sum, res[0] + res[1]);
        Assert.Contains(res[0], arr);
        Assert.Contains(res[1], arr);
        
    }
}