using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALMailing
{
    public class RetrieveServer : RetieveServerInterface
    {
        #region Properties
        public string HostName { get; set; }
        public int Port { get; set; }
        public string NetworkUser { get; set; }
        public string NetworkPassword { get; set; }
        public bool UseSsl { get; set; }
        #endregion

        #region Constructors
        public RetrieveServer()
        {
            InitClass("", 0, "", "", false);
        }

        public RetrieveServer(string host)
        {
            InitClass(host, 995, "", "", false);
        }

        public RetrieveServer(string host, string username, string password)
        {
            InitClass(host, 995, username, password, false);
        }

        public RetrieveServer(string host, int port, string username, string password)
        {
            InitClass(host, port, username, password, false);
        }

        public RetrieveServer(string host, int port, string username, string password, bool usessl)
        {
            InitClass(host, port, username, password, usessl);
        }
        #endregion

        #region Public methods
        public bool DeleteMailOnServer(int index)
        {
            throw new NotImplementedException();
        }

        public Collection<Email> RetrieveMails()
        {
            Collection<Email> retmails = new Collection<Email>();
            //POP3Client pop3;
            return retmails;
        }
        #endregion

        #region Private methods
        private void CheckProperties()
        {
            bool throw_ex = false;
            string msg = "";

            if (HostName.Equals(String.Empty))
            {
                msg += "HostName cannot be null or empty!\n";
                throw_ex = true;
            }
            if (NetworkUser.Equals(String.Empty))
            {
                msg += "NetworkUser cannot be null or empty!\n";
                throw_ex = true;

            }
            if (NetworkPassword.Equals(String.Empty))
            {
                msg += "NetworkPassword cannot be null or empty!\n";
                throw_ex = true;

            }
            if (throw_ex)
            {
                throw new NullReferenceException(msg);
            }
        }

        private void InitClass(string host, int port, string username, string password, bool usessl)
        {
            HostName = host;
            Port = port;
            NetworkUser = username;
            NetworkPassword = password;
            UseSsl = usessl;
        }
        #endregion
    }
}
