using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using HouseBills.Entities;
using HouseBills.Models;
using NHibernate.Linq;

namespace HouseBills.Controllers
{
    public class AccountController : ControllerBase
    {
        public ActionResult LogIn()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LogIn(LoginModel model)
        {
            var factory = CreateSessionFactory();
            using(var session = factory.OpenSession())
            {
                var userCount = (from p in session.Query<Tenant>() where p.Name == model.Name select p).Count();
                if (userCount == 1)
                {
                    var user = (from p in session.Query<Tenant>() where p.Name == model.Name select p).First();
                    FormsAuthentication.SetAuthCookie(model.Name, false);
                    var userModel = user.GetDebtModel();
                    userModel.DebtsIOweToPeople = (from d in session.Query<Debt>() where d.Debtor.Id == user.Id && !d.Paid select d).ToList();

                    Session["Model"] = userModel;
                    return RedirectToAction("Index", "Debt");
                }

                return RedirectToAction("LogIn");
            }
        }

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("LogIn", "Account");
        }
    }
}
