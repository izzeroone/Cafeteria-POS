using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core;
using System.Linq;
using System.Linq.Expressions;
using Cafocha.Entities;
using Cafocha.Security;

namespace Cafocha.Repository.Generic
{
    public interface IGenericRepository<TEntity> where TEntity : class
    {
        /// <summary>
        ///     Get data
        /// </summary>
        /// <param name="filter">Lambda expression to filtering data</param>
        /// <param name="orderBy">Lambda expression to ordering data</param>
        /// <param name="includeProperties">
        ///     the properties represent the relationship with other entities (use ',' to seperate
        ///     these properties)
        /// </param>
        /// <returns></returns>
        IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "");

        TEntity GetById(object id);
        void Insert(TEntity entity);
        void Delete(object id);
        void Delete(TEntity entityTODelete);
        void Update(TEntity entityToUpdate);

        /// <summary>
        ///     auto generate id for all entities in Asowell Database
        ///     all id type is 10 character and the sign is depend on the type of entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        TEntity AutoGeneteId_DBAsowell(TEntity entity);
    }

    public class GenericRepository<TEntity> : IGenericRepository<TEntity> where TEntity : class
    {
        private const string ENCRYPT_PHASE = "oma_zio_decade";

        private static readonly int ID_SIZE_DBASOWELL = 10;
        internal ILocalContext context;
        internal DbSet dbSet;

        public GenericRepository(ILocalContext context)
        {
            this.context = context;
            dbSet = context.Set<TEntity>();


            if (typeof(TEntity) == typeof(AdminRe))
            {
                foreach (var entity in dbSet)
                {
                    var admin = entity as AdminRe;
                    var decryptPass = AESThenHMAC.SimpleDecryptWithPassword(admin.Pass, ENCRYPT_PHASE);
                    admin.DecryptedPass = decryptPass;
                }
            }
            else
            {
                if (typeof(TEntity) == typeof(Employee))
                    foreach (var entity in dbSet)
                    {
                        var emp = entity as Employee;
                        var decryptPass = AESThenHMAC.SimpleDecryptWithPassword(emp.Pass, ENCRYPT_PHASE);
//                        string decryptCode = AESThenHMAC.SimpleDecryptWithPassword(emp.EmpCode, ENCRYPT_PHASE);
//                        string decryptPass = emp.Pass;
                        var decryptCode = emp.EmpCode;
                        emp.DecryptedPass = decryptPass;
                        emp.DecryptedCode = decryptCode;
                    }
            }
        }


        /// <summary>
        ///     Get data
        /// </summary>
        /// <param name="filter">Lambda expression to filtering data</param>
        /// <param name="orderBy">Lambda expression to ordering data</param>
        /// <param name="includeProperties">
        ///     the properties represent the relationship with other entities (use ',' to seperate
        ///     these properties)
        /// </param>
        /// <returns></returns>
        public virtual IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "")
        {
            try
            {
                // Apply the filter expression
                var query = (IQueryable<TEntity>) dbSet;


                if (filter != null) query = query.Where(filter);


                // Loading related data (using eager-loading)
                foreach (var includeProperty in includeProperties.Split(
                    new[] {','}, StringSplitOptions.RemoveEmptyEntries))
                    query = query.Include(includeProperty);

                // Apply the orderBy expression
                if (orderBy != null) return orderBy(query).ToList();


                return query.ToList();
            }
            catch (EntityCommandExecutionException ex)
            {
                return Get(filter, orderBy, includeProperties);
            }
        }

        public virtual TEntity GetById(object id)
        {
            try
            {
                return (TEntity) dbSet.Find(id);
            }
            catch (EntityCommandExecutionException ex)
            {
                return GetById(id);
            }
        }

        public virtual void Insert(TEntity entity)
        {
            try
            {
                if (typeof(TEntity) == typeof(AdminRe))
                {
                    var admin = entity as AdminRe;
                    var encryptPass = AESThenHMAC.SimpleEncryptWithPassword(admin.Pass, ENCRYPT_PHASE);
                    admin.Pass = encryptPass;
                }
                else
                {
                    if (typeof(TEntity) == typeof(Employee))
                    {
                        var emp = entity as Employee;
                        var encryptPass = AESThenHMAC.SimpleEncryptWithPassword(emp.Pass, ENCRYPT_PHASE);
                        var encryptCode = AESThenHMAC.SimpleEncryptWithPassword(emp.EmpCode, ENCRYPT_PHASE);
                        emp.Pass = encryptPass;
                        emp.EmpCode = encryptCode;
                    }
                }


                dbSet.Add(AutoGeneteId_DBAsowell(entity));
            }
            catch (EntityCommandExecutionException ex)
            {
                Insert(entity);
            }
        }

        public virtual void Delete(object id)
        {
            try
            {
                var entityToDelete = (TEntity) dbSet.Find(id);
                dbSet.Remove(entityToDelete);
            }
            catch (EntityCommandExecutionException ex)
            {
                Delete(id);
            }
        }

        public virtual void Delete(TEntity entityTODelete)
        {
            try
            {
                if (context.Entry(entityTODelete).State == EntityState.Deleted) dbSet.Attach(entityTODelete);
                dbSet.Remove(entityTODelete);
            }
            catch (EntityCommandExecutionException ex)
            {
                Delete(entityTODelete);
            }
        }

        public virtual void Update(TEntity entityToUpdate)
        {
            try
            {
                if (typeof(TEntity) == typeof(AdminRe))
                {
                    var admin = entityToUpdate as AdminRe;

                    //check the password change
                    try
                    {
                        var decryptPass = AESThenHMAC.SimpleDecryptWithPassword(admin.Pass, ENCRYPT_PHASE);
                        if (decryptPass.Equals(admin.DecryptedPass)) admin.Pass = admin.DecryptedPass;
                    }
                    catch (Exception ex)
                    {
                        //occur when the admin.Pass is not a encrypt password
                    }

                    var encryptPass = AESThenHMAC.SimpleEncryptWithPassword(admin.Pass, ENCRYPT_PHASE);
                    admin.Pass = encryptPass;
                }
                else
                {
                    if (typeof(TEntity) == typeof(Employee))
                    {
                        var emp = entityToUpdate as Employee;

                        //check the password change
                        try
                        {
                            var decryptPass = AESThenHMAC.SimpleDecryptWithPassword(emp.Pass, ENCRYPT_PHASE);
                            var decryptCode =
                                AESThenHMAC.SimpleDecryptWithPassword(emp.EmpCode, ENCRYPT_PHASE);
                            if (decryptPass.Equals(emp.DecryptedPass)) emp.Pass = emp.DecryptedPass;
                            if (decryptCode.Equals(emp.EmpCode)) emp.EmpCode = emp.DecryptedCode;
                        }
                        catch (Exception ex)
                        {
                            //occur when the emp.Pass is not a encrypt password
                        }


                        var encryptPass = AESThenHMAC.SimpleEncryptWithPassword(emp.Pass, ENCRYPT_PHASE);
                        var encryptCode = AESThenHMAC.SimpleEncryptWithPassword(emp.EmpCode, ENCRYPT_PHASE);
                        emp.Pass = encryptPass;
                        emp.EmpCode = encryptCode;
                    }
                }


                dbSet.Attach(entityToUpdate);
                context.Entry(entityToUpdate).State = EntityState.Modified;
            }
            catch (EntityCommandExecutionException ex)
            {
                Update(entityToUpdate);
            }
        }

        /// <summary>
        ///     auto generate id for all entities in Asowell Database
        ///     all id type is 10 character and the sign is depend on the type of entity
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public TEntity AutoGeneteId_DBAsowell(TEntity entity)
        {
            var sign = "";
            if (entity is Employee)
            {
                sign = "EMP";
                // lấy số thứ tự mới nhất
                var numberWantToset = (Get().Count() + 1).ToString();

                var blank = ID_SIZE_DBASOWELL - (sign.Length + numberWantToset.Length);
                var result = sign;
                for (var i = 0; i < blank; i++) result += "0";
                result += numberWantToset;

                var emp = entity as Employee;
                emp.EmpId = result;
            }
            else if (entity is AdminRe)
            {
                sign = "AD";
                // lấy số thứ tự mới nhất
                var numberWantToset = (Get().Count() + 1).ToString();

                var blank = ID_SIZE_DBASOWELL - (sign.Length + numberWantToset.Length);
                var result = sign;
                for (var i = 0; i < blank; i++) result += "0";
                result += numberWantToset;


                var admin = entity as AdminRe;
                admin.AdId = result;
            }
            else if (entity is Customer)
            {
                sign = "CUS";
                // lấy số thứ tự mới nhất
                var numberWantToset = (Get().Count() + 1).ToString();

                var blank = ID_SIZE_DBASOWELL - (sign.Length + numberWantToset.Length);
                var result = sign;
                for (var i = 0; i < blank; i++) result += "0";
                result += numberWantToset;

                var cus = entity as Customer;
                cus.CusId = result;
            }
            else if (entity is WareHouse)
            {
                sign = "WAH";
                // lấy số thứ tự mới nhất
                var numberWantToset = (Get().Count() + 1).ToString();

                var blank = ID_SIZE_DBASOWELL - (sign.Length + numberWantToset.Length);
                var result = sign;
                for (var i = 0; i < blank; i++) result += "0";
                result += numberWantToset;

                var wh = entity as WareHouse;
                wh.WarehouseId = result;
            }
            else if (entity is Product)
            {
                sign = "P";
                // lấy số thứ tự mới nhất
                var numberWantToset = (Get().Count() + 1).ToString();

                var blank = ID_SIZE_DBASOWELL - (sign.Length + numberWantToset.Length);
                var result = sign;
                for (var i = 0; i < blank; i++) result += "0";
                result += numberWantToset;

                var p = entity as Product;
                p.ProductId = result;
            }
            else if (entity is ProductDetail)
            {
                sign = "PD";
                // lấy số thứ tự mới nhất
                var numberWantToset = (Get().Count() + 1).ToString();

                var blank = ID_SIZE_DBASOWELL - (sign.Length + numberWantToset.Length);
                var result = sign;
                for (var i = 0; i < blank; i++) result += "0";
                result += numberWantToset;

                var pd = entity as ProductDetail;
                pd.PdetailId = result;
            }
            else if (entity is OrderNote)
            {
                sign = "ORD";
                // lấy số thứ tự mới nhất
                var numberWantToset = (Get().Count() + 1).ToString();

                var blank = ID_SIZE_DBASOWELL - (sign.Length + numberWantToset.Length);
                var result = sign;
                for (var i = 0; i < blank; i++) result += "0";
                result += numberWantToset;

                var ord = entity as OrderNote;
                ord.OrdernoteId = result;
            }
            else if (entity is SalaryNote)
            {
                sign = "SAN";
                // lấy số thứ tự mới nhất
                var numberWantToset = (Get().Count() + 1).ToString();

                var blank = ID_SIZE_DBASOWELL - (sign.Length + numberWantToset.Length);
                var result = sign;
                for (var i = 0; i < blank; i++) result += "0";
                result += numberWantToset;

                var sln = entity as SalaryNote;
                sln.SnId = result;
            }
            else if (entity is WorkingHistory)
            {
                sign = "WOH";
                // lấy số thứ tự mới nhất
                var numberWantToset = (Get().Count() + 1).ToString();

                var blank = ID_SIZE_DBASOWELL - (sign.Length + numberWantToset.Length);
                var result = sign;
                for (var i = 0; i < blank; i++) result += "0";
                result += numberWantToset;

                var wh = entity as WorkingHistory;
                wh.WhId = result;
            }


            else if (entity is StockOut)
            {
                sign = "STO";
                // lấy số thứ tự mới nhất
                var numberWantToset = (Get().Count() + 1).ToString();

                var blank = ID_SIZE_DBASOWELL - (sign.Length + numberWantToset.Length);
                var result = sign;
                for (var i = 0; i < blank; i++) result += "0";
                result += numberWantToset;

                var stkout = entity as StockOut;
                stkout.StockoutId = result;
            }
            else if (entity is StockIn)
            {
                sign = "STI";
                // lấy số thứ tự mới nhất
                var numberWantToset = (Get().Count() + 1).ToString();

                var blank = ID_SIZE_DBASOWELL - (sign.Length + numberWantToset.Length);
                var result = sign;
                for (var i = 0; i < blank; i++) result += "0";
                result += numberWantToset;

                var stkIn = entity as StockIn;
                stkIn.SiId = result;
            }
            else if (entity is ApWareHouse)
            {
                sign = "APW";
                // lấy số thứ tự mới nhất
                var numberWantToset = (Get().Count() + 1).ToString();

                var blank = ID_SIZE_DBASOWELL - (sign.Length + numberWantToset.Length);
                var result = sign;
                for (var i = 0; i < blank; i++) result += "0";
                result += numberWantToset;

                var wh = entity as ApWareHouse;
                wh.ApwarehouseId = result;
            }
            else if (entity is Stock)
            {
                sign = "STK";
                // lấy số thứ tự mới nhất
                var numberWantToset = (Get().Count() + 1).ToString();

                var blank = ID_SIZE_DBASOWELL - (sign.Length + numberWantToset.Length);
                var result = sign;
                for (var i = 0; i < blank; i++) result += "0";
                result += numberWantToset;

                var stock = entity as Stock;
                stock.StoId = result;
            }


            return entity;
        }
    }
}