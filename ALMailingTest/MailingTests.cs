using ALMailing;
using NUnit.Framework;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net.Configuration;

namespace ALMailingTest
{
    [TestFixture]
    public class MailingTests
    {
        [Test]
        public void CreateMailingEmpty()
        {
            Mailing mailing = new Mailing();
            SendServer expSendHost = mailing.SendHost;

            Assert.AreEqual(expSendHost, mailing.SendHost);
            Assert.IsEmpty(mailing.Mail);
        }

        [Test]
        public void CreateMailingWithParameters()
        {
            SendServer sendhost = new SendServer("smtp.server.xy", 587, "user.name@domain.xy", "password");
            MailMessage mail = new MailMessage("user1.name@domain.xy", "user2.name@domain.xy");

            Mailing mailing = new Mailing(sendhost, mail);

            Assert.AreEqual(mailing.SendHost, sendhost);
            Assert.AreEqual(mailing.Mail.First(), mail);
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
            List<MailMessage> lmail = new List<MailMessage>();
            Mailing mailing = new Mailing();
            SendServer sendhost = new SendServer();
            string username = "user.name@domain.xy";
            string password = "password";

            sendhost.HostName = "smtp.server1.xy";
            sendhost.NetworkUser = username;
            sendhost.NetworkPassword = password;
            mailing.SendHost = sendhost;

            lmail.Add(new MailMessage("user1.name@domain.xy", "user2.name@domain.xy"));
            lmail.Add(new MailMessage("user1.name@domain.yz", "user2.name@domain.yz"));
            mailing.Mail = lmail;

            Assert.AreEqual(mailing.SendHost, sendhost);
            Assert.AreEqual(mailing.SendHost.NetworkUser, username);
            Assert.AreEqual(mailing.SendHost.NetworkPassword, password);
            Assert.AreEqual(mailing.Mail, lmail);
        }

        [Test]
        public void MailingSendSingleMail()
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            MailSettingsSectionGroup mailsettings = (MailSettingsSectionGroup) config.GetSectionGroup("system.net/mailSettings");
            Mailing mailing = new Mailing();
            MailMessage mail = new MailMessage(
                                 new MailAddress(mailsettings.Smtp.From),
                                 new MailAddress(ConfigurationManager.AppSettings["mailDefaultRecipient"])
                               );
            SendServer sendhost = new SendServer(
                                    mailsettings.Smtp.Network.Host,
                                    (int)mailsettings.Smtp.Network.Port,
                                    mailsettings.Smtp.Network.UserName,
                                    mailsettings.Smtp.Network.Password
                                  );

            mail.Subject = ConfigurationManager.AppSettings["mailSubject"];
            mail.Body = mailing.GetMailBodyFromTemplate(ConfigurationManager.AppSettings["mailBodyTxtTemplate"]);

            string msg = mailing.SendSingleMail(sendhost, mail);

            Assert.IsEmpty(msg);
        }

        [Test]
        public void MailingSendSingleMailHtml()
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            MailSettingsSectionGroup mailsettings = (MailSettingsSectionGroup) config.GetSectionGroup("system.net/mailSettings");
            Mailing mailing = new Mailing();
            MailMessage mail = new MailMessage(
                                 new MailAddress(mailsettings.Smtp.From),
                                 new MailAddress(ConfigurationManager.AppSettings["mailDefaultRecipient"])
                               );
            SendServer sendhost = new SendServer(
                                    mailsettings.Smtp.Network.Host,
                                    (int)mailsettings.Smtp.Network.Port,
                                    mailsettings.Smtp.Network.UserName,
                                    mailsettings.Smtp.Network.Password
                                  );

            string[] addresspart = ConfigurationManager.AppSettings["mailDefaultRecipient"].Split('@');

            mail.Subject = ConfigurationManager.AppSettings["mailSubject"];
            mail.IsBodyHtml = true;
            mail.Body = mailing.GetMailBodyFromTemplate(ConfigurationManager.AppSettings["mailBodyHtmlTemplate"]);
            mail.Body = mail.Body.Replace("[:RECEPIENT:]", addresspart[0]);
            mail.Attachments.Add(new Attachment(ConfigurationManager.AppSettings["mailBodyImageFile"]));
            mail.Attachments[0].ContentId = "logo.png";

            string msg = mailing.SendSingleMail(sendhost, mail);

            Assert.IsEmpty(msg);
        }

        [TestCase("anti.hacker@mailcontrol.com", "andreas.luedecke@kontacts.de", "")]
        //[TestCase("anti.hacker@mailcontrol.com", "a_luedecke@gmx.de", "")]
        //[TestCase("anti.hacker@mailcontrol.com", "a.luedecke4@gmail.com", "")]
        public void MailingSendSingleMailHtmlWithInput(string from, string to, string expected)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            MailSettingsSectionGroup mailsettings = (MailSettingsSectionGroup)config.GetSectionGroup("system.net/mailSettings");
            Mailing mailing = new Mailing();
            MailMessage mail = new MailMessage(
                                 new MailAddress(from),
                                 new MailAddress(to)
                               );
            mailing.SendHost = new SendServer(
                                 mailsettings.Smtp.Network.Host,
                                 (int)mailsettings.Smtp.Network.Port,
                                 mailsettings.Smtp.Network.UserName,
                                 mailsettings.Smtp.Network.Password
                               );

            string[] addresspart = to.Split('@');

            mail.Subject = ConfigurationManager.AppSettings["mailSubject"];
            mail.IsBodyHtml = true;
            mail.Body = mailing.GetMailBodyFromTemplate(ConfigurationManager.AppSettings["mailBodyHtmlTemplate"]);
            mail.Body = mail.Body.Replace("[:RECEPIENT:]", addresspart[0]);
            mail.Attachments.Add(new Attachment(ConfigurationManager.AppSettings["mailBodyImageFile"]));
            mail.Attachments[0].ContentId = "logo.png";

            string msg = mailing.SendSingleMail(mail);

            Assert.AreEqual(expected, msg);
        }

        [Test]
        public void MailingSendMailsHtml()
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            List<string> laddress = new List<string>()
            {
                "andreas.luedecke@kontacts.de",
                "a_luedecke@gmx.de",
                "a.luedecke4@gmail.com"
                /*"alfred.liesecke@kontacts.de",
                "c.kapella@freenet.de",
                "andreas.penzold@kontacts.de",
                "azamat.khasanov@kontacts.de",
                "ettker@posteo.de",
                "michael.kickmunter@kontacts.de",
                "sl@kontacts.de"*/
            };
            List<MailMessage> lmail = new List<MailMessage>();
            MailSettingsSectionGroup mailsettings = (MailSettingsSectionGroup)config.GetSectionGroup("system.net/mailSettings");
            Mailing mailing = new Mailing();
            string img_file = ConfigurationManager.AppSettings["mailBodyImageFile"];
            string mailbody = mailing.GetMailBodyFromTemplate(ConfigurationManager.AppSettings["mailBodyHtmlTemplate"]);

            foreach (string address in laddress)
            {
                MailMessage mail = new MailMessage(
                                     new MailAddress(address),
                                     new MailAddress(address)
                                   );
                string[] addresspart = address.Split('@');

                mail.Subject = addresspart[0];
                mail.IsBodyHtml = true;
                mail.Body = mailbody.Replace("[:RECEPIENT:]", addresspart[0]);
                mail.Attachments.Add(new Attachment(img_file));
                mail.Attachments[0].ContentId = "logo.png";

                lmail.Add(mail);
            }

            mailing.Mail = lmail;
            mailing.SendHost = new SendServer(
                                 mailsettings.Smtp.Network.Host,
                                 (int)mailsettings.Smtp.Network.Port,
                                 mailsettings.Smtp.Network.UserName,
                                 mailsettings.Smtp.Network.Password
                               );

            string msg = mailing.SendMails();

            Assert.IsEmpty(msg);
        }
    }
}
