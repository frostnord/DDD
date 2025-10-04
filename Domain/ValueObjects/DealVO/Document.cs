using System;
using System.Collections.Generic;
using CSharpFunctionalExtensions;
using Domain.ValueObjects;
using DDD.Domain.ValueObjects;

namespace Domain.ValueObjects
{
    /// <summary>
    /// Объект значения, представляющий документ
    /// </summary>
    public class Document : ValueObject
    {
        /// <summary>
        /// Уникальный идентификатор документа
        /// </summary>
        public Guid Id { get; }
        
        /// <summary>
        /// Название документа
        /// </summary>
        public string Title { get; }
        
        /// <summary>
        /// Тип документа (
        /// </summary>
        public string DocumentType { get; }
        
        /// <summary>
        /// Путь к файлу документа
        /// </summary>
        public string FilePath { get; }
        
        /// <summary>
        /// Дата создания документа
        /// </summary>
        public DateTime CreatedAt { get; }

        /// <summary>
        /// Создает новый экземпляр документа
        /// </summary>
        /// <param name="title">Название документа</param>
        /// <param name="documentType">Тип документа</param>
        /// <param name="filePath">Путь к файлу документа</param>
        private Document(string title, string documentType, string filePath)
        {
            Id = Guid.NewGuid();
            Title = title;
            DocumentType = documentType;
            FilePath = filePath;
            CreatedAt = DateTime.UtcNow;
        }

        /// <summary>
        /// Фабричный метод для создания экземпляра документа с возвратом результата
        /// </summary>
        /// <param name="title">Название документа</param>
        /// <param name="documentType">Тип документа</param>
        /// <param name="filePath">Путь к файлу документа</param>
        /// <returns>Result с экземпляром Document при успешной валидации или ошибкой при провале валидации</returns>
        public static Result<Document> Create(string title, string documentType, string filePath)
        {
            var validationErrors = new List<string>();

            if (string.IsNullOrWhiteSpace(title))
                validationErrors.Add("Название документа не может быть пустым");

            if (string.IsNullOrWhiteSpace(documentType))
                validationErrors.Add("Тип документа не может быть пустым");

            if (string.IsNullOrWhiteSpace(filePath))
                validationErrors.Add("Путь к файлу документа не может быть пустым");

            return validationErrors.Count > 0
                ? Result.Failure<Document>(string.Join("; ", validationErrors))
                : Result.Success(new Document(title, documentType, filePath));
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Id;
            yield return Title ?? string.Empty;
            yield return DocumentType ?? string.Empty;
            yield return FilePath ?? string.Empty;
            yield return CreatedAt;
        }

        public override string ToString()
        {
            return $"{Title} ({DocumentType}) - {FilePath}";
        }
    }
}