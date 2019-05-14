using EASendMail;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace ALMailing
{
    public class SendServer : SendServerInterface
    {
        #region Properties
        public string HostName { get; set; }
        public int Port { get; set; }
        public string NetworkUser { get; set; }
        public string NetworkPassword { get; set; }
        public bool UseSsl { get; set; }
        #endregion

        #region Constructors
        public SendServer()
        {
            InitClass("", 0, "", "", false);
        }

        public SendServer(string host)
        {
            InitClass(host, 25, "", "", false);
        }

        public SendServer(string host, string username, string password)
        {
            InitClass(host, 25, username, password, false);
        }

        public SendServer(string host, int port, string username, string password)
        {
            InitClass(host, port, username, password, false);
        }

        public SendServer(string host, int port, string username, string password, bool usessl)
        {
            InitClass(host, port, username, password, usessl);
        }
        #endregion

        #region Public methods
        public string SendMailTLS(Email mail, SvrConnType conntype)
        {
            string msg = "";
            EASendMail.SmtpClient smtp = new EASendMail.SmtpClient();

            try
            {
                CheckProperties();

                SmtpServer svr = new SmtpServer(HostName);

                svr.Port = Port;
                svr.User = NetworkUser;
                svr.Password = NetworkPassword;
                svr.ConnectType = (SmtpConnectType)conntype;
                smtp.SendMail(svr, GetSmtpMail(mail));
            }
            catch (Exception e)
            {
                msg = e.Message;
            }

            return msg;
        }

        public async Task SendSingleMail(Email mail)
        {
            CheckProperties();

            NetworkCredential credential = new NetworkCredential(NetworkUser, NetworkPassword);
            System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient(HostName, Port);
            smtp.EnableSsl = UseSsl;



            using (smtp)
            {
                await smtp.SendMailAsync(GetSysNetMail(mail));
            }

        }

        public List<Task> SendMails(Collection<Email> lmail)
        {
            if (lmail == null)
            {
                throw new ArgumentException("Cannot be null", "lmail");
            }

            List<Task> ltask = new List<Task>();

            foreach (Email mail in lmail)
            {
                ltask.Add(SendSingleMail(mail));
            }

            return ltask;
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

        private MailMessage GetSysNetMail(Email mail)
        {
            MailMessage retmail = new MailMessage(
                                         new System.Net.Mail.MailAddress(mail.From.Address, mail.From.DisplayName),
                                         new System.Net.Mail.MailAddress(mail.To.Address, mail.To.DisplayName)
                                       );

            retmail.Subject = mail.Subject;
            retmail.IsBodyHtml = mail.IsHtml;
            retmail.Body = mail.Body;

            if (mail.Cc.Count > 0)
            {
                foreach (EmailAddress addr in mail.Cc)
                {
                    retmail.CC.Add(new System.Net.Mail.MailAddress(addr.Address, addr.DisplayName));
                }
            }

            if (mail.Bcc.Count > 0)
            {
                foreach (EmailAddress addr in mail.Bcc)
                {
                    retmail.Bcc.Add(new System.Net.Mail.MailAddress(addr.Address, addr.DisplayName));
                }
            }

            if (mail.Attachments.Count > 0)
            {
                foreach (EmailAttachment att in mail.Attachments)
                {

                    retmail.Attachments.Add(new System.Net.Mail.Attachment(att.Path) { ContentId = att.ContentId});
                }

            }

            return retmail;
        }

        private SmtpMail GetSmtpMail(Email mail)
        {
            SmtpMail retmail = new SmtpMail("TryIt");

            retmail.From = new EASendMail.MailAddress(mail.From.DisplayName, mail.From.Address);
            retmail.To.Add(new EASendMail.MailAddress(mail.To.DisplayName, mail.To.Address));
            retmail.Subject = mail.Subject;
            
            if (mail.IsHtml)
            {
                retmail.HtmlBody = mail.Body;
            }
            else
            {
                retmail.TextBody = mail.Body;
            }

            if (mail.Cc.Count > 0)
            {
                foreach (EmailAddress addr in mail.Cc)
                {
                    retmail.Cc.Add(new EASendMail.MailAddress(addr.DisplayName, addr.Address));
                }
            }

            if (mail.Bcc.Count > 0)
            {
                foreach (EmailAddress addr in mail.Bcc)
                {
                    retmail.Bcc.Add(new EASendMail.MailAddress(addr.DisplayName, addr.Address));
                }
            }

            if (mail.Attachments.Count > 0)
            {
                foreach (EmailAttachment att in mail.Attachments)
                {
                    retmail.AddAttachment(att.Path);
                }

            }

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
