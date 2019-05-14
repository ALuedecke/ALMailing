using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;

namespace ALMailing
{
    public class Mailing : MailingInterface
    {
        #region Properties
        public Collection<Email> MailsRetrieved { get; set; }
        public Collection<Email> MailsToSend { get; set; }
        public RetrieveServer RetrieveHost { get; set; }
        public SendServer SendHost { get; set; }
        #endregion

        #region Constructors
        public Mailing()
        {
            InitClass(new RetrieveServer(), new SendServer(), new Collection<Email>(), new Collection<Email>());
        }

        public Mailing(SendServer sendhost, Email mailtosend)
        {
            InitClass(new RetrieveServer(), sendhost, new Collection<Email>(), new Collection<Email>());
            MailsToSend.Add(mailtosend);
        }

        public Mailing(RetrieveServer retrievehost)
        {
            InitClass(retrievehost, new SendServer(), new Collection<Email>(), new Collection<Email>());
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

        public Collection<Email> RetrieveMails(RetrieveServer retrievehost, RetrieveType retrievetype)
        {
             switch (retrievetype)
            {
                case RetrieveType.IMAP:
                    return retrievehost.RetrieveMailsIMAP();
                case RetrieveType.POP3:
                    return retrievehost.RetrieveMailsPOP3();
                default:
                    return new Collection<Email>();
            }
        }

        public string RetrieveMails(RetrieveType retrievetype)
        {
            string msg = "";

            try
            {
                MailsRetrieved = RetrieveMails(RetrieveHost, retrievetype);
            }
            catch (Exception e)
            {
                msg = e.Message;
            }
            
            return msg;
        }

        public string SendMailTLS(Email mail, SvrConnType conntype)
        {
            return SendHost.SendMailTLS(mail, conntype);
        }

        public string SendSingleMail(Email mail)
        {
            return SendSingleMail(SendHost, mail);
        }

        public string SendSingleMail(SendServer sendhost, Email mail)
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

        public string SendMails()
        {
            return SendMails(SendHost, MailsToSend);
        }

        public string SendMails(Collection<Email> mails)
        {
            return SendMails(SendHost, mails);
        }

        public string SendMails(SendServer sendhost, Collection<Email> mails)
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
        #endregion

        #region Private Methods
        private void InitClass(
                       RetrieveServer retrievehost,
                       SendServer sendhost,
                       Collection<Email> mailsretrieved,
                       Collection<Email> mails2send
                     ) 
        {
            MailsRetrieved = mailsretrieved;
            MailsToSend = mails2send;
            RetrieveHost = retrievehost;
            SendHost = sendhost;
        }
        #endregion
    }
}
