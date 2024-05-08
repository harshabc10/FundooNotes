using Microsoft.AspNetCore.Mvc;
using ModelLayer.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositaryLayer.Interface
{
    public interface IUserRepo
    {
        public Task<int> createUser(UserEntity entity);
        public Task<UserEntity> GetUserByEmail(string email);
        Task<int> UpdatePassword(string mailid, string password);
    }
}
