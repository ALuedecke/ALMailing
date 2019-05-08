using System.Collections.ObjectModel;

namespace ALMailing
{
    public interface MailingInterface
    {
        #region Properties
        Collection<Email> Mails { get; set; }
        SendServer SendHost { get; set; }
        #endregion

        #region Methods
        string GetMailBodyFromTemplate(string path);
        string SendSingleMail(Email mail);
        string SendSingleMail(SendServer sendhost, Email mail);
        string SendMails();
        string SendMails(Collection<Email> lmail);
        string SendMails(SendServer sendhost, Collection<Email> mails);
        #endregion
    }
}
