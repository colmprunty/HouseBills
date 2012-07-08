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
            var userCount = (from p in NhSession.Query<Tenant>() where p.Name == model.Name select p).Count();
            if (userCount == 1)
            {
                FormsAuthentication.SetAuthCookie(model.Name, false);
                var userModel = CreateUserModel(NhSession, model.Name);
                var people = from p in NhSession.Query<Tenant>() where p.Name != model.Name select p;
                userModel.People = (from person in people select new SelectListItem { Text = person.Name, Value = person.Id.ToString() }).ToList();
                return View("Index", userModel);
            }

            return RedirectToAction("LogIn");
        }

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("LogIn", "Account");
        }
    }
}
