using System;
using System.Linq;
using System.Threading.Tasks;
using CardsAPI.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace CardsAPI.Repository
{
    public interface IUserRepository
    {
        bool Signup(UserUpsertion user);
    }

    public class UserRepository : IUserRepository
    { 
        private readonly UserDbContext _userDbContext;

        public UserRepository(UserDbContext userDbContext)
        {
            _userDbContext = userDbContext;
        }

        public bool Signup(UserUpsertion user)
        {
            try
            {
                // check if user is existing
                var existingUser = (from dbUser in _userDbContext.Users
                                    where dbUser.MailId == user.MailId
                                    select dbUser).SingleOrDefaultAsync();

                if (null != existingUser.Result)
                    return false;

                if (CreateUserAsync(user).Result > 0)
                {
                    return true;
                }
                return false;
            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public async Task<int> CreateUserAsync(UserUpsertion user)
        {
            DbUser dbUser = new DbUser();
            dbUser.MailId = user.MailId;
            dbUser.Username = user.Name;
            dbUser.Password = user.Password;
            dbUser.Topics = new List<DbTopic>();
            _userDbContext.Add(dbUser);
            await _userDbContext.SaveChangesAsync();
            return dbUser.Id;
        }
    }
}
