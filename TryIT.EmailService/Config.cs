using System;

namespace TryIT.EmailService
{
    public class EmailConfig
    {
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public bool EnableSsl {get; set;}
        public string FromAddress { get; set; }
    }

    public class Config
    {
        internal static EmailConfig CurrentConfig;
        
        internal static string SmtpServer
        {
            get
            {
                if (CurrentConfig == null)
                {
                    throw new ArgumentNullException("EmailConfig is null, please call Configure method in Startup.cs");
                }

                if (string.IsNullOrEmpty(CurrentConfig.SmtpServer))
                {
                    throw new ArgumentNullException("SmtpServer is invalid, please call Configure method in Startup.cs with valid Smtp Server");
                }

                return CurrentConfig.SmtpServer;
            }
        }
        internal static int SmtpPort
        {
            get
            {
                if (CurrentConfig == null)
                {
                    throw new ArgumentNullException("EmailConfig is null, please call Configure method in Startup.cs");
                }

                if (CurrentConfig.SmtpPort == 0)
                {
                    throw new ArgumentNullException("SmtpPort is invalid, please call Configure method in Startup.cs with valid Smtp Port");
                }

                return CurrentConfig.SmtpPort;
            }
        }
        internal static bool EnableSsl
        {
            get
            {
                if (CurrentConfig == null)
                {
                    throw new ArgumentNullException("EmailConfig is null, please call Configure method in Startup.cs");
                }

                return CurrentConfig.EnableSsl;
            }
        }
        internal static string FromAddress
        {
            get
            {
                if (CurrentConfig != null && !string.IsNullOrEmpty(CurrentConfig.FromAddress))
                {
                    return CurrentConfig.FromAddress;
                }
                return string.Empty;
            }
        }
        private static string _queryPassword = string.Empty;

        public static void Configure(EmailConfig emailConfig)
        {
            CurrentConfig = emailConfig;
        }
    }
}
