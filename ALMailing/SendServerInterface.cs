using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace ALMailing
{
    public interface SendServerInterface
    {
        #region Properties
        string HostName { get; set; }
        int Port { get; set; }
        string NetworkUser { get; set; }
        string NetworkPassword { get; set; }
        #endregion

        #region Methods
        Task SendSingleMail(MailMessage mail);
        List<Task> SendMails(List<MailMessage> lmail);
        #endregion
    }
}
