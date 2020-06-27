using FourOFive.Managers;
using FourOFive.Models.DataBaseModels;
using FourOFive.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace FourOFive.Services
{
    public class UserService : IUserService
    {
        private readonly ICreditUtility credit;
        private readonly IEncryptUtility encrypt;
        private readonly IAuthorityUtility authority;
        private readonly IDatabaseModelManager database;
        private readonly ILogManager logger;

        public UserService(ICreditUtility credit, IEncryptUtility encrypt, IAuthorityUtility authority, IDatabaseModelManager database, ILogManagerFactory loggerFactory)
        {
            this.credit = credit;
            this.encrypt = encrypt;
            this.authority = authority;
            this.database = database;
            logger = loggerFactory.CreateManager<UserService>();
        }

        public async Task<User> RegisterAsync(User user)
        {
            byte[] vs = await encrypt.CreateNewSaltAsync();
            User registeredUser = new User
            {
                UserName = user.UserName ?? throw new NullReferenceException("用户名为空"),
                Salt = Convert.ToBase64String(vs),
                Password = await encrypt.HashEncryptAsync(user.Password ?? throw new NullReferenceException("密码为空"), vs),
                Name = user.Name ?? throw new NullReferenceException("名字为空"),
                NationalIdentificationNumber = user.NationalIdentificationNumber ?? throw new NullReferenceException("身份证为空"),
                CreditValue = credit.GetInitialCreditValue(),
                Authority = authority.GetInitialUserAuthorityValue()
            };
            int affectedRows = await database.InsertAsync(new User[] { registeredUser });
            if (affectedRows == 0)
            {
                throw new Exception("创建用户记录失败");
            }

            return registeredUser;

        }
        public async Task<User> LogInAsync(User user)
        {
            User loggedUser = (await database.SelectAsync<User, User>(u => u.UserName == user.UserName)).FirstOrDefault();
            if (loggedUser == null || !loggedUser.Password.Equals(await encrypt.HashEncryptAsync(user.Password ?? throw new NullReferenceException("密码为空"), Convert.FromBase64String(loggedUser.Salt))))
            {
                throw new Exception("用户名或密码错误");
            }

            return loggedUser;
        }
        public async Task<List<User>> QueryAsync(Expression<Func<User, bool>> whereExp = null, int pageIndex = 0, int pageSize = 0)
        {
            return await database.SelectAsync<User, User>(whereExp, pageIndex: pageIndex, pageSize: pageSize);
        }

        public async Task<long> CountAsync(Expression<Func<User, bool>> whereExp = null)
        {
            return await database.CountAsync(whereExp);
        }
    }
}
