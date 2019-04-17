using ALMailing;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net;

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
            Mailing mailing = new Mailing();
            SmtpClient smtp = new SmtpClient("smtp.1und1.de", 587);
            MailMessage mail = new MailMessage("a_luedecke@gmx.de", "a_luedecke@gmx.de");

            mail.Subject = "Dein Account wurde gehackt!";
            mail.Body = "Irgend so ein blöder Wichser denkt er könnte groß in Bitcoins abkassieren, " +
                        "dass er sich da mal nicht täuscht.\n" +
                        "Solche Aktionen können voll in die Hose gehen " +
                        "und dann ist das Geschrei groß.\n\n\n Der Anti-Hacker";
            smtp.Credentials = new NetworkCredential("andreas.luedecke@kontacts.de", "L@ydas!004");

            string msg = mailing.SendSingleMail(smtp, mail);

            Assert.IsEmpty(msg);
        }
    }
}
