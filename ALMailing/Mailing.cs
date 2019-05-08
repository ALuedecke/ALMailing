using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Net.Mail;
using System.Threading.Tasks;

namespace ALMailing
{
    public class Mailing : MailingInterface
    {
        #region Properties
        public Collection<MailMessage> Mails { get; set; }
        public SendServer SendHost { get; set; }
        #endregion

        #region Constructors
        public Mailing()
        {
            Mails = new Collection<MailMessage>();
            SendHost = new SendServer();
        }

        public Mailing(SendServer sendhost, MailMessage mail)
        {
            Mails = new Collection<MailMessage>();
            Mails.Add(mail);
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
            return SendMails(SendHost, Mails);
        }

        public string SendMails(Collection<MailMessage> mails)
        {
            return SendMails(SendHost, mails);
        }

        public string SendMails(SendServer sendhost, Collection<MailMessage> mails)
        {
            string msg = "";

            List<Task> ltask = sendhost.SendMails(mails);
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
