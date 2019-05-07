using System.Collections.Generic;
using System.Net.Mail;

namespace ALMailing
{
    public interface MailingInterface
    {
        #region Properties
        IEnumerable<MailMessage> Mail { get; set; }
        SendServer SendHost { get; set; }
        #endregion

        #region Methods
        string GetMailBodyFromTemplate(string path);
        string SendSingleMail(MailMessage mail);
        string SendSingleMail(SendServer sendhost, MailMessage mail);
        string SendMails();
        string SendMails(List<MailMessage> lmail);
        string SendMails(SendServer sendhost, List<MailMessage> lmail);
        #endregion
    }
}
