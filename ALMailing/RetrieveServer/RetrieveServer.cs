using Limilabs.Client.IMAP;
using Limilabs.Client.POP3;
using Limilabs.Mail;
using Limilabs.Mail.Headers;
using Limilabs.Mail.MIME;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace ALMailing
{
    public class RetrieveServer : RetieveServerInterface
    {
        #region Properties
        public string HostName { get; set; }
        public int Port { get; set; }
        public string NetworkUser { get; set; }
        public string NetworkPassword { get; set; }
        public bool UseSsl { get; set; }
        #endregion

        #region Constructors
        public RetrieveServer()
        {
            InitClass("", 0, "", "", false);
        }

        public RetrieveServer(string host)
        {
            InitClass(host, 995, "", "", false);
        }

        public RetrieveServer(string host, string username, string password)
        {
            InitClass(host, 995, username, password, false);
        }

        public RetrieveServer(string host, int port, string username, string password)
        {
            InitClass(host, port, username, password, false);
        }

        public RetrieveServer(string host, int port, string username, string password, bool usessl)
        {
            InitClass(host, port, username, password, usessl);
        }
        #endregion

        #region Public methods
        public bool DeleteMailOnServer(string uid)
        {
            throw new NotImplementedException();
        }

        public Collection<Email> RetrieveMailsIMAP()
        {
            Collection<Email> retmails = new Collection<Email>();

            CheckProperties();

            Imap imap = new Imap();
            
            using (imap)
            {
                imap.Connect(HostName, Port, UseSsl);
                imap.UseBestLogin(NetworkUser, NetworkPassword);

                imap.ExamineInbox();
                
                foreach (long uid in imap.Search(Flag.Unseen))
                {
                    IMail imail = new MailBuilder().CreateFromEml(imap.GetMessageByUID(uid));
                }

                imap.Close();
            }

            return retmails;
        }

        public Collection<Email> RetrieveMailsPOP3()
        {
            Collection<Email> retmails = new Collection<Email>();

            CheckProperties();

            Pop3 pop3 = new Pop3();

            using (pop3)
            {
                pop3.Connect(HostName, Port, UseSsl);
                pop3.UseBestLogin(NetworkUser, NetworkPassword);

                foreach (string uid in pop3.GetAll())
                {
                    IMail imail = new MailBuilder().CreateFromEml(pop3.GetMessageByUID(uid));
                    retmails.Add(GetEmail(imail)); 
                }

                pop3.Close();

            }

            return retmails;
        }
        #endregion

        #region Private methods
        private void CheckProperties()
        {
            bool throw_ex = false;
            string msg = "";

            if (HostName.Equals(String.Empty))
            {
                msg += "HostName cannot be null or empty!\n";
                throw_ex = true;
            }
            if (NetworkUser.Equals(String.Empty))
            {
                msg += "NetworkUser cannot be null or empty!\n";
                throw_ex = true;

            }
            if (NetworkPassword.Equals(String.Empty))
            {
                msg += "NetworkPassword cannot be null or empty!\n";
                throw_ex = true;

            }
            if (throw_ex)
            {
                throw new NullReferenceException(msg);
            }
        }

        private Email GetEmail(IMail imail)
        {
            Email retmail = new Email();

            retmail.Date = (DateTime)imail.Date;
            retmail.From = new EmailAddress(imail.From.First().Address, imail.From.First().Name);

            if (imail.To.Count > 0)
            {
                foreach (MailAddress addr in imail.To)
                {
                    retmail.To.Add(new EmailAddress(addr.GetMailboxes().First().Address, addr.Name));
                }
            }

            if (imail.Cc.Count > 0)
            {
                foreach (MailAddress addr in imail.Cc)
                {
                    retmail.Cc.Add(new EmailAddress(addr.GetMailboxes().First().Address, addr.Name));
                }
            }

            if (imail.Bcc.Count > 0)
            {
                foreach (MailAddress addr in imail.Bcc)
                {
                    retmail.Cc.Add(new EmailAddress(addr.GetMailboxes().First().Address, addr.Name));
                }
            }

            retmail.Subject = imail.Subject;

            if (String.IsNullOrEmpty(imail.Text))
            {
                retmail.Body = imail.Html;
                retmail.IsHtml = true;
            }
            else
            {
                retmail.Body = imail.Text;
                retmail.IsHtml = false;
            }
            
            if (imail.Attachments.Count > 0)
            {
                foreach (MimeData att in imail.Attachments)
                {
                    retmail.Attachments.Add(new EmailAttachment()
                    {
                        ContentId = att.ContentId,
                        Data = att.Data,
                        Path = att.FileName
                    });
                }
            }

            retmail.Uid = imail.MessageID;

            return retmail;
        }

        private void InitClass(string host, int port, string username, string password, bool usessl)
        {
            HostName = host;
            Port = port;
            NetworkUser = username;
            NetworkPassword = password;
            UseSsl = usessl;
        }
        #endregion
    }
}
