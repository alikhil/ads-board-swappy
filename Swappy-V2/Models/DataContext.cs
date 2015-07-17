using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace Swappy_V2.Models
{
    public class DataContext : DbContext
    {
        public DbSet<AppUserModel> Users { get; set; }
    }
}