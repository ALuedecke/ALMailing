using System.Collections.Generic;
using System.Net.Mail;
using System.Threading.Tasks;
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
        void SendSingleMail(SmtpClient smtp, MailMessage mail);
        #endregion
    }
}
