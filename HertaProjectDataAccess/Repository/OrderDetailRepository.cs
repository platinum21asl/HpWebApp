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
    public class OrderDetailRepository : Repository<OrderDetail>, IOrderDetailRepository 
    {

        private readonly ApplicationDbContext _db;

        public OrderDetailRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;

        }

        public void Update(OrderDetail obj)
        {
           _db.OrderDetails.Update(obj);
        }
    }
}
