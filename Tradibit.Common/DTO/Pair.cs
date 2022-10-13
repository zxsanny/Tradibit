namespace Tradibit.Common.DTO;

public class Pair
{
    public static string SEPARATOR = "";

    public Currency BaseCurrency { get; set; }
    public Currency QuoteCurrency { get; set; }
        
    public Pair(string baseCurrency, string quoteCurrency) : 
        this(new Currency(baseCurrency), new Currency(quoteCurrency)) { }

    public Pair(Currency baseCurrency, Currency quotecurrency)
    {
        BaseCurrency = baseCurrency;
        QuoteCurrency = quotecurrency;
    }

    public override string ToString() => $"{BaseCurrency}{SEPARATOR}{QuoteCurrency}";

    public static Pair ParseOrDefault(string input) 
    {
        var strs = input.Split(SEPARATOR);
        if (strs.Length != 2 
            || strs[0].Length < 2 || strs[0].Length > 7
            || strs[1].Length < 2 || strs[1].Length > 7) 
        {
            return new Pair(Currency.BTC, Currency.USDT);
        }
        return new Pair(strs[0], strs[1]);
    }
}

public class Currency : IEquatable<Currency>
{
    private string Value { get; }
    public Currency(string value)
    {
        if (string.IsNullOrEmpty(value)) throw new Exception("Currency name should not be empty!");
        if (value.Length > 8) throw new Exception("Max lenght of currency name is 7 symbols");

        Value = value;
    }

    public override string ToString() => Value;

    public bool Equals(Currency other) =>
        Value == other.Value;

    public override bool Equals(object obj) =>
        Equals((Currency)obj);

    public override int GetHashCode() =>
        Value.GetHashCode();

    public static implicit operator string(Currency currency) => 
        currency.Value;

    public static Currency USDT = new("USDT");
    public static Currency BTC = new("BTC");
    public static Currency ETH = new("ETH");
}