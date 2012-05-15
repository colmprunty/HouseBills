using System.Web.Mvc;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using HouseBills.Entities.Maps;
using NHibernate;

namespace HouseBills.Controllers
{
    public class ControllerBase : Controller
    {
        public ISessionFactory CreateSessionFactory()
        {
            return Fluently.Configure()
            .Database(MsSqlConfiguration
            .MsSql2008
            .ConnectionString(c => c.FromConnectionStringWithKey("OfflineConnectionString")))
                .Mappings(m => m.FluentMappings
                .AddFromAssemblyOf<TenantMap>())
            .BuildSessionFactory();
        }
    }
}
