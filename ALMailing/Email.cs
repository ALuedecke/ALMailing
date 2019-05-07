using System.Net.Mail;

using System.Collections.ObjectModel;

namespace ALMailing
{
    public class Email
    {

        #region Properties
        public EmailAddress From { get; set; }
        public EmailAddress To { get; set; }
        public string Body { get; set; }
        public bool IsBodyHtml { get; set; }
        public string Subject { get; set; }
        Collection<Attachment> Attachments { get; set; }
        #endregion

        #region Constructors

        #endregion
    }
}
