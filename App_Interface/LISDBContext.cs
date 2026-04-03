using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;
using System.Reflection;
using System.Threading.Tasks;
using System.Data.Entity.Infrastructure;
using System.Data.Common;
using System.Data.SqlClient;

namespace lissystem.App_Interface
{
    public class LISDBContext : DbContext
    {
        public LISDBContext(string nameOrConnectionString) : base(nameOrConnectionString)
        {
            Database.SetInitializer<LISDBContext>(null);
            Database.CommandTimeout = 0;
            Configuration.ProxyCreationEnabled = false;
            Configuration.LazyLoadingEnabled = false;
            Configuration.AutoDetectChangesEnabled = false;
            Configuration.ValidateOnSaveEnabled = true;
        }

        /// <summary>
        /// Get server datetime.
        /// </summary>
        /// <returns></returns>
        public DateTime ServerDateTime()
        {
            try
            {
                DateTime currentDateTime = Database.SqlQuery<DateTime>("SELECT GETDATE()").FirstOrDefault();
                return currentDateTime;
            }
            catch {
                return DateTime.Now;
            }
        }

        /// <summary>
        /// Get server datetime (Async).
        /// </summary>
        /// <returns></returns>

        public async Task<DateTime> ServerDateTimeAsync()
        {
            try
            {
                DateTime currentDateTime = await Database.SqlQuery<DateTime>("SELECT GETDATE()").FirstOrDefaultAsync();
                return currentDateTime;
            }
            catch
            {
                return DateTime.Now;
            }
        }

        /// <summary>
        /// Update entity model.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entityModel"></param>
        /// <param name="autoLastMaint"></param>
        public virtual void UpdateEntityModel<T>(T entityModel, bool autoLastMaint = false)
        {
            bool isModified = false;
            var dbEntity = Entry(entityModel);
            foreach (string entProperty in dbEntity.OriginalValues.PropertyNames)
            {
                DbPropertyEntry property = dbEntity.Property(entProperty);
                var originalValue = dbEntity.OriginalValues[entProperty];
                var currentValue = dbEntity.CurrentValues[entProperty];
                if (originalValue != null && !originalValue.Equals(currentValue))
                {
                    property.IsModified = true;
                    isModified = true;
                }
                else if (originalValue == null && currentValue != null)
                    property.IsModified = true;
                else
                    property.IsModified = false;
            }

            if (autoLastMaint && isModified)
            {
                foreach (string entProperty in dbEntity.OriginalValues.PropertyNames)
                {
                    DbPropertyEntry property = dbEntity.Property(entProperty);
                    var originalValue = dbEntity.OriginalValues[entProperty];
                    var currentValue = dbEntity.CurrentValues[entProperty];
                    if (entProperty.ToLower() == "updated_at" || entProperty.ToLower() == "last_maint")
                    {
                        property.CurrentValue = ServerDateTime();
                        property.IsModified = true;
                    }

                    if (entProperty.ToLower() == "last_maint_logon" || entProperty.ToLower() == "updated_by")
                    {
                        if (currentValue == null || string.IsNullOrEmpty(currentValue.ToString()))
                        {
                            if (HttpContext.Current != null && HttpContext.Current.User != null && HttpContext.Current.User.Identity != null && HttpContext.Current.User.Identity.IsAuthenticated)
                            {
                                if (entProperty.ToLower() == "updated_by")
                                    property.CurrentValue = LISRequest.IsAuthenticate ? LISRequest.CurrentAccountId : Guid.Empty;
                                else
                                    property.CurrentValue = LISRequest.CurrentAccountCode;
                                property.IsModified = true;
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Save data changed.
        /// </summary>
        /// <returns></returns>
        public int SaveChangesByTrans()
        {
            int affectedRows = 0;
            using (var dbContextTransaction = this.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
            {
                try
                {
                    affectedRows = SaveChanges();
                    dbContextTransaction.Commit();
                }
                catch (Exception)
                {
                    dbContextTransaction.Rollback();
                    throw;
                }
            }
            return affectedRows;
        }

        /// <summary>
        /// Save data changed (Async).
        /// </summary>
        /// <returns></returns>
        public async Task<int> SaveChangeByTransAsync()
        {
            int affectedRows = 0;
            using (var dbContextTransaction = this.Database.BeginTransaction(System.Data.IsolationLevel.ReadCommitted))
            {
                try
                {
                    affectedRows = await SaveChangesAsync();
                    dbContextTransaction.Commit();
                }
                catch (Exception)
                {
                    dbContextTransaction.Rollback();
                    throw;
                }
            }
            return affectedRows;
        }
    }
}