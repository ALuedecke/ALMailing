using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace ALMailing
{
    public class Mailing : MailingInterface
    {
        #region Members
        private List<MailMessage> m_mail;
        #endregion

        #region Properties
        public NetworkCredential Credential { get; set; }

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

        public SmtpClient Smtp { get; set; }
        #endregion

        #region Constructors
        public Mailing()
        {
            InitMembers();
        }
        public Mailing(SmtpClient smptp, NetworkCredential credantial, MailMessage mail)
        {
            InitMembers();
            Smtp =smptp;
            Credential = credantial;
            m_mail.Add(mail);
        }
        #endregion

        #region Public Methods
        public string GetMailBodyFromTemplate(string path)
        {
            string template = "";

            if (File.Exists(path))
            {
                template = File.ReadAllText(path);
            }

            return template;
        }

        public string SendMails()
        {
            return SendMails(Smtp, m_mail);
        }

        public string SendMails(List<MailMessage> lmail)
        {
            return SendMails(Smtp, lmail);
        }

        public string SendMails(SmtpClient smtp, List<MailMessage> lmail)
        {
            string msg = "";


            if (smtp == null)
            {
                throw new ArgumentException("Cannot be null", "smtp");
            }
            if (lmail == null)
            {
                throw new ArgumentException("Cannot be null", "lmail");
            }
            if (smtp.Credentials == null)
            {
                if (Credential == null)
                {
                    throw new NullReferenceException("Credential cannot be null") ;
                }
                else
                {
                    smtp.Credentials = Credential;
                }
            }

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

        public string SendSingleMail(MailMessage mail)
        {
            return SendSingleMail(Smtp, mail);
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
            Credential = new NetworkCredential();
            m_mail = new List<MailMessage>();
            Smtp = new SmtpClient();
        }

        private async Task Send(SmtpClient smtp, MailMessage mail)
        {
            if (smtp == null)
            {
                throw new ArgumentException("Cannot be null", "smtp");
            }
            if (mail == null)
            {
                throw new ArgumentException("Cannot be null", "lmail");
            }
            if (smtp.Credentials == null)
            {
                if (Credential == null)
                {
                    throw new NullReferenceException("Credential cannot be null");
                }
                else
                {
                    smtp.Credentials = Credential;
                }
            }

            using (smtp)
            {
                await smtp.SendMailAsync(mail);
            }
        }
        #endregion
    }
}
