using System;
using CardsAPI.Models;
using CardsAPI.Repository;

namespace CardsAPI.Services
{
    public class LoginService : ILogin
    {
        private readonly IUserRepository _userRepository;

        public LoginService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public bool Signup(UserUpsertion user)
        {
            return _userRepository.Signup(user);
        }
    }
}
