using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CardsAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CardsAPI.Repository
{
    public interface IUserRepository
    {
    }

    public class UserRepository : IUserRepository
    {
        private readonly UserDbContext _userDbContext;

        public UserRepository (UserDbContext userDbContext)
        {
            _userDbContext = userDbContext;
        }

    }
}