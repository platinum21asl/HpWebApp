using HertaProjectModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HertaProjectDataAccess.Repository.IRepository
{
    public interface ICateogryRepository : IRepository<Category>
    {
        void Update(Category obj);
    }
}
