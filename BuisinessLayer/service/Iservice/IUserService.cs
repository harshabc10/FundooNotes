using Microsoft.AspNetCore.Mvc;
using ModelLayer.Entity;
using RepositaryLayer.DTO.RequestDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuisinessLayer.service.Iservice
{
    public interface IUserService
    {
        public Task<int> createUser(UserRequest request);
        public Task<UserResponce> Login(String Email, String password);
        public Task<String> ChangePasswordRequest(String Email);
        Task<string> ChangePassword(string otp, String password);
        //public Task<UserResponce> Authenticate(string email, string password);
    }
}
