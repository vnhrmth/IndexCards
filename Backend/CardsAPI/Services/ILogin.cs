using System;
using CardsAPI.Models;

namespace CardsAPI.Services
{
    public interface ILogin
    {
        bool Signup(UserUpsertion user);
    }
}
