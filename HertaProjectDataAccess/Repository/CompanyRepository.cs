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
    public class CompanyRepository : Repository<Company>, ICompanyRepository 
    {

        private readonly ApplicationDbContext _db;

        public CompanyRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;

        }

        public void Update(Company obj)
        {
           _db.Companies.Update(obj);
        }
    }
}
