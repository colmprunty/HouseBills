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
        public ActionResult Index(UserModel model)
        {
            //var model = Session["Model"];
            return View(model);
        }

        public ActionResult CreateDebt()
        {
            return View();
        }

        public ActionResult ConsolidateDebts(int debtorId, int personId)
        {
            var factory = CreateSessionFactory();
            using (var session = factory.OpenSession())
            {
                var user = (from p in session.Query<Tenant>() where p.Id == personId select p).First();
                var debts = (from d in session.Query<Debt>() where 
                                 (d.Person.Id == personId && d.Debtor.Id == debtorId) ||
                                 (d.Debtor.Id == personId && d.Person.Id == debtorId)
                             select d).ToList();
                foreach (var debt in debts)
                {
                    debt.Paid = true;
                    session.Save(debt);
                }

                session.Flush();
                return View("Index", CreateUserModel(session, user.Name ));
            }

            
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
                var model = CreateUserModel(session, person.Name);
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
                    session.Save(debtor);
                }

                session.Flush();

                var userModel = CreateUserModel(session, debtOwner.Name);

                return View("Index", userModel);
            }
        }

        
    }

}
