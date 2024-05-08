using ModelLayer.Entity;
using RepositaryLayer.DTO.RequestDto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositaryLayer.Helper
{
    public class RegistrationDetailsForPublishing
    {

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        // Add any other fields you want to include for publishing

        public RegistrationDetailsForPublishing(UserEntity userRegModel)
        {
            FirstName = userRegModel.UserFirstName;
            LastName = userRegModel.UserLastName;
            Email = userRegModel.UserEmail;

        }
    }
}
