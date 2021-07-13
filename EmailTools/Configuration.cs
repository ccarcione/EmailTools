namespace EmailTools
{
    public class Configuration
    {
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public bool UseSsl { get; set; }
        public string Mittente { get; set; }
        public string SmtpUsername { get; set; }
        public string SmtpPassword { get; set; }
        public bool Authenticate { get; set; }
        public string AliasName { get; set; }
        public bool EnableRecoveryEmail { get; set; }
        public string RecoveryEmailPath { get; set; }
        public bool EnableBackupSendEmail { get; set; }
        public string BackupSendEmailPath { get; set; }
    }
}
