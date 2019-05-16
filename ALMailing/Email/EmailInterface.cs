using System;
using System.Collections.ObjectModel;

namespace ALMailing
{
    public interface EmailInterface
    {
        #region Properties
        DateTime? Date { get; set; }
        EmailAddress From { get; set; }
        Collection<EmailAddress> To { get; set; }
        Collection<EmailAddress> Cc { get; set; }
        Collection<EmailAddress> Bcc { get; set; }
        string Subject { get; set; }
        string Body { get; set; }
        bool IsHtml { get; set; }
        Collection<EmailAttachment> Attachments { get; set; }
        string Uid { get; set; }
        #endregion

        #region Methods
        string GetAddressCollectionAsString(
                        Collection<EmailAddress> laddress,
                        EmailAddressPart addresspart = EmailAddressPart.BOTH
               );
        string GetAttachmentCollectionAsString(Collection<EmailAttachment> lattachment);
        string GetBodyAsPlainText();
        #endregion
    }
}
