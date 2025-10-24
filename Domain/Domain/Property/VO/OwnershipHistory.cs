using CSharpFunctionalExtensions;

namespace Domain.Domain.Property.VO
{
    /// <summary>
    /// Объект значения, представляющий историю владения недвижимостью
    /// </summary>
    public class OwnershipHistory : ValueObject
    {
        private readonly List<OwnershipRecord> _records;

        public OwnershipHistory()
        {
            _records = new List<OwnershipRecord>();
        }

        public IReadOnlyList<OwnershipRecord> Records => _records.AsReadOnly();

        private OwnershipHistory(List<OwnershipRecord> records = null)
        {
            _records = records ?? new List<OwnershipRecord>();
        }

        public static Result<OwnershipHistory> Create(List<OwnershipRecord> records = null)
        {
            var history = new OwnershipHistory(records?.ToList() ?? new List<OwnershipRecord>());
            return Result.Success(history);
        }

        public static Result<OwnershipHistory> Create()
        {
            var history = new OwnershipHistory();
            return Result.Success(history);
        }


        public Result AddRecord(OwnershipRecord record)
        {
            // Проверка инвариантов
            if (record.EndDate == null && _records.Any(r => r.EndDate == null))
            {
                return Result.Failure("Нельзя добавить более одного открытого периода владения");
            }

            bool overlaps = _records.Any(existing =>
                PeriodsOverlap(existing.StartDate, existing.EndDate, record.StartDate, record.EndDate));
            if (overlaps)
            {
                return Result.Failure("Новый период владения пересекается с существующей историей");
            }

            _records.Add(record);
            _records.Sort((r1, r2) => r1.StartDate.CompareTo(r2.StartDate));

            return Result.Success();
        }

        public OwnershipRecord GetCurrentOwner()
        {
            if (!_records.Any())
            {
                return null;
            }

            return _records.OrderByDescending(r => r.StartDate).FirstOrDefault();
        }

        private static bool PeriodsOverlap(DateTime s1, DateTime? e1, DateTime s2, DateTime? e2)
        {
            var end1 = e1 ?? DateTime.MaxValue;
            var end2 = e2 ?? DateTime.MaxValue;
            return s1 <= end2 && s2 <= end1;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            foreach (var record in _records)
            {
                yield return record;
            }
        }
    }
}