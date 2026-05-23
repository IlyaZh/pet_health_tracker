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

    public static Weight FromKilograms(decimal value) => new(value);
    public static Weight FromKilograms(double value) => new((decimal)value);

    public static Weight operator +(Weight a, Weight b) => new(a.Value + b.Value);
    public static Weight operator -(Weight a, Weight b) => new(a.Value - b.Value);

    public static implicit operator double(Weight weight) => (double)weight.Value;
    public static implicit operator decimal(Weight weight) => weight.Value;

    public override string ToString() => $"{Value:F2} kg";
}