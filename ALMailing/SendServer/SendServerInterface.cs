using ALMailing.Enums;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        bool UseSsl { get; set; }
        #endregion

        #region Methods
        string SendMailTLS(Email mail, SvrConnType conntype);
        Task SendSingleMail(Email mail);
        List<Task> SendMails(Collection<Email> lmail);
        #endregion
    }
}
