namespace ArchieHealthTracker.Domain.Entities;

public readonly record struct Weight
{
    private Weight(decimal value)
    {
        if (value < 0) throw new ArgumentException("Вес не может быть отрицательным");
        if (value > 150) throw new ArgumentException("Арчи точно столько весит?");
        Value = value;
    }

    public decimal Value { get; }

    public static Weight FromKilograms(decimal value)
    {
        return new Weight(value);
    }

    public static Weight FromKilograms(double value)
    {
        return new Weight((decimal)value);
    }

    public static Weight operator +(Weight a, Weight b)
    {
        return new Weight(a.Value + b.Value);
    }

    public static Weight operator -(Weight a, Weight b)
    {
        return new Weight(a.Value - b.Value);
    }

    public static implicit operator double(Weight weight)
    {
        return (double)weight.Value;
    }

    public static implicit operator decimal(Weight weight)
    {
        return weight.Value;
    }

    public override string ToString()
    {
        return $"{Value:F2} kg";
    }
}
