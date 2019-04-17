using System.Collections.Generic;
using System.Net.Mail;
using System.Net;

namespace ALMailing
{
    public interface MailingInterface
    {
        #region Properties
        IEnumerable<MailMessage> Mail { get; set; }
        IEnumerable<NetworkCredential> Credential { get; set; }
        IEnumerable<SmtpClient> Smptp { get; set; }
        #endregion

        #region Methods
        string SendSingleMail(SmtpClient smtp, MailMessage mail);
        string SendMails(SmtpClient smtp, List<MailMessage> lmail);
        #endregion
    }
}
