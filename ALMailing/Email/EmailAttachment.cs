namespace ALMailing
{
    public class EmailAttachment
    {
        #region Properties
        public string Path { get; set; }
        public string ContentId { get; set; }
        public byte[] Data { get; set; }
        #endregion

        #region Constructors
        public EmailAttachment()
        {
            InitClass("", "");
        }
        public EmailAttachment(string path)
        {
            InitClass(path, "");
        }
        public EmailAttachment(string path, string contentid)
        {
            InitClass(path, contentid);
        }
        #endregion

        #region Private methods
        private void InitClass(string path, string contentid)
        {
            Path = path;
            ContentId = contentid;
        }
        #endregion
    }
}
