using Microsoft.AspNetCore.Mvc;
using ModelLayer.Entity;
using RepositaryLayer.DTO.RequestDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisinessLayer.Interface
{
    public interface IUserService
    {
        public Task<int> createUser(UserRequest request);
        public Task<UserResponce> Login(string Email, string password);
        public Task<string> ChangePasswordRequest(string Email);
        Task<string> ChangePassword(string otp, string password);
        //public Task<UserResponce> Authenticate(string email, string password);
    }
}
