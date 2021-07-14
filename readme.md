# Email Tools

Utility library for e-mail.
- simple sending e-mail
- sending e-mail with multiple attachments

**repository link --> https://gitlab.com/projects-experimenta/emailtools**

## Getting started

EmailTools is installed from NuGet.

In your _appsettings.json_ file, under the _EmailTools_ node:

```json
{
  "EmailTools": {
    "SmtpServer": "smtp.example.com",
    "SmtpPort": 25,
    "UseSsl": false,
    "Mittente": "email@example.com",
    "SmtpUsername": "email@example.com",
    "SmtpPassword": "password",
    "Authenticate": true,
    "AliasName": "AAA",
    // saves e-mails not sent due to errors.
    "EnableRecoveryEmail": false,
    "RecoveryEmailPath": "email/error",
    // save all sent emails.
    "EnableBackupSendEmail": false,
    "BackupSendEmailPath": "email/backup"
  }
}
```

The easiest way to use _Email tools_ is to get the configuration from json file and use the _new_ operator

```c#
var builder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false);
IConfiguration config = builder.Build();

var emailTools = new ETools(config.GetSection("EmailTools").Get<Configuration>());

```

and use it like that:

```c#
Options options = new Options();
options.AttachmentsStream.Add(new Tuple<string, Stream>("attachmentExample.txt", File.OpenRead("attachmentExample.txt")));
options.AttachmentsStream.Add(new Tuple<string, Stream>("att1.txt", File.OpenRead("att1.txt")));
options.AttachmentsStream.Add(new Tuple<string, Stream>("att2.txt", File.OpenRead("att2.txt")));

await _emailTools.SendEmailAsync(
    emailsTo: _emailDestinatario,
    subject: $"email di test con allegato {DateTime.Now.ToString()}",
    message: $@"{System.Environment.NewLine} CANCELLA IMMEDIATAMENTE!!!!!",
    options);
```
attachments can also be added via their path:

```c#
Options options = new Options();
options.AttachmentsFilePath.Add("att1.txt");
options.AttachmentsFilePath.Add("att2.txt");
options.AttachmentsFilePath.Add("attachmentExample.txt");
```

## Backup and Recovery

- enabling the flag **EnableBackupSendEmail** all sent emails will be saved in the configured directory
- enabling the flag **EnableRecoveryEmail** in case of errors the email is saved in the configured directory

## Contributing / Help

For any doubt or problem consult and/or open an issue here --> https://gitlab.com/projects-experimenta/emailtools/-/issues
