using EASendMail;
using System.Collections.Generic;
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
        bool UseSsl { get; set; }
        #endregion

        #region Methods
        Task SendSingleMail(MailMessage mail);
        List<Task> SendMails(List<MailMessage> lmail);
        string SendMailTLS(SmtpMail mail, SvrConnType conntype);
        #endregion
    }

    public enum SvrConnType
    {
        NORMAL = SmtpConnectType.ConnectNormal,
        SSLAUTO = SmtpConnectType.ConnectSSLAuto,
        STARTTLS = SmtpConnectType.ConnectSTARTTLS,
        SSLDIRECT = SmtpConnectType.ConnectDirectSSL,
        TLS = SmtpConnectType.ConnectTryTLS
    }
}
