using System.Collections.ObjectModel;
using System.Net.Mail;

namespace ALMailing
{
    public interface MailingInterface
    {
        #region Properties
        Collection<MailMessage> Mails { get; set; }
        SendServer SendHost { get; set; }
        #endregion

        #region Methods
        string GetMailBodyFromTemplate(string path);
        string SendSingleMail(MailMessage mail);
        string SendSingleMail(SendServer sendhost, MailMessage mail);
        string SendMails();
        string SendMails(Collection<MailMessage> lmail);
        string SendMails(SendServer sendhost, Collection<MailMessage> lmail);
        #endregion
    }
}
