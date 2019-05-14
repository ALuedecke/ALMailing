using EASendMail;

namespace ALMailing
{
    public enum SvrConnType
    {
        NORMAL = SmtpConnectType.ConnectNormal,
        SSLAUTO = SmtpConnectType.ConnectSSLAuto,
        STARTTLS = SmtpConnectType.ConnectSTARTTLS,
        SSLDIRECT = SmtpConnectType.ConnectDirectSSL,
        TLS = SmtpConnectType.ConnectTryTLS
    }
}
