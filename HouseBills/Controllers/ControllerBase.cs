using System.Linq;
using System.Web.Mvc;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;
using HouseBills.Dto;
using HouseBills.Entities;
using HouseBills.Entities.Maps;
using HouseBills.Models;
using NHibernate;
using NHibernate.Linq;

namespace HouseBills.Controllers
{
    public class ControllerBase : Controller
    {
        public ISession NhSession { get; set; }
        public Tenant CurrentUser { get; set; }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            NhSession = CreateSessionFactory().OpenSession();
            CurrentUser = User == null ? null : (from p in NhSession.Query<Tenant>() where p.Name == User.Identity.Name select p).SingleOrDefault();
            base.OnActionExecuting(filterContext);
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            NhSession.Flush();
            NhSession.Close();
            base.OnActionExecuted(filterContext);
        }

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

        public UserModel CreateUserModel(ISession session, string name)
        {
            session.Flush();
            var userModel = new UserModel();
            var user = (from p in session.Query<Tenant>() where p.Name == name select p).First();
            var otherUsers = (from p in session.Query<Tenant>() where p.Name != name && p.Instance == user.Instance select p);
            var allDebts = (from d in session.Query<Debt>() select d);
            userModel.DebtsOwedToMe = (from d in session.Query<Debt>() where d.Person.Id == user.Id && !d.Paid select d).ToList();
            userModel.DebtsIOweToPeople = (from d in session.Query<Debt>() where d.Debtor.Id == user.Id && !d.Paid select d).ToList();
            userModel.PaidDebts = (from d in session.Query<Debt>() where d.Person.Id == user.Id && d.Paid select d).ToList();

            foreach (var person in otherUsers)
            {
                var iNeedToPay = allDebts.ToList().Where(x => x.Debtor.Id == user.Id && x.Person.Id == person.Id && !x.Paid).Sum(y => y.Amount);
                var owedToMe = allDebts.ToList().Where(x => x.Person.Id == user.Id && x.Debtor.Id == person.Id && !x.Paid).Sum(y => y.Amount);

                userModel.Breakdown.Add(new BreakdownDto { Person = person, Total = owedToMe - iNeedToPay });
            }

            userModel.PersonId = user.Id;
            userModel.InstanceId = user.Instance;
            userModel.Tenants = (from p in NhSession.Query<Tenant>() where p.Instance == user.Instance && !p.Archived select p).ToList();
            return userModel;
        }
    }
}
