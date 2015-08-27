using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Swappy_V2.Models
{
    public interface IRepository<Type>
    {
        List<Type> GetList();
        Type GetComputer(int id);
        void Create(Type item);
        void Update(Type item);
        void Delete(int id);
        void Save();
    }
}