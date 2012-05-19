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
                    FormsAuthentication.SetAuthCookie(model.Name, false);
                    var userModel = CreateUserModel(session, model.Name);
                    return View("Index", userModel);
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
