namespace Tradibit.Shared.DTO.Primitives;

public class Pair : IEquatable<Pair>
{
    private const string Separator = "";

    public readonly Currency BaseCurrency;
    public readonly Currency QuoteCurrency;

    //for ef-core
    public Pair(){}
    
    public Pair(string baseCurrency, string quoteCurrency) : 
        this(new Currency(baseCurrency), new Currency(quoteCurrency)) { }

    public Pair(Currency baseCurrency, Currency quoteCurrency)
    {
        BaseCurrency = baseCurrency;
        QuoteCurrency = quoteCurrency;
    }

    public override string ToString() => $"{BaseCurrency}{Separator}{QuoteCurrency}";

    public static Pair Parse(string input) 
    {
        var strings = input.Split(Separator);
        if (strings.Length != 2 
            || strings[0].Length < 2 || strings[0].Length > 7
            || strings[1].Length < 2 || strings[1].Length > 7)
            throw new Exception("Can't parse pair");
        
        return new Pair(strings[0], strings[1]);
    }
    
    public static bool operator ==(Pair? pair1, Pair? pair2)
    {
        if (pair1 is null)
            return pair2 is null;
        
        return pair1.Equals(pair2);
    }

    public static bool operator !=(Pair pair1, Pair pair2) =>
        !(pair1 == pair2);
    
    public bool Equals(Pair? other)
    {
        if (ReferenceEquals(null, other)) return false;
        if (ReferenceEquals(this, other)) return true;
        return Equals(BaseCurrency, other.BaseCurrency) && Equals(QuoteCurrency, other.QuoteCurrency);
    }

    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        return obj.GetType() == GetType() && Equals((Pair)obj);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(BaseCurrency, QuoteCurrency);
    }

    public static readonly Pair BTC_USDT = new(Currency.BTC, Currency.USDT);
}

public class Currency : IEquatable<Currency>
{
    private string Value { get; }
    
    //for ef-core
    public Currency(){}
    
    public Currency(string value)
    {
        if (string.IsNullOrEmpty(value)) throw new Exception("Currency name should not be empty!");
        if (value.Length > 8) throw new Exception("Max lenght of currency name is 7 symbols");

        Value = value;
    }

    public override string ToString() => Value;

    public static bool operator ==(Currency? currency1, Currency? currency2)
    {
        if (currency1 is null)
            return currency2 is null;
        
        return currency1.Equals(currency2);
    }

    public static bool operator !=(Currency currency1, Currency currency2) =>
        !(currency1 == currency2);

    public bool Equals(Currency? other) =>
        Value != null && Value.Equals(other?.Value);

    public override bool Equals(object? obj) =>
        Equals((Currency?)obj);

    public override int GetHashCode() =>
        Value?.GetHashCode() ?? 0;

    public static implicit operator string(Currency currency) => 
        currency.Value;

    public static readonly Currency USDT = new("USDT");
    public static readonly Currency BTC = new("BTC");
    public static Currency ETH = new("ETH");
}