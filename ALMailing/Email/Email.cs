using System;
using System.Collections.ObjectModel;

namespace ALMailing
{
    public class Email
    {
        #region Properties
        public DateTime Date { get; set; }
        public EmailAddress From { get; set; }
        public Collection<EmailAddress> To { get; set; }
        public Collection<EmailAddress> Cc { get; set; }
        public Collection<EmailAddress> Bcc { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool IsHtml { get; set; }
        public Collection<EmailAttachment> Attachments { get; set; }
        public string Uid { get; set; }
        #endregion

        #region Constructors
        public Email()
        {
            InitClass(new EmailAddress(), new Collection<EmailAddress>(), "", "", false);
        }

        public Email(EmailAddress from, EmailAddress to)
        {
            Collection<EmailAddress> lto = new Collection<EmailAddress>();
            lto.Add(to);

            InitClass(from, lto, "", "", false);
        }

        public Email(string from, string to)
        {
            Collection<EmailAddress> lto = new Collection<EmailAddress>();
            lto.Add(new EmailAddress(to));

            InitClass(new EmailAddress(from), lto, "", "", false);
        }
        #endregion

        #region Private methods
        private void InitClass(EmailAddress from, Collection<EmailAddress> to, string subject, string body, bool ishtml)
        {
            From = from;
            To = to;
            Cc = new Collection<EmailAddress>();
            Bcc = new Collection<EmailAddress>();
            Subject = subject;
            Body = body;
            IsHtml = ishtml;
            Attachments = new Collection<EmailAttachment>();
        }
        #endregion
    }
}
