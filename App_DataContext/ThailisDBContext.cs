using lissystem.App_Interface;
using lissystem.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

using lissystem.Models.Thailis;
namespace lissystem.App_DataContext
{
    public class ThailisDBContext : LISDBContext
    {
        public ThailisDBContext() : base("name=RedbowThailis")
        {
            
        }

        public DbSet<ThaiLis_Part> Parts { get; set; }
        public DbSet<ThaiLis_PartToolClass> PartToolClasses { get; set; }
        public DbSet<ThaiLis_SalesOrderDetail> SalesOrderDetails { get; set; }
        public DbSet<ThaiLis_SalesOrderDetailSplit> SalesOrderDetailSplits { get; set; }
        public DbSet<ThaiLis_ShipToCustomer> ShipToCustomers { get; set; }
        public DbSet<ThaiLis_SerialTrack> SerialTracks { get; set; }
        public DbSet<ThaiLis_ToolClass> ToolClasses { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("dbo");

            modelBuilder.Entity<ThaiLis_Part>().ToTable("part");
            modelBuilder.Entity<ThaiLis_PartToolClass>().ToTable("part_tool_class");
            modelBuilder.Entity<ThaiLis_SalesOrderDetail>().ToTable("sales_order_detail");
            modelBuilder.Entity<ThaiLis_SalesOrderDetailSplit>().ToTable("sales_order_detail_split");
            modelBuilder.Entity<ThaiLis_ShipToCustomer>().ToTable("ship_to_customer");
            modelBuilder.Entity<ThaiLis_SerialTrack>().ToTable("serial_track");
            modelBuilder.Entity<ThaiLis_ToolClass>().ToTable("tool_class");

            base.OnModelCreating(modelBuilder);
        }
    }
}