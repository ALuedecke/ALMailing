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

            Assert.IsEmpty(mailing.Smptp);
            Assert.IsEmpty(mailing.Credential);
            Assert.IsEmpty(mailing.Mail);
        }

        [Test]
        public void CreateMailingWithParameters()
        {
            SmtpClient smtp = new SmtpClient("smtp.server.xy");
            NetworkCredential credential = new NetworkCredential("user.name@domain.xy", "password");
            MailMessage mail = new MailMessage("user1.name@domain.xy", "user2.name@domain.xy");

            Mailing mailing = new Mailing(smtp, credential, mail);

            Assert.AreEqual(mailing.Smptp.First(), smtp);
            Assert.AreEqual(mailing.Credential.First(), credential);
            Assert.AreEqual(mailing.Mail.First(), mail);
        }

        [Test]
        public void MailingAdd_1_SmptServer()
        {
            Mailing mailing = new Mailing();
            List<SmtpClient> lsmtp = new List<SmtpClient>();

            lsmtp.Add(new SmtpClient("smtp.server.xy"));
            mailing.Smptp = lsmtp;

            Assert.AreEqual(mailing.Smptp, lsmtp);
        }

        [Test]
        public void MailingAdd_2_SmptServers()
        {
            Mailing mailing = new Mailing();
            List<SmtpClient> lsmtp = new List<SmtpClient>();

            lsmtp.Add(new SmtpClient("smtp.server1.xy"));
            lsmtp.Add(new SmtpClient("smtp.server2.xy"));
            mailing.Smptp = lsmtp;

            Assert.AreEqual(mailing.Smptp, lsmtp);
        }

        [Test]
        public void MailingAdd_1_SmptServer_1_Credential()
        {
            Mailing mailing = new Mailing();
            List<SmtpClient> lsmtp = new List<SmtpClient>();
            List<NetworkCredential> lcredential = new List<NetworkCredential>();

            lsmtp.Add(new SmtpClient("smtp.server1.xy"));
            lcredential.Add(new NetworkCredential("user.name@domain.xy", "password"));
            mailing.Smptp = lsmtp;
            mailing.Credential = lcredential;

            Assert.AreEqual(mailing.Smptp, lsmtp);
            Assert.AreEqual(mailing.Credential, lcredential);
        }

        [Test]
        public void MailingAdd_1_SmptServer_1_Credential_2_Mails()
        {
            Mailing mailing = new Mailing();
            List<SmtpClient> lsmtp = new List<SmtpClient>();
            List<NetworkCredential> lcredential = new List<NetworkCredential>();
            List<MailMessage> lmail = new List<MailMessage>();

            lsmtp.Add(new SmtpClient("smtp.server1.xy"));
            lcredential.Add(new NetworkCredential("user.name@domain.xy", "password"));
            lmail.Add(new MailMessage("user1.name@domain.xy", "user2.name@domain.xy"));
            lmail.Add(new MailMessage("user1.name@domain.yz", "user2.name@domain.yz"));
            mailing.Smptp = lsmtp;
            mailing.Credential = lcredential;
            mailing.Mail = lmail;

            Assert.AreEqual(mailing.Smptp, lsmtp);
            Assert.AreEqual(mailing.Credential, lcredential);
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
        [TestCase("anti.hacker@mailcontrol.com", "a_luedecke@gmx.de", "")]
        [TestCase("anti.hacker@mailcontrol.com", "a.luedecke4@gmail.com", "")]
        public void MailingSendSingleMailHtmlWithInput(string from, string to, string expected)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            MailSettingsSectionGroup mailsettings = (MailSettingsSectionGroup)config.GetSectionGroup("system.net/mailSettings");
            Mailing mailing = new Mailing();
            MailMessage mail = new MailMessage(
                                 new MailAddress(from),
                                 new MailAddress(to)
                               );
            SmtpClient smtp = new SmtpClient(
                                mailsettings.Smtp.Network.Host,
                                (int) mailsettings.Smtp.Network.Port
                              );

            string[] addresspart = to.Split('@');

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

            Assert.AreEqual(expected, msg);
        }

        [Test]
        public void MailingSendMailsHtml()
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            List<string> laddress = new List<string>()
            {
                "alfred.liesecke@kontacts.de",
                "andreas.luedecke@kontacts.de",
                /*"a_luedecke@gmx.de",
                "a.luedecke4@gmail.com",
                "c.kapella@freenet.de"
                "andreas.penzold@kontacts.de",*/
                "azamat.khasanov@kontacts.de",
                "ettker@posteo.de",
                "michael.kickmunter@kontacts.de",
                "sl@kontacts.de"
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

            SmtpClient smtp = new SmtpClient(
                                mailsettings.Smtp.Network.Host,
                                (int) mailsettings.Smtp.Network.Port
                              );
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(
                                 mailsettings.Smtp.Network.UserName,
                                 mailsettings.Smtp.Network.Password
                               );

            string msg = mailing.SendMails(smtp, lmail);

            Assert.IsEmpty(msg);
        }
    }
}
