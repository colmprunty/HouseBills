﻿using System.Linq;
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
        public ISessionFactory CreateSessionFactory()
        {
            return Fluently.Configure()
            .Database(MsSqlConfiguration
            .MsSql2008
            .ConnectionString(c => c.FromConnectionStringWithKey("DefaultConnectionString")))
                .Mappings(m => m.FluentMappings
                .AddFromAssemblyOf<TenantMap>())
            .BuildSessionFactory();
        }

        public UserModel CreateUserModel(ISession session, string name)
        {
            var userModel = new UserModel();
            var user = (from p in session.Query<Tenant>() where p.Name == name select p).First();
            var otherUsers = (from p in session.Query<Tenant>() where p.Name != name select p);
            var allDebts = (from d in session.Query<Debt>() select d);
            userModel.DebtsOwedToMe = (from d in session.Query<Debt>() where d.Person.Id == user.Id && !d.Paid select d).ToList();
            userModel.DebtsIOweToPeople = (from d in session.Query<Debt>() where d.Debtor.Id == user.Id && !d.Paid select d).ToList();

            foreach (var person in otherUsers)
            {
                var iNeedToPay = allDebts.ToList().Where(x => x.Debtor.Id == user.Id && x.Person.Id == person.Id && !x.Paid).Sum(y => y.Amount);
                var owedToMe = allDebts.ToList().Where(x => x.Person.Id == user.Id && x.Debtor.Id == person.Id && !x.Paid).Sum(y => y.Amount);

                userModel.Breakdown.Add(new BreakdownDto { Person = person, Total = owedToMe - iNeedToPay });
            }

            userModel.PersonId = user.Id;
            return userModel;
        }
    }
}
