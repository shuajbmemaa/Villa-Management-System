using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Villa.Domain.Entities;

namespace Villa.Application.Common.Interfaces
{
    public interface IRepository<T> where T : class //kjo osht ne menyre gjenerale te pergjithshme class munet me kan hotel , hotelnumber , dhoma etj
    {
        IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter = null, string? includeProperties = null);
        T Get(Expression<Func<T, bool>> filter, string? includeProperties = null);
        void Add(T entity);

        bool Any(Expression<Func<T, bool>> filter);
        void Remove(T entity);
    }
}
