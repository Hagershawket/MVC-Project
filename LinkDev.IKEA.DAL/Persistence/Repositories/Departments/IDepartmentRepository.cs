using LinkDev.IKEA.DAL.Entities.Departments;
using LinkDev.IKEA.DAL.Persistence.Repositories._Generic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkDev.IKEA.DAL.Persistence.Repositories.Departments
{
    public interface IDepartmentRepository : IGenericRepository<Department>
    {
        
    }
}
