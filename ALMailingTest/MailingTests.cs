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

            mail.Subject = "Deine Geräte wurden gehackt!";
            mail.Body = "An den Möchtegern-Hacker,\n\n" +
                        "du glaubst mit dieser Masche groß in Bitcoins abkassieren zu können, täusch dich mal nicht.\n" +
                        "Solche Aktionen können voll in die Hose gehen und dann ist das Geschrei groß.\n" +
                        "Du bist entlarvt und es wurde Anzeige erstattet. Freue dich auf den Besuch eines Sondereinsatzkommandos.\n\n\n Der Anti-Hacker";
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
            MailSettingsSectionGroup mailsettings = (MailSettingsSectionGroup)config.GetSectionGroup("system.net/mailSettings");
            Mailing mailing = new Mailing();
            MailMessage mail = new MailMessage(
                                 new MailAddress(mailsettings.Smtp.From),
                                 new MailAddress(mailsettings.Smtp.From)
                               );
            SmtpClient smtp = new SmtpClient(
                                mailsettings.Smtp.Network.Host,
                                (int) mailsettings.Smtp.Network.Port
                              );
            string img_file = "C:\\Users\\LuedeckeA\\Pictures\\Saved Pictures\\logo.png";
            mail.Subject = "Du wurdest gehackt!";
            mail.IsBodyHtml = true;
            mail.Body = "<html>" +
                        "<body>" +
                        "<div>" +
                        "An den Möchtegern-Hacker,<br /><br />" +
                        "du glaubst mit dieser Masche groß in Bitcoins abkassieren zu können, täusch dich mal nicht.<br />" +
                        "Solche Aktionen können voll in die Hose gehen und dann ist das Geschrei groß.<br />" +
                        "Du bist entlarvt und es wurde Anzeige erstattet. Freue dich auf den Besuch eines Sondereinsatzkommandos.<br /><br />" +
                        "<span style=\"color:#800000; font-weight:bold;\">Der Anti-Hacker</span>" +
                        "</div>" +
                        "<img src=\"cid:logo.png\" width=\"100\">" +
                        "</body>" +
                        "</html>";
            mail.Attachments.Add(new Attachment(img_file));
            mail.Attachments[0].ContentId = "logo.png";
            smtp.UseDefaultCredentials = false;
            smtp.Credentials = new NetworkCredential(
                                 mailsettings.Smtp.Network.UserName,
                                 mailsettings.Smtp.Network.Password
                               );

            string msg = mailing.SendSingleMail(smtp, mail);

            Assert.IsEmpty(msg);
        }

        [TestCase("andreas.luedecke@kontacts.de", "")]
        [TestCase("a_luedecke@gmx.de", "")]
        [TestCase("a.luedecke4@gmail.com", "")]
        public void MailingSendSingleMailHtmlWithInput(string mailaddress, string expected)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            MailSettingsSectionGroup mailsettings = (MailSettingsSectionGroup)config.GetSectionGroup("system.net/mailSettings");
            Mailing mailing = new Mailing();
            MailMessage mail = new MailMessage(
                                 new MailAddress(mailaddress),
                                 new MailAddress(mailaddress)
                               );
            SmtpClient smtp = new SmtpClient(
                                mailsettings.Smtp.Network.Host,
                                (int) mailsettings.Smtp.Network.Port
                              );
            string img_file = "C:\\Users\\LuedeckeA\\Pictures\\Saved Pictures\\logo.png";
            mail.Subject = "Du wurdest gehackt!";
            mail.IsBodyHtml = true;
            mail.Body = "<html>" +
                        "<body>" +
                        "<div>" +
                        "An den Möchtegern-Hacker,<br /><br />" +
                        "du glaubst mit dieser Masche groß in Bitcoins abkassieren zu können, täusch dich mal nicht.<br />" +
                        "Solche Aktionen können voll in die Hose gehen und dann ist das Geschrei groß.<br />" +
                        "Du bist entlarvt und es wurde Anzeige erstattet. Freue dich auf den Besuch eines Sondereinsatzkommandos.<br /><br />" +
                        "<span style=\"color:#800000; font-weight:bold;\">Der Anti-Hacker</span>" +
                        "</div>" +
                        "<img src=\"cid:logo.png\" width=\"100\">" +
                        "</body>" +
                        "</html>";
            mail.Attachments.Add(new Attachment(img_file));
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
                //"alfred.liesecke@kontacts.de",
                "andreas.luedecke@kontacts.de",
                "a_luedecke@gmx.de",
                "a.luedecke4@gmail.com"
                /*"andreas.penzold@kontacts.de",
                "azamat.khasanov@kontacts.de",
                "ettker@posteo.de",
                "michael.kickmunter@kontacts.de",
                "sl@kontacts.de"*/
            };
            List<MailMessage> lmail = new List<MailMessage>();
            MailSettingsSectionGroup mailsettings = (MailSettingsSectionGroup)config.GetSectionGroup("system.net/mailSettings");
            Mailing mailing = new Mailing();
            string img_file = "C:\\Users\\LuedeckeA\\Pictures\\Saved Pictures\\logo.png";

            foreach (string address in laddress)
            {
                MailMessage mail = new MailMessage(
                                     new MailAddress(address),
                                     new MailAddress(address)
                                   );
                mail.Subject = "Du wurdest gehackt!";
                mail.IsBodyHtml = true;
                mail.Body = "<html>" +
                            "<body>" +
                            "<div>" +
                            "An den Möchtegern-Hacker,<br /><br />" +
                            "du glaubst mit dieser Masche groß in Bitcoins abkassieren zu können, täusch dich mal nicht.<br />" +
                            "Solche Aktionen können voll in die Hose gehen und dann ist das Geschrei groß.<br />" +
                            "Du bist entlarvt und es wurde Anzeige erstattet. Freue dich auf den Besuch eines Sondereinsatzkommandos.<br /><br />" +
                            "<span style=\"color:#800000; font-weight:bold;\">Der Anti-Hacker</span>" +
                            "</div>" +
                            "<img src=\"cid:logo.png\" width=\"100\">" +
                            "</body>" +
                            "</html>";
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
