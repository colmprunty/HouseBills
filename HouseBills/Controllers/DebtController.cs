﻿using System;
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
            var debts = (from d in NhSession.Query<Debt>()
                         where 
                                (d.Person.Id == personId && d.Debtor.Id == debtorId) ||
                                (d.Debtor.Id == personId && d.Person.Id == debtorId)
                            select d).ToList();

            foreach (var debt in debts)
            {
                debt.Paid = true;
                NhSession.Save(debt);
            }

            return View("Index", CreateUserModel(NhSession, CurrentUser.Name));
        }

        [HttpPost]
        public ActionResult ReceiveDebt(int debtId)
        {
                var debt = (from d in NhSession.Query<Debt>() where d.Id == debtId select d).Single();
                debt.Paid = true;
                NhSession.Save(debt);
                var model = CreateUserModel(NhSession, CurrentUser.Name);
                return View("Index", model);
            
        }

        public ActionResult CreateDebtForOnePerson()
        {
            var people = from p in NhSession.Query<Tenant>() where p.Name != CurrentUser.Name select p;
            var model = new DebtModel { People = (from person in people select new SelectListItem(){ Text = person.Name, Value = person.Id.ToString()}).ToList() };
            
            return View(model);
        }

        [HttpPost]
        public ActionResult CreateDebtForOnePerson(DebtModel model)
        {
                var debtor = (from p in NhSession.Query<Tenant>() where p.Id == model.PersonId select p).First();

                var debt = new Debt
                               {
                                   Amount = model.Amount,
                                   CreatedDate = DateTime.Now,
                                   Debtor = debtor,
                                   Description = model.Description,
                                   Person = CurrentUser,
                                   Paid = false
                               };

                NhSession.Save(debt);

                var indexModel = CreateUserModel(NhSession, CurrentUser.Name);
                return View("Index", indexModel);
            
        }

        [HttpPost]
        public ActionResult CreateDebt(DebtModel model)
        {
            var debtOwner = (from p in NhSession.Query<Tenant>() where p.Name == CurrentUser.Name select p).First();
            var debtors = (from p in NhSession.Query<Tenant>() where p.Id != debtOwner.Id select p);

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

                NhSession.Save(debt);
                NhSession.Save(debtor);
            }

            var userModel = CreateUserModel(NhSession, CurrentUser.Name);

            return View("Index", userModel);
        }
    }
}
