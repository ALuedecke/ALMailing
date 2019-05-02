using System.Collections.Generic;
using System.IO;
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

        public SendServer SendHost { get; set; }
        #endregion

        #region Constructors
        public Mailing()
        {
            m_mail = new List<MailMessage>();
            SendHost = new SendServer();
        }
        public Mailing(SendServer sendhost, MailMessage mail)
        {
            m_mail = new List<MailMessage>();
            m_mail.Add(mail);
            SendHost = sendhost;
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
            return SendMails(SendHost, m_mail);
        }

        public string SendMails(List<MailMessage> lmail)
        {
            return SendMails(SendHost, lmail);
        }

        public string SendMails(SendServer sendhost, List<MailMessage> lmail)
        {
            string msg = "";

            List<Task> ltask = sendhost.SendMails(lmail);
            Task.WaitAll(ltask.ToArray());

            foreach (Task task in ltask)
            {
                if (task.Exception != null)
                {
                    msg += task.Exception.Message + '\n';
                }

                task.Dispose();
            }

            return msg;
        }

        public string SendSingleMail(MailMessage mail)
        {
            return SendSingleMail(SendHost, mail);
        }
        public string SendSingleMail(SendServer sendhost, MailMessage mail)
        {
            string msg = "";
            Task task = sendhost.SendSingleMail(mail);
            task.Wait();

            if (task.Exception != null)
            {
                msg = task.Exception.Message;
            }

            task.Dispose();

            return msg;
        }
        #endregion
    }
}
