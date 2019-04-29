using System.Collections.Generic;
using System.Net.Mail;
using System.Net;

namespace ALMailing
{
    public interface MailingInterface
    {
        #region Properties
        IEnumerable<MailMessage> Mail { get; set; }
        NetworkCredential Credential { get; set; }
        SmtpClient Smtp { get; set; }
        #endregion

        #region Methods
        string GetMailBodyFromTemplate(string path);
        string SendSingleMail(MailMessage mail);
        string SendSingleMail(SmtpClient smtp, MailMessage mail);
        string SendMails();
        string SendMails(List<MailMessage> lmail);
        string SendMails(SmtpClient smtp, List<MailMessage> lmail);
        #endregion
    }
}
