using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using HouseBills.Entities;
using HouseBills.Entities.Maps;
using NHibernate.Tool.hbm2ddl;
using NUnit.Framework;

namespace HouseBills.Controllers
{
    [TestFixture]
    public class DebugController : ControllerBase
    {
        [Test]
        public void CreateDatabase()
        {
            var cfg = Fluently.Configure()
                .Database(MsSqlConfiguration
                    .MsSql2008
                    .ConnectionString(c => c.FromConnectionStringWithKey("OfflineConnectionString")))
                .Mappings(m => m.FluentMappings
                    .AddFromAssemblyOf<TenantMap>());

            cfg.ExposeConfiguration(x => new SchemaExport(x).Execute(true, true, false))
                .BuildConfiguration();
        }

        [Test]
        public void AddSomeone()
        {
            var newbie = new Tenant("Colm");
            NhSession.Save(newbie);
        }
    }
}