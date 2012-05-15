using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using HouseBills.Entities.Maps;
using NHibernate;

namespace HouseBills
{
    public class MvcApplication : HttpApplication
    {
        private static readonly ISessionFactory SessionFactory = CreateSessionFactory();

        public static ISession CurrentSession
        {
            get { return HttpContext.Current.Items["NHibernateSession"] as ISession; }
            set { HttpContext.Current.Items["NHibernateSession"] = value; }
        }

        public MvcApplication()
        {
            BeginRequest += (sender, args) =>
                                {
                                    CurrentSession = SessionFactory.OpenSession();
                                };
            EndRequest += (o, eventArgs) =>
            {
                var session = CurrentSession;
                if (session != null)
                {
                    session.Dispose();
                }
            };
        }

        private static ISessionFactory CreateSessionFactory()
        {
            return Fluently.Configure()
            .Database(MsSqlConfiguration
            .MsSql2008
            .ConnectionString(c => c.FromConnectionStringWithKey("OfflineConnectionString")))
                .Mappings(m => m.FluentMappings
                .AddFromAssemblyOf<TenantMap>())
            .BuildSessionFactory();
        }


        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapRoute(
                "Default", // Route name
                "{controller}/{action}/{id}", // URL with parameters
                new { controller = "Account", action = "LogIn", id = UrlParameter.Optional } // Parameter defaults
            );

        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

        }

        
    }
}