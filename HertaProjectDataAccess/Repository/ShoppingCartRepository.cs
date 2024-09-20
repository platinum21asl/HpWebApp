using HertaProjectDataAccess.Data;
using HertaProjectDataAccess.Data;
using HertaProjectDataAccess.Repository.IRepository;
using HertaProjectModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HertaProjectDataAccess.Repository
{
    public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository 
    {

        private readonly ApplicationDbContext _db;

        public ShoppingCartRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;

        }

        public void Update(ShoppingCart obj)
        {
           _db.ShoppingCarts.Update(obj);
        }
    }
}
