﻿//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Radus_ConSec
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class RDS_ConSecEntities1 : DbContext
    {
        public RDS_ConSecEntities1()
            : base("name=RDS_ConSecEntities1")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<ADMIN> ADMIN { get; set; }
        public virtual DbSet<EMPLOYEE> EMPLOYEE { get; set; }
        public virtual DbSet<PATROL> PATROL { get; set; }
        public virtual DbSet<ROUTE> ROUTE { get; set; }
        public virtual DbSet<PATROLITEM> PATROLITEM { get; set; }
        public virtual DbSet<EMPLOYEEPATROL> EMPLOYEEPATROL { get; set; }
        public virtual DbSet<MOVEMENTS> MOVEMENTS { get; set; }
    }
}