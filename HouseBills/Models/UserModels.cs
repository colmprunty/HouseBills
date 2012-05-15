using System.Collections.Generic;
using HouseBills.Entities;

namespace HouseBills.Models
{
    public class UserModels
    {
    }

    public class LoginModel
    {
        public string Name { get; set; }
    }

    public class UserModel
    {
        public UserModel()
        {
            DebtsOwedToMe = new List<Debt>();
            DebtsIOweToPeople = new List<Debt>();
        }

        public List<Debt> DebtsOwedToMe { get; set; } 
        public List<Debt> DebtsIOweToPeople { get; set; }
    }
}