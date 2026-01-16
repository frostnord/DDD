using System;

namespace Presenter.DTOs.BuyerDTO
{
    public class CreateBuyerRequest
    {
        public Guid ClientId { get; set; }
        public int PreferredNumberOfRooms { get; set; }
        public int PreferredFloor { get; set; }
        public int PreferredTotalFloors { get; set; }
        public string PreferredType { get; set; } = string.Empty;
        public bool? PreferParking { get; set; }
        public string PreferredHeatingType { get; set; } = string.Empty;
        public string PreferredCondition { get; set; } = string.Empty;
    }
}