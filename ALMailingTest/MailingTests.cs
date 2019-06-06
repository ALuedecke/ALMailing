using ALMailing;
using NUnit.Framework;
using System.Configuration;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net.Configuration;

namespace ALMailingTest
{
    [TestFixture]
    public class MailingTests
    {
        [Test]
        public void Create_Mailing_Empty()
        {
            Mailing mailing = new Mailing();
            SendServer expSendHost = mailing.SendHost;

            Assert.AreEqual(expSendHost, mailing.SendHost);
            Assert.IsEmpty(mailing.MailsToSend);
        }

        [Test]
        public void Create_Mailing_WithParameters()
        {
            SendServer sendhost = new SendServer("smtp.server.xy", 587, "user.name@domain.xy", "password");
            Email mail = new Email("user1.name@domain.xy", "user2.name@domain.xy");

            Mailing mailing = new Mailing(sendhost, mail);

            Assert.AreEqual(mailing.SendHost, sendhost);
            Assert.AreEqual(mailing.MailsToSend.First(), mail);
        }

        [Test]
        public void Mailing_Set_RetrieveServer()
        {
            Mailing mailing = new Mailing();
            RetrieveServer retrievehost = new RetrieveServer();

            mailing.RetrieveHost = retrievehost;

            Assert.AreEqual(mailing.RetrieveHost, retrievehost);
        }

        [Test]
        public void Mailing_Set_RetrieveServer_Credential()
        {
            Mailing mailing = new Mailing();
            RetrieveServer retrievehost = new RetrieveServer();
            string username = "user.name@domain.xy";
            string password = "password";

            retrievehost.HostName = "pop3.server1.xy";
            retrievehost.NetworkUser = username;
            retrievehost.NetworkPassword = password;

            mailing.RetrieveHost = retrievehost;


            TestContext.WriteLine("RetrieveHost: " + mailing.RetrieveHost.HostName);
            TestContext.WriteLine("RetrieveHost.NetworkUser: " + mailing.RetrieveHost.NetworkUser);
            TestContext.WriteLine("RetrieveHost.NetworkPassword: " + mailing.RetrieveHost.NetworkPassword);

            Assert.AreEqual(mailing.RetrieveHost, retrievehost);
            Assert.AreEqual(mailing.RetrieveHost.NetworkUser, username);
            Assert.AreEqual(mailing.RetrieveHost.NetworkPassword, password);
        }

        [Test]
        public void Mailing_Set_SendServer()
        {
            Mailing mailing = new Mailing();
            SendServer sendhost = new SendServer();

            mailing.SendHost = sendhost;

            Assert.AreEqual(mailing.SendHost, sendhost);
        }

        [Test]
        public void Mailing_Set_SendServer_Credential()
        {
            Mailing mailing = new Mailing();
            SendServer sendhost = new SendServer();
            string username = "user.name@domain.xy";
            string password = "password";

            sendhost.HostName = "smtp.server1.xy";
            sendhost.NetworkUser = username;
            sendhost.NetworkPassword = password;

            mailing.SendHost = sendhost;

            Assert.AreEqual(mailing.SendHost, sendhost);
            Assert.AreEqual(mailing.SendHost.NetworkUser, username);
            Assert.AreEqual(mailing.SendHost.NetworkPassword, password);
        }

        [Test]
        public void Mailing_Set_SendServer_Credential_Add_2_Mails()
        {
            Collection<Email> mails = new Collection<Email>();
            Mailing mailing = new Mailing();
            SendServer sendhost = new SendServer();
            string username = "user.name@domain.xy";
            string password = "password";

            sendhost.HostName = "smtp.server1.xy";
            sendhost.NetworkUser = username;
            sendhost.NetworkPassword = password;
            mailing.SendHost = sendhost;

            mails.Add(new Email("user1.name@domain.xy", "user2.name@domain.xy"));
            mails.Add(new Email("user1.name@domain.yz", "user2.name@domain.yz"));
            mailing.MailsToSend = mails;

            Assert.AreEqual(mailing.SendHost, sendhost);
            Assert.AreEqual(mailing.SendHost.NetworkUser, username);
            Assert.AreEqual(mailing.SendHost.NetworkPassword, password);
            Assert.AreEqual(mailing.MailsToSend, mails);
        }

        [Test]
        public void Mailing_RetrieveMails_IMAP()
        {
            string port = ConfigurationManager.AppSettings["imapPort"];
            RetrieveServer retrievehost = new RetrieveServer(
                                                ConfigurationManager.AppSettings["imapServer"],
                                                int.Parse(port),
                                                ConfigurationManager.AppSettings["imapUser"],
                                                ConfigurationManager.AppSettings["imapPassword"]
                                              );
            retrievehost.UseSsl = true;

            Mailing mailing = new Mailing(retrievehost);

            TestContext.WriteLine(mailing.RetrieveHost.HostName);
            TestContext.WriteLine(mailing.RetrieveHost.Port);
            TestContext.WriteLine(mailing.RetrieveHost.NetworkUser);
            TestContext.WriteLine(mailing.RetrieveHost.NetworkPassword);
            TestContext.WriteLine(mailing.RetrieveHost.UseSsl);

            string msg = mailing.RetrieveMails(RetrieveType.IMAP);

            foreach (Email mail in mailing.MailsRetrieved)
            {
                TestContext.WriteLine("From: " + mail.From);
                TestContext.WriteLine("Subject: " + mail.Subject);
                TestContext.WriteLine("---");
            }

            Assert.IsEmpty(msg);
        }

        [Test]
        public void Mailing_RetrieveMails_POP3()
        {
            string port = ConfigurationManager.AppSettings["pop3Port"];
            RetrieveServer retrievehost = new RetrieveServer(
                                                ConfigurationManager.AppSettings["pop3Server"],
                                                int.Parse(port),
                                                ConfigurationManager.AppSettings["pop3User"],
                                                ConfigurationManager.AppSettings["pop3Password"]
                                              );
            retrievehost.UseSsl = false;

            Mailing mailing = new Mailing(retrievehost);

            TestContext.WriteLine("POP3-Server: " + mailing.RetrieveHost.HostName);
            TestContext.WriteLine("Port: " + mailing.RetrieveHost.Port);
            TestContext.WriteLine("Network User: " + mailing.RetrieveHost.NetworkUser);
            //TestContext.WriteLine(mailing.RetrieveHost.NetworkPassword);
            TestContext.WriteLine("SSL: " + mailing.RetrieveHost.UseSsl);

            string msg = mailing.RetrieveMails(RetrieveType.POP3);

            foreach (Email mail in mailing.MailsRetrieved)
            {
                TestContext.WriteLine(mail.ToString());
            }

            Assert.IsEmpty(msg);
        }

        [Test]
        public void Mailing_SendSingleMail()
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            MailSettingsSectionGroup mailsettings = (MailSettingsSectionGroup) config.GetSectionGroup("system.net/mailSettings");
            Mailing mailing = new Mailing();
            Email mail = new Email(
                                 new EmailAddress(mailsettings.Smtp.From),
                                 new EmailAddress(ConfigurationManager.AppSettings["mailDefaultRecipient"])
                             );
            SendServer sendhost = new SendServer(
                                    mailsettings.Smtp.Network.Host,
                                    mailsettings.Smtp.Network.Port,
                                    mailsettings.Smtp.Network.UserName,
                                    mailsettings.Smtp.Network.Password
                                  );

            mail.Subject = ConfigurationManager.AppSettings["mailSubject"];
            mail.Body = mailing.GetMailBodyFromTemplate(ConfigurationManager.AppSettings["mailBodyTxtTemplate"]);
            mail.Body = mail.Body.Replace("[:RECEPIENT:]", ConfigurationManager.AppSettings["mailDefaultRecipient"]);

            string msg = mailing.SendSingleMail(sendhost, mail);

            Assert.IsEmpty(msg);
        }

        [Test]
        public void Mailing_SendSingleMail_Html()
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            MailSettingsSectionGroup mailsettings = (MailSettingsSectionGroup) config.GetSectionGroup("system.net/mailSettings");
            Mailing mailing = new Mailing();
            Email mail = new Email(
                                 new EmailAddress(mailsettings.Smtp.From),
                                 new EmailAddress(ConfigurationManager.AppSettings["mailDefaultRecipient"])
                               );
            SendServer sendhost = new SendServer(
                                    mailsettings.Smtp.Network.Host,
                                    mailsettings.Smtp.Network.Port,
                                    mailsettings.Smtp.Network.UserName,
                                    mailsettings.Smtp.Network.Password
                                  );

            string[] addresspart = ConfigurationManager.AppSettings["mailDefaultRecipient"].Split('@');

            mail.Subject = ConfigurationManager.AppSettings["mailSubject"];
            mail.IsHtml = true;
            mail.Body = mailing.GetMailBodyFromTemplate(ConfigurationManager.AppSettings["mailBodyHtmlTemplate"]);
            mail.Body = mail.Body.Replace("[:RECEPIENT:]", addresspart[0]);
            mail.Attachments.Add(new EmailAttachment(ConfigurationManager.AppSettings["mailBodyImageFile"]));
            mail.Attachments[0].ContentId = "logo.png";

            string msg = mailing.SendSingleMail(sendhost, mail);

            Assert.IsEmpty(msg);
        }

        [Test]
        public void Mailing_SendSingleMail_Html_2_Recipients()
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            MailSettingsSectionGroup mailsettings = (MailSettingsSectionGroup)config.GetSectionGroup("system.net/mailSettings");
            Mailing mailing = new Mailing();
            Email mail = new Email(
                                 new EmailAddress(mailsettings.Smtp.From),
                                 new EmailAddress(ConfigurationManager.AppSettings["mailDefaultRecipient"])
                               );
            SendServer sendhost = new SendServer(
                                    mailsettings.Smtp.Network.Host,
                                    mailsettings.Smtp.Network.Port,
                                    mailsettings.Smtp.Network.UserName,
                                    mailsettings.Smtp.Network.Password
                                  );

            string[] addresspart = ConfigurationManager.AppSettings["mailDefaultRecipient"].Split('@');

            mail.To.Add(new EmailAddress("andreas.luedecke@kontacts.de", "Andreas Lüdecke"));
            mail.Subject = ConfigurationManager.AppSettings["mailSubject"];
            mail.IsHtml = true;
            mail.Body = mailing.GetMailBodyFromTemplate(ConfigurationManager.AppSettings["mailBodyHtmlTemplate"]);
            mail.Body = mail.Body.Replace("[:RECEPIENT:]", addresspart[0]);
            mail.Attachments.Add(new EmailAttachment(ConfigurationManager.AppSettings["mailBodyImageFile"]));
            mail.Attachments[0].ContentId = "logo.png";

            string msg = mailing.SendSingleMail(sendhost, mail);

            Assert.IsEmpty(msg);
        }

        [TestCase("anti.hacker@mailcontrol.com", "andreas.luedecke@kontacts.de", "")]
        //[TestCase("anti.hacker@mailcontrol.com", "a_luedecke@gmx.de", "")]
        //[TestCase("anti.hacker@mailcontrol.com", "a.luedecke4@gmail.com", "")]
        public void Mailing_SendSingleMail_Html_WithInput(string from, string to, string expected)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            MailSettingsSectionGroup mailsettings = (MailSettingsSectionGroup)config.GetSectionGroup("system.net/mailSettings");
            Mailing mailing = new Mailing();
            Email mail = new Email(
                                 new EmailAddress(from),
                                 new EmailAddress(to)
                               );
            mailing.SendHost = new SendServer(
                                 mailsettings.Smtp.Network.Host,
                                 mailsettings.Smtp.Network.Port,
                                 mailsettings.Smtp.Network.UserName,
                                 mailsettings.Smtp.Network.Password
                               );

            string[] addresspart = to.Split('@');

            mail.Subject = ConfigurationManager.AppSettings["mailSubject"];
            mail.IsHtml = true;
            mail.Body = mailing.GetMailBodyFromTemplate(ConfigurationManager.AppSettings["mailBodyHtmlTemplate"]);
            mail.Body = mail.Body.Replace("[:RECEPIENT:]", addresspart[0]);
            mail.Attachments.Add(new EmailAttachment(ConfigurationManager.AppSettings["mailBodyImageFile"]));
            mail.Attachments[0].ContentId = "logo.png";

            string msg = mailing.SendSingleMail(mail);

            Assert.AreEqual(expected, msg);
        }

        [Test]
        public void Mailing_SendMails_Html()
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            Collection<string> laddress = new Collection<string>()
            {
                "andreas.luedecke@kontacts.de",
                "azamat.khasanov@kontacts.de",
                "ettker@posteo.de",
                "michael.kickmunter@kontacts.de",
                "sl@kontacts.de"
                /*"alfred.liesecke@kontacts.de",
                "a_luedecke@gmx.de",
                "a.luedecke4@gmail.com"
                "c.kapella@freenet.de",
                "andreas.penzold@kontacts.de",
                "azamat.khasanov@kontacts.de",
                "ettker@posteo.de",
                "michael.kickmunter@kontacts.de",
                "sl@kontacts.de"*/
            };
            MailSettingsSectionGroup mailsettings = (MailSettingsSectionGroup)config.GetSectionGroup("system.net/mailSettings");
            Mailing mailing = new Mailing();
            string img_file = ConfigurationManager.AppSettings["mailBodyImageFile"];
            string mailbody = mailing.GetMailBodyFromTemplate(ConfigurationManager.AppSettings["mailBodyHtmlTemplate"]);

            foreach (string address in laddress)
            {
                Email mail = new Email(
                                     new EmailAddress(address),
                                     new EmailAddress(address)
                                   );
                string[] addresspart = address.Split('@');

                mail.Subject = addresspart[0];
                mail.IsHtml = true;
                mail.Body = mailbody.Replace("[:RECEPIENT:]", addresspart[0]);
                mail.Attachments.Add(new EmailAttachment(img_file));
                mail.Attachments[0].ContentId = "logo.png";

                mailing.MailsToSend.Add(mail);
            }

            mailing.SendHost = new SendServer(
                                 mailsettings.Smtp.Network.Host,
                                 mailsettings.Smtp.Network.Port,
                                 mailsettings.Smtp.Network.UserName,
                                 mailsettings.Smtp.Network.Password
                               );

            string msg = mailing.SendMails();

            Assert.IsEmpty(msg);
        }

        [Test]
        public void Mailing_SendMailTLS()
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            MailSettingsSectionGroup mailsettings = (MailSettingsSectionGroup)config.GetSectionGroup("system.net/mailSettings");
            Mailing mailing = new Mailing();
            Email mail = new Email(
                                 new EmailAddress(mailsettings.Smtp.From),
                                 new EmailAddress(ConfigurationManager.AppSettings["mailDefaultRecipient"])
                               );
            SendServer sendhost = new SendServer(
                                    mailsettings.Smtp.Network.Host,
                                    mailsettings.Smtp.Network.Port,
                                    mailsettings.Smtp.Network.UserName,
                                    mailsettings.Smtp.Network.Password
                                  );
            mail.Subject = ConfigurationManager.AppSettings["mailSubject"];
            mail.Body = mailing.GetMailBodyFromTemplate(ConfigurationManager.AppSettings["mailBodyTxtTemplate"]);
            mail.Body = mail.Body.Replace("[:RECEPIENT:]", ConfigurationManager.AppSettings["mailDefaultRecipient"]);

            mailing.SendHost = sendhost;
            string msg = mailing.SendMailTLS(mail, SvrConnType.STARTTLS);

            Assert.IsEmpty(msg);
        }
    }
}
