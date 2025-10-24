using System.Reflection;

namespace Domain.Utilities;

/// <summary>
/// Базовое "умное" перечисление
/// </summary>
public abstract class Enumeration<TEnum> : IEquatable<Enumeration<TEnum>>, IComparable<Enumeration<TEnum>>
   where TEnum : Enumeration<TEnum>
{
   private static readonly Dictionary<int, TEnum> _byValue;
   private static readonly Dictionary<string, TEnum> _byName;
   
   static Enumeration()
   {
       var values = typeof(TEnum)
           .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.DeclaredOnly)
           .Where(f => f.FieldType == typeof(TEnum))
           .Select(f => f.GetValue(null))
           .Cast<TEnum>()
           .ToList();

       _byValue = values.ToDictionary(e => e.Value);
       _byName = values.ToDictionary(e => e.Name, StringComparer.OrdinalIgnoreCase);
   }

   /// <summary>
   /// Название перечисления
   /// </summary>
   public string Name { get; }

   /// <summary>
   /// Значение перечисления
   /// </summary>
   public int Value { get; }

   protected Enumeration(int value, string name)
   {
       Value = value;
       Name = name ?? throw new ArgumentNullException(nameof(name));
   }

   /// <summary>
   /// Создание перечисления по значению
   /// </summary>
   /// <param name="value">Значение перечисления</param>
   /// <returns>TEnum - перечисление из семейства</returns>
   /// <exception cref="ArgumentException">Исключение, если перечисления с таким значением не существует</exception>
   public static TEnum FromValue(int value) =>
       _byValue.TryGetValue(value, out var result)
           ? result
           : throw new ArgumentException($"Значение {value} не поддерживается.");

   /// <summary>
   /// Создание перечисления по имени
   /// </summary>
   /// <param name="name">Название перечисления</param>
   /// <returns>TEnum - перечисление из семейства</returns>
   /// <exception cref="ArgumentException">Исключение, если перечисления с таким названием не существует</exception>
   public static TEnum FromName(string name) =>
       _byName.TryGetValue(name, out var result)
           ? result
           : throw new ArgumentException($"Имя {name} не поддерживается.");

   public virtual bool Equals(Enumeration<TEnum> other) =>
       other is not null && Value.Equals(other.Value);

   public override bool Equals(object obj) =>
       obj is Enumeration<TEnum> other && Equals(other);

   public override int GetHashCode() => Value.GetHashCode();
   
   public int CompareTo(Enumeration<TEnum> other) =>
       other is null ? 1 : Value.CompareTo(other.Value);
   
   public static bool operator ==(Enumeration<TEnum> left, Enumeration<TEnum> right) =>
       left?.Equals(right) ?? right is null;

   public static bool operator !=(Enumeration<TEnum> left, Enumeration<TEnum> right) =>
       !(left == right);
}