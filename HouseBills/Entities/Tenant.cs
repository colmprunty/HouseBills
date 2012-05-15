using System.Collections.Generic;
using System.Linq;
using HouseBills.Models;

namespace HouseBills.Entities
{
    public class Tenant
    {
        protected Tenant(){}

        public Tenant(string name)
        {
            Name = name;
        }

        private readonly IList<Debt> _debts = new List<Debt>();

        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual IList<Debt> Debts { get { return _debts; } }

        public virtual void AddDebt(Debt debt)
        {
            _debts.Add(debt);
        }

        public virtual UserModel GetDebtModel()
        {
            return new UserModel
                            {
                                DebtsOwedToMe = Debts.Where(x => !x.Paid).Distinct().ToList()
                            };
        }
    }
}