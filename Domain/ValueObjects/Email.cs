using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;
using System.Text.Json.Serialization;

namespace Domain.ValueObjects;

/// <summary>
/// Объект значения, представляющий электронную почту
/// </summary>
/// <summary>
/// Объект значения, представляющий электронную почту в доменной модели
/// </summary>
public class Email : ValueObject
{
    /// <summary>
    /// Максимальная длина email-адреса согласно RFC 5321
    /// </summary>
    public const int MAX_EMAIL_LEANGTH = 100;

    private static readonly Regex EmailRegex = new Regex(
        @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        RegexOptions.Compiled |
        RegexOptions.IgnoreCase);

    /// <summary>
    /// Значение электронной почты
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// Приватный конструктор для создания экземпляра Email
    /// </summary>
    /// <param name="value">Строковое значение email</param>
    [JsonConstructor]
    private Email(string value)
    {
        Value = value.Trim();
    }

    /// <summary>
    /// Фабричный метод для создания экземпляра электронной почты с возвратом результата
    /// </summary>
    /// <param name="value">Электронная почта</param>
    /// <returns>Result с экземпляром Email при успешной валидации или ошибкой при провале валидации</returns>
    public static Result<Email> Create(string value)
    {
        // Проверяем, что значение не является null, пустой строкой или строкой с одними пробелами
        // Это необходимо, чтобы избежать исключения при обработке регулярного выражения
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result.Failure<Email>("Email не может быть пустым");
        }

        var trimmedValue = value.Trim();

        // Проверяем формат email с помощью регулярного выражения
        if (!IsValidEmail(trimmedValue))
        {
            return Result.Failure<Email>("Некорректный формат email");
        }

        return Result.Success(new Email(trimmedValue));
    }

    private static bool IsValidEmail(string email)
    {
        return EmailRegex.IsMatch(email);
    }

    protected override IEnumerable<IComparable> GetEqualityComponents()
    {
        yield return Value.ToLower();
    }

    public override string ToString() => Value;

    public static implicit operator string(Email email) => email.Value;
}