namespace ArchieHealthTracker.Domain.Entities;

/// <summary>
/// A value object representing a pet's weight with validation.
/// </summary>
public readonly record struct Weight
{
    private Weight(decimal value)
    {
        if (value < 0) throw new ArgumentException("Weight cannot be negative");
        if (value > 150) throw new ArgumentException("Are you sure about this weight?");
        Value = value;
    }

    /// <summary>
    /// The weight value in kilograms.
    /// </summary>
    public decimal Value { get; }

    /// <summary>
    /// Creates a Weight instance from a decimal value.
    /// </summary>
    /// <param name="value">The weight in kilograms.</param>
    public static Weight FromKilograms(decimal value)
    {
        return new Weight(value);
    }

    /// <summary>
    /// Creates a Weight instance from a double value.
    /// </summary>
    /// <param name="value">The weight in kilograms.</param>
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
