using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tabula.Interfaces
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string reciever, string subject, string message);
    }
}
