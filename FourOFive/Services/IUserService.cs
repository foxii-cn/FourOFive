using FourOFive.Models.DataBaseModels;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FourOFive.Services
{
    public interface IUserService
    {
        public Task<User> RegisterAsync(User user);
        public Task<User> LogInAsync(User user);
        public Task<List<User>> QueryAsync(Expression<Func<User, bool>> whereExp = null, int pageIndex = 0, int pageSize = 0);
        public Task<long> CountAsync(Expression<Func<User, bool>> whereExp = null);
    }
}
