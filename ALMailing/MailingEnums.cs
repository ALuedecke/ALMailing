using EASendMail;

namespace ALMailing
{
    public enum RetrieveType
    {
        IMAP,
        POP3
    }

    public enum SvrConnType
    {
        NORMAL = SmtpConnectType.ConnectNormal,
        SSLAUTO = SmtpConnectType.ConnectSSLAuto,
        STARTTLS = SmtpConnectType.ConnectSTARTTLS,
        SSLDIRECT = SmtpConnectType.ConnectDirectSSL,
        TLS = SmtpConnectType.ConnectTryTLS
    }
}
