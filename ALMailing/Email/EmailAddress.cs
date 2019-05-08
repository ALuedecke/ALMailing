namespace ALMailing
{
    public class EmailAddress
    {
        #region Properties
        public string Address { get; set; }
        public string DisplayName { get; set; }
        #endregion

        #region Constructors
        public EmailAddress()
        {
            InitClass("", "");
        }

        public EmailAddress(string address)
        {
            InitClass(address, "");
        }

        public EmailAddress(string address, string displayname)
        {
            InitClass(address, displayname);
        }
        #endregion

        #region Private methods
        private void InitClass(string address, string displayname)
        {
            Address = address;
            DisplayName = displayname;
        }
        #endregion
    }
}
