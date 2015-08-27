using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.Entity;
namespace Swappy_V2.Models
{
    /// <summary>
    /// Интерфейс обертки для работы с таблицами
    /// </summary>
    /// <typeparam name="Type">Тип данных в таблице</typeparam>
    public interface IRepository<Type>
    {
        List<Type> GetList();
        Type Get(int id);
        void Create(Type item);
        void Update(Type item);
        void Delete(int id);
        void Save();
    }

    /// <summary>
    /// Обертка для работы с таблицей объявлений
    /// </summary>
    public class DealsRepository : IRepository<DealModel>
    {
        private bool disposed = false;

        DataContext db = new DataContext();
        public List<DealModel> GetList()
        {
            var list = db.Deals.Include(x => x.ItemToChange).Include(x => x.Variants);
            return list.ToList();
        }

        public void Create(DealModel deal)
        {
            db.Deals.Add(deal);
        }

        public void Update(DealModel deal)
        {
            db.Entry(deal).State = EntityState.Modified;
        }

        public void Delete(int id)
        {
            var deal = db.Deals.Find(id);
            db.Deals.Remove(deal);
        }

        public void Save()
        {
            db.SaveChanges();
        }

        public DealModel Get(int id)
        {
            return db.Deals.Find(id);
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