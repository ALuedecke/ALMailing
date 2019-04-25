using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace ALMailing
{
    public class Mailing : MailingInterface
    {
        #region Members
        private List<NetworkCredential> m_credential;
        private List<MailMessage> m_mail;
        private List<SmtpClient> m_smtp;
        #endregion

        #region Properties
        public IEnumerable<NetworkCredential> Credential
        {
            get
            {
                return m_credential;
            }

            set
            {
                m_credential = (List<NetworkCredential>) value;
            }
        }

        public IEnumerable<MailMessage> Mail
        {
            get
            {
                return m_mail;
            }

            set
            {
                m_mail = (List<MailMessage>) value;
            }
        }

        public IEnumerable<SmtpClient> Smptp
        {
            get
            {
                return m_smtp;
            }

            set
            {
                m_smtp = (List<SmtpClient>) value;
            }
        }
        #endregion

        #region Constructors
        public Mailing()
        {
            InitMembers();
        }
        public Mailing(SmtpClient smptp, NetworkCredential credantial, MailMessage mail)
        {
            InitMembers();
            m_smtp.Add(smptp);
            m_credential.Add(credantial);
            m_mail.Add(mail);
        }
        #endregion

        #region Public Methods
        public string SendMails(SmtpClient smtp, List<MailMessage> lmail)
        {
            List<Task> ltask = new List<Task>();
            string msg = "";


            using (smtp)
            {
                foreach (MailMessage mail in lmail)
                {
                    try
                    {
                        smtp.Send(mail);
                    }
                    catch (Exception e)
                    {
                        msg += e.Message + '\n';
                    }

                }
            }

            return msg;
        }

        public string SendSingleMail(SmtpClient smtp, MailMessage mail)
        {
            string msg = "";
            Task task = Send(smtp, mail);
            task.Wait();

            if (task.Exception != null)
            {
                msg = task.Exception.Message;
            }

            task.Dispose();

            return msg;
        }
        #endregion

        #region Private Methods
        private void InitMembers()
        {
            m_credential = new List<NetworkCredential>();
            m_mail = new List<MailMessage>();
            m_smtp = new List<SmtpClient>();
        }

        private async Task Send(SmtpClient smtp, MailMessage mail)
        {
            using (smtp)
            {
                await smtp.SendMailAsync(mail);
            }
        }
        #endregion
    }
}
