using System;

namespace HouseBills.Entities
{
    public class Debt
    {
        public virtual int Id { get; set; }
        public virtual decimal Amount { get; set; }
        public virtual Tenant Debtor { get; set; }
        public virtual Tenant Person { get; set; }
        public virtual string Description { get; set; }
        public virtual bool Paid { get; set; }
        public virtual DateTime CreatedDate { get; set; }
    }
}