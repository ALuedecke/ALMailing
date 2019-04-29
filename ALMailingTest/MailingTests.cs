using ALMailing;
using NUnit.Framework;
using System.Configuration;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;
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
            NetworkCredential expCredential = mailing.Credential;
            SmtpClient expSmtp = mailing.Smtp;

            Assert.AreEqual(expSmtp, mailing.Smtp);
            Assert.AreEqual (expCredential, mailing.Credential);
            Assert.IsEmpty(mailing.Mail);
        }

        [Test]
        public void CreateMailingWithParameters()
        {
            SmtpClient smtp = new SmtpClient("smtp.server.xy");
            NetworkCredential credential = new NetworkCredential("user.name@domain.xy", "password");
            MailMessage mail = new MailMessage("user1.name@domain.xy", "user2.name@domain.xy");

            Mailing mailing = new Mailing(smtp, credential, mail);

            Assert.AreEqual(mailing.Smtp, smtp);
            Assert.AreEqual(mailing.Credential, credential);
            Assert.AreEqual(mailing.Mail.First(), mail);
        }

        [Test]
        public void Mailing_Set_SmptServer()
        {
            Mailing mailing = new Mailing();
            SmtpClient smtp = new SmtpClient();

            mailing.Smtp = smtp;

            Assert.AreEqual(mailing.Smtp, smtp);
        }

        [Test]
        public void Mailing_Set_SmptServer_Credential()
        {
            Mailing mailing = new Mailing();
            NetworkCredential credential = new NetworkCredential("user.name@domain.xy", "password");
            SmtpClient smtp = new SmtpClient("smtp.server1.xy");

            mailing.Smtp = smtp;
            mailing.Credential = credential;

            Assert.AreEqual(mailing.Smtp, smtp);
            Assert.AreEqual(mailing.Credential, credential);
        }

        [Test]
        public void Mailing_Set_SmptServer_Credential_Add_2_Mails()
        {
            Mailing mailing = new Mailing();
            NetworkCredential credential = new NetworkCredential("user.name@domain.xy", "password");
            List<MailMessage> lmail = new List<MailMessage>();
            SmtpClient smtp = new SmtpClient("smtp.server1.xy");

            lmail.Add(new MailMessage("user1.name@domain.xy", "user2.name@domain.xy"));
            lmail.Add(new MailMessage("user1.name@domain.yz", "user2.name@domain.yz"));
            mailing.Smtp = smtp;
            mailing.Credential = credential;
            mailing.Mail = lmail;

            Assert.AreEqual(mailing.Smtp, smtp);
            Assert.AreEqual(mailing.Credential, credential);
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
                                 new MailAddress(mailsettings.Smtp.From)
                               );
            SmtpClient smtp = new SmtpClient(
                                mailsettings.Smtp.Network.Host,
                                (int) mailsettings.Smtp.Network.Port
                              );

            mail.Subject = ConfigurationManager.AppSettings["mailSubject"];
            mail.Body = mailing.GetMailBodyFromTemplate(ConfigurationManager.AppSettings["mailBodyTxtTemplate"]);
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(
                                 mailsettings.Smtp.Network.UserName,
                                 mailsettings.Smtp.Network.Password
                               );

            string msg = mailing.SendSingleMail(smtp, mail);

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
                                 new MailAddress(mailsettings.Smtp.From)
                               );
            SmtpClient smtp = new SmtpClient(
                                mailsettings.Smtp.Network.Host,
                                (int) mailsettings.Smtp.Network.Port
                              );

            string[] addresspart = mailsettings.Smtp.From.Split('@');

            mail.Subject = ConfigurationManager.AppSettings["mailSubject"];
            mail.IsBodyHtml = true;
            mail.Body = mailing.GetMailBodyFromTemplate(ConfigurationManager.AppSettings["mailBodyHtmlTemplate"]);
            mail.Body = mail.Body.Replace("[:RECEPIENT:]", addresspart[0]);
            mail.Attachments.Add(new Attachment(ConfigurationManager.AppSettings["mailBodyImageFile"]));
            mail.Attachments[0].ContentId = "logo.png";
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(
                                 mailsettings.Smtp.Network.UserName,
                                 mailsettings.Smtp.Network.Password
                               );

            string msg = mailing.SendSingleMail(smtp, mail);

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
            mailing.Credential = new NetworkCredential(
                                   mailsettings.Smtp.Network.UserName,
                                   mailsettings.Smtp.Network.Password
                                 );
            mailing.Smtp = new SmtpClient(
                             mailsettings.Smtp.Network.Host,
                             (int) mailsettings.Smtp.Network.Port
                           );
            mailing.Smtp.UseDefaultCredentials = false;

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
                "andreas.luedecke@kontacts.de"
                /*"alfred.liesecke@kontacts.de",
                "a_luedecke@gmx.de",
                "a.luedecke4@gmail.com";
                "c.kapella@freenet.de"
                "andreas.penzold@kontacts.de"
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

            mailing.Credential = new NetworkCredential(
                                   mailsettings.Smtp.Network.UserName,
                                   mailsettings.Smtp.Network.Password
                                 );
            mailing.Mail = lmail;
            mailing.Smtp = new SmtpClient(
                                 mailsettings.Smtp.Network.Host,
                                 (int)mailsettings.Smtp.Network.Port
                               );
            mailing.Smtp.UseDefaultCredentials = false;

            string msg = mailing.SendMails();

            Assert.IsEmpty(msg);
        }
    }
}
