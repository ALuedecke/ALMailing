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
        #endregion

        #region Constructors
        public SendServer()
        {
            HostName = "";
            Port = 0;
            NetworkUser = "";
            NetworkPassword = "";
        }
        public SendServer(string host, int port, string username, string password)
        {
            HostName = host;
            Port = port;
            NetworkUser = username;
            NetworkPassword = password;

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
            SmtpClient smtp = new SmtpClient(HostName, Port);

            using (smtp)
            {
                await smtp.SendMailAsync(mail);
            }

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
        #endregion
    }
}
