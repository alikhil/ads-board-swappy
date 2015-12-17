using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.Entity;
namespace Swappy_V2.Models
{
    /// <summary>
    /// Interface of db wrapper
    /// </summary>
    /// <typeparam name="Type">Type of data on tables</typeparam>
    public interface IRepository<Type>
    {
        IEnumerable<Type> GetAll();
        Type Get(int id);
        void Create(Type item);
        void AddRange(IEnumerable<Type> forAdd);
        void Update(Type item);
        void Delete(int id);
        void Save();
    }

    /// <summary>
    /// Deals table wrapper
    /// </summary>
    public class DealsRepository : IRepository<DealModel>
    {
        private bool disposed = false;

        ApplicationDbContext db = new ApplicationDbContext();
        public IEnumerable<DealModel> GetAll()
        {
            var list = db.Deals.Where(x=> x.State == DealState.Public).Include(x => x.Variants).Include(x => x.Images);
            return list;
        }

        public void AddRange(IEnumerable<DealModel> forAdd)
        {
            db.Deals.AddRange(forAdd);
        }

        public void Create(DealModel deal)
        {
            deal.DealCreated = DateTime.UtcNow;
            db.Deals.Add(deal);
        }

        public void Update(DealModel deal)
        {
            deal.DealUpdated = DateTime.UtcNow;
            db.Entry(deal).State = EntityState.Modified;
        }

        public void Delete(int id)
        {
            var deal = db.Deals.Find(id);
            deal.State = DealState.Deleted;
            Update(deal);
        }

        public void Save()
        {
            db.SaveChanges();
        }

        public DealModel Get(int id)
        {
            try
            {
                return GetAll().Single(x => x.Id == id);
            }
            catch(Exception e)
            {
                //TODO: log exception
                return null;
            }
        }

        public virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    db.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }

    /// <summary>
    /// Wrapper for user table
    /// </summary>
    public class UsersRepository : IRepository<AppUserModel>
    {
        private bool disposed = false;

        ApplicationDbContext db = new ApplicationDbContext();
        public IEnumerable<AppUserModel> GetAll()
        {
            var list = db.Users;
            return list;
        }

        public void AddRange(IEnumerable<AppUserModel> forAdd)
        {
            db.Users.AddRange(forAdd);
        }

        public void Create(AppUserModel user)
        {
            db.Users.Add(user);
        }

        public void Update(AppUserModel user)
        {
            db.Users.Attach(user);
            db.Entry(user).State = EntityState.Modified;
        }

        public void Delete(int id)
        {
            var user = db.Users.Find(id);
            db.Users.Remove(user);
        }

        public void Save()
        {
            db.SaveChanges();
        }

        public AppUserModel Get(int id)
        {
            return db.Users.Find(id);
        }

        public virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    db.Dispose();
                }
            }
            this.disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }

}