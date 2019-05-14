using System.Collections.ObjectModel;

namespace ALMailing
{
    public interface MailingInterface
    {
        #region Properties
        Collection<Email> MailsToSend { get; set; }
        SendServer SendHost { get; set; }
        #endregion

        #region Methods
        string GetMailBodyFromTemplate(string path);
        string SendMailTLS(Email mail, SvrConnType conntype);
        string SendSingleMail(Email mail);
        string SendSingleMail(SendServer sendhost, Email mail);
        string SendMails();
        string SendMails(Collection<Email> lmail);
        string SendMails(SendServer sendhost, Collection<Email> mails);
        #endregion
    }
}
