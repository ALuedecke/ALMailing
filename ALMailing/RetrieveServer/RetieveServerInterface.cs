using System.Collections.ObjectModel;

namespace ALMailing
{
    public interface RetieveServerInterface
    {
        #region Properties
        string HostName { get; set; }
        int Port { get; set; }
        string NetworkUser { get; set; }
        string NetworkPassword { get; set; }
        bool UseSsl { get; set; }
        #endregion

        #region Methods
        Collection<Email> RetrieveMailsIMAP();
        Collection<Email> RetrieveMailsPOP3();
        #endregion
        bool DeleteMailOnServer(string uid);
    }
}
