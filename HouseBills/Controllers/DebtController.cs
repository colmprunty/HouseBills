using System;
using System.Linq;
using System.Web.Mvc;
using HouseBills.Entities;
using HouseBills.Models;
using NHibernate.Linq;

namespace HouseBills.Controllers
{
    public class DebtController : ControllerBase
    {
        public ActionResult Index()
        {
            var model = Session["Model"];
            return View(model);
        }

        public ActionResult CreateDebt()
        {
            return View();
        }

        [HttpPost]
        public ActionResult ReceiveDebt(int debtId)
        {
            var factory = CreateSessionFactory();
            using (var session = factory.OpenSession())
            {
                var debt = (from d in session.Query<Debt>() where d.Id == debtId select d).Single();
                debt.Paid = true;
                session.Save(debt);
                session.Flush();
                var person = (from p in session.Query<Tenant>() where p.Id == debt.Person.Id select p).Single();
                var model = person.GetDebtModel();

                return View("Index", model);
            }
        }

        [HttpPost]
        public ActionResult CreateDebt(DebtModel model)
        {
            var factory = CreateSessionFactory();
            using (var session = factory.OpenSession())
            {
                var debtOwner = (from p in session.Query<Tenant>() where p.Name == User.Identity.Name select p).First();
                var debtors = (from p in session.Query<Tenant>() where p.Id != debtOwner.Id select p);

                foreach (var debtor in debtors)
                {
                    var debt = new Debt
                    {
                        Amount = model.Amount / 4,
                        Description = model.Description,
                        Debtor = debtor,
                        Paid = false,
                        Person = debtOwner,
                        CreatedDate = DateTime.Now
                    };

                    session.Save(debt);
                    debtOwner.AddDebt(debt);
                    session.Save(debtor);
                }

                session.Flush();

                var userModel = debtOwner.GetDebtModel();
                userModel.DebtsIOweToPeople = (from d in session.Query<Debt>() where d.Debtor.Id == debtOwner.Id && !d.Paid select d).ToList();

                return View("Index", userModel);
            }
        }

        
    }

}
