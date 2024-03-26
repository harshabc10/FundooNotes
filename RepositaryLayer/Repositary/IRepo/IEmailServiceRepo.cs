﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RepositaryLayer.Repositary.IRepo
{
    public interface IEmailServiceRepo
    {
        Task SendEmailAsync(string recipientEmail, string subject, string body);
    }
}
