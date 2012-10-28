using HouseBills.Entities;

namespace HouseBills.Dto
{
    public class BreakdownDto
    {
        public decimal Total { get; set; }
        public Tenant Person { get; set; }
    }
}