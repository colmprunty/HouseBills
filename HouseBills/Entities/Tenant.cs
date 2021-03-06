﻿namespace HouseBills.Entities
{
    public class Tenant
    {
        protected Tenant(){}

        public Tenant(string name)
        {
            Name = name;
        }

        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual int Instance { get; set; }
        public virtual bool Archived { get; set; }
    }
}