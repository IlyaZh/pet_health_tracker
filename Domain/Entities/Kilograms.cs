namespace ArchieHealthTracker.Entities;

public readonly record struct Kilograms(double Value)
{
 public static Kilograms operator +(Kilograms a, Kilograms b) =>  new(a.Value + b.Value);
 public static Kilograms operator -(Kilograms a , Kilograms b) => new(a.Value - b.Value);

 public static Kilograms From(double value) =>
     value < 0 ? throw new ArgumentException("Weight cannot be negative") : new(value);

 public static Kilograms From(int value) =>
     value < 0 ? throw new ArgumentException("Weight cannot be negative") : new(value);

 public override string ToString() => $"{Value} kg";
 
}