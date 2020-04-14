using System;
using CardsAPI.Models;

namespace CardsAPI.Services
{
    public interface ILogin
    {
        User Login(string emailId, string password);
        bool Signup(UserUpsertion user);
    }
}
