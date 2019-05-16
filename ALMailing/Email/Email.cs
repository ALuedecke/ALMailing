using System;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

namespace ALMailing
{
    public class Email: EmailInterface
    {
        #region Properties
        public DateTime? Date { get; set; }
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

        #region Public methods
        public string GetAddressCollectionAsString(
                        Collection<EmailAddress> laddress,
                        EmailAddressPart addresspart = EmailAddressPart.BOTH
                      )
        {
            string retvalue = "";

            if (laddress.Count > 0)
            {
                bool start = true;

                foreach (EmailAddress addr in laddress)
                {
                    switch (addresspart)
                    {
                        case EmailAddressPart.ADDRESS:
                            retvalue += (start) ? addr.Address : "; " + addr.Address;
                            break;
                        case EmailAddressPart.DISPLAYNAME:
                            retvalue += (start) ? addr.DisplayName : "; " + addr.DisplayName;
                            break;
                        case EmailAddressPart.BOTH:
                            retvalue += (start) ? addr.ToString() : "; " + addr.ToString();
                            break;
                    }

                    if (start)
                    {
                        start = false;
                    }
                }
            }

            return retvalue;
        }

        public string GetAttachmentCollectionAsString(Collection<EmailAttachment> lattachment)
        {
            string retvalue = "";

            if (lattachment.Count > 0 )
            {
                bool start = true;

                foreach (EmailAttachment att in lattachment )
                {
                    retvalue += (start) ? att.ToString() : "; " + att.ToString();
                }

                if (start)
                {
                    start = false;
                }
            }

            return retvalue;
        }

        public string GetBodyAsPlainText()
        {
            string retvalue = Body;

            retvalue = Regex.Replace(retvalue, "\t", " ");
            retvalue = Regex.Replace(retvalue, "<(.|\n)*?>", "");
            retvalue = Regex.Replace(retvalue, @"^\s*(\r\n|\Z)", "", RegexOptions.Multiline);
            retvalue = Regex.Replace(retvalue, @"[ ]{2,}", " ");

            return retvalue;
        }
        #endregion

        #region Public overrides
        public override string ToString()
        {
            string part = "";
            string retvalue = "";

            retvalue = "--------";
            retvalue += "\nUid: " + Uid;
            retvalue += "\nDate: " + Date;
            retvalue += "\nFrom: " + From;
            retvalue += "\nTo: " + GetAddressCollectionAsString(To);

            part = GetAddressCollectionAsString(Cc);

            if (!String.IsNullOrEmpty(part))
            {
                retvalue += "\n--------";
                retvalue += "\nCC: " + part;
            }

            part = GetAddressCollectionAsString(Bcc);

            if (!String.IsNullOrEmpty(part))
            {
                retvalue += "\nBCC: " + part;
            }

            retvalue += "\n--------";
            retvalue += "\nSubject: " + Subject;

            part = GetAttachmentCollectionAsString(Attachments);

            if (!String.IsNullOrEmpty(part))
            {
                retvalue += "\n--------";
                retvalue += "\nAttachment(s): " + part;
            }

            retvalue += "\n--------";
            retvalue += "\n" + GetBodyAsPlainText();

            return retvalue;
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
