using FluentNHibernate.Mapping;

namespace HouseBills.Entities.Maps
{
    public class DebtMap : ClassMap<Debt>
    {
        public DebtMap()
        {
            Id(x => x.Id);
            Map(x => x.Amount);
            Map(x => x.Description);
            Map(x => x.Paid);
            Map(x => x.CreatedDate);
            References(x => x.Debtor).Not.LazyLoad();
            References(x => x.Person).Not.LazyLoad();
        }
    }
}