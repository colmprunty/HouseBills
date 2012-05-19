using FluentNHibernate.Mapping;

namespace HouseBills.Entities.Maps
{
    public class TenantMap : ClassMap<Tenant>
    {
        public TenantMap()
        {
            Id(x => x.Id);
            Map(x => x.Name);
        }
    }
}