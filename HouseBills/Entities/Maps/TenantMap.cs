using FluentNHibernate.Mapping;

namespace HouseBills.Entities.Maps
{
    public class TenantMap : ClassMap<Tenant>
    {
        public TenantMap()
        {
            Id(x => x.Id).GeneratedBy.Increment();
            Map(x => x.Name);
            Map(x => x.Instance);
            Map(x => x.Archived);
        }
    }
}