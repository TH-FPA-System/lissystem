using lissystem.App_Interface;
using lissystem.Models;
using lissystem.Models.Authentication;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace lissystem.App_DataContext
{
    public class BaseDBContext : LISDBContext
    {
        public BaseDBContext() : base("name=BaseLISSystemDB")
        {
        }

        public DbSet<inventory_category> InventoryCategories { get; set; }
        public DbSet<inventory> Inventories { get; set; }
        public DbSet<inventory_subcategory> InventorySubCategories { get; set; }
        public DbSet<SysModuleRole> SysModuleRoles { get; set; }
        public DbSet<SysModule> SysModules { get; set; }
        public DbSet<UserAccount> UserAccounts { get; set; }
        public DbSet<UserAccountInformation> UserAccountInformations { get; set; }
        public DbSet<UserAccountRole> UserAccountRoles { get; set; }
        public DbSet<UserAccountSession> UserAccountSessions { get; set; }

        public DbSet<PlannerOrderSplitProdExcept> PlannerOrderSplitProdClassExcepts { get; set; }
        public DbSet<ProductClass> ProductClasses { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("dbo");

            modelBuilder.Entity<inventory_category>().ToTable("lis_inv_categories");
            modelBuilder.Entity<inventory>().ToTable("lis_inv_lists");
            modelBuilder.Entity<inventory_subcategory>().ToTable("lis_inv_subcategories");

            modelBuilder.Entity<PlannerOrderSplitProdExcept>().ToTable("planner_order_split_prod_excepts");
            modelBuilder.Entity<ProductClass>().ToTable("product_class");

            modelBuilder.Entity<SysModuleRole>().ToTable("sys_module_roles");
            modelBuilder.Entity<SysModule>().ToTable("sys_modules");
            modelBuilder.Entity<UserAccount>().ToTable("user_accounts");
            modelBuilder.Entity<UserAccountInformation>().ToTable("user_account_informations");
            modelBuilder.Entity<UserAccountRole>().ToTable("user_account_roles");
            modelBuilder.Entity<UserAccountSession>().ToTable("user_account_sessions");

            base.OnModelCreating(modelBuilder);
        }

    }
}