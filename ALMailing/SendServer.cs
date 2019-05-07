using EASendMail;
using System;
using System.Collections.Generic;
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
        public List<Task> SendMails(List<MailMessage> lmail)
        {
            if (lmail == null)
            {
                throw new ArgumentException("Cannot be null", "lmail");
            }

            List<Task> ltask = new List<Task>();

            foreach (MailMessage mail in lmail)
            {
                ltask.Add(SendSingleMail(mail));
            }

            return ltask;
        }

        public async Task SendSingleMail(MailMessage mail)
        {
            CheckProperties();

            NetworkCredential credential = new NetworkCredential(NetworkUser, NetworkPassword);
            System.Net.Mail.SmtpClient smtp = new System.Net.Mail.SmtpClient(HostName, Port);

            smtp.EnableSsl = UseSsl;

            using (smtp)
            {
                await smtp.SendMailAsync(mail);
            }

        }

        public string SendMailTLS(SmtpMail mail, SvrConnType conntype)
        {
            string msg = "";
            EASendMail.SmtpClient smtp = new EASendMail.SmtpClient();
            SmtpServer svr = new SmtpServer(HostName);

            svr.Port = Port;
            svr.User = NetworkUser;
            svr.Password = NetworkPassword;
            svr.ConnectType = (SmtpConnectType) conntype;

            try
            {
                smtp.SendMail(svr, mail);
            }
            catch (Exception e)
            {
                msg = e.Message;
            }

            return msg;
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
