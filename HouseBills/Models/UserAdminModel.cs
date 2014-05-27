using System.Collections.Generic;
using HouseBills.Entities;

namespace HouseBills.Models
{
    public class UserAdminModel
    {
        public List<Tenant> Tenants { get; set; }
        public string NewUserName { get; set; }
    }
}