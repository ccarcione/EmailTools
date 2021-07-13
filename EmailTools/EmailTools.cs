using MimeKit;
using MimeKit.Text;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace EmailTools
{
    /// <summary>
    /// http://www.mimekit.net/docs/html/Creating-Messages.htm
    /// </summary>
    public class EmailTools
    {
        private readonly Configuration _configuration;

        public EmailTools(string jsonConfiguration)
        {
            _configuration = JsonConvert.DeserializeObject<Configuration>(jsonConfiguration);
        }

        public EmailTools(Configuration configuration)
        {
            _configuration = configuration;
        }

        public async Task SendEmailAsync(IEnumerable<string> emailsTo, string subject, string message)
        {
            MimeMessage mimeMsg = Utility.PrepareToFromSubject(_configuration, emailsTo, subject, message);

            //We will say we are sending HTML. But there are options for plaintext etc. 
            mimeMsg.Body = new TextPart(TextFormat.Html)
            {
                Text = message
            };

            await SendEmail(mimeMsg);
        }

        public async Task SendEmailAsync(IEnumerable<string> emailsTo, string subject, string message, Options options)
        {
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }

            MimeMessage mimeMsg = Utility.PrepareToFromSubject(_configuration, emailsTo, subject, message);

            if (options.CCEmails != null && options.CCEmails.Any())
            {
                mimeMsg.Cc.AddRange(Utility.MailboxAddressBuilder(options.CCEmails));
            }

            if (options.BCCEmails != null && options.BCCEmails.Any())
            {
                mimeMsg.Bcc.AddRange(Utility.MailboxAddressBuilder(options.BCCEmails));
            }

            // create our message text, just like before (except don't set it as the message.Body)
            TextPart body = new TextPart(TextFormat.Plain) { Text = message };

            if (!options.AttachmentsFilePath.Any() && !options.AttachmentsStream.Any())
            {
                //We will say we are sending HTML. But there are options for plaintext etc. 
                mimeMsg.Body = new TextPart(TextFormat.Html)
                {
                    Text = message
                };
            }
            else
            {
                // controllo e inserisco allegati

                // now create the multipart/mixed container to hold the message text and the attachment.
                Multipart multipart = new Multipart("mixed");
                multipart.Add(body);

                options.AttachmentsFilePath.ForEach(f =>
                {
                    // create an attachment for the file located at path.
                    MimePart attachmentToAdd = new MimePart(Utility.GetMimeType(Path.GetExtension(f)))
                    {
                        Content = new MimeContent(File.OpenRead(f)),
                        ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                        ContentTransferEncoding = ContentEncoding.Base64,
                        FileName = Path.GetFileName(f)
                    };

                    multipart.Add(attachmentToAdd);
                });

                options.AttachmentsStream.ForEach(f =>
                {
                    // create an attachment for the file located at path.
                    MimePart attachmentToAdd = new MimePart(Utility.GetMimeType(Path.GetExtension(f.Item1)))
                    {
                        Content = new MimeContent(f.Item2),
                        ContentDisposition = new ContentDisposition(ContentDisposition.Attachment),
                        ContentTransferEncoding = ContentEncoding.Base64,
                        FileName = Path.GetFileName(f.Item1)
                    };

                    multipart.Add(attachmentToAdd);
                });


                // now set the multipart/mixed as the message body
                mimeMsg.Body = multipart;
            }

            if (!string.IsNullOrWhiteSpace(options.AliasName))
            {
                mimeMsg.From[0].Name = options.AliasName;
            }

            await SendEmail(mimeMsg);
        }

        private async Task SendEmail(MimeMessage mimeMsg)
        {
            try
            {
                //Be careful that the SmtpClient class is the one from Mailkit not the framework!
                using (MailKit.Net.Smtp.SmtpClient emailClient = new MailKit.Net.Smtp.SmtpClient())
                {
                    //The last parameter here is to use SSL (Which you should!)
                    await emailClient.ConnectAsync(_configuration.SmtpServer, _configuration.SmtpPort, _configuration.UseSsl);

                    //Remove any OAuth functionality as we won't be using it. 
                    emailClient.AuthenticationMechanisms.Remove("XOAUTH2");

                    if (_configuration.Authenticate)
                    {
                        await emailClient.AuthenticateAsync(_configuration.SmtpUsername, _configuration.SmtpPassword);
                    }

                    await emailClient.SendAsync(mimeMsg);

                    await emailClient.DisconnectAsync(true);

                    if (_configuration.EnableBackupSendEmail)
                    {
                        Utility.SaveEmailToPath(mimeMsg, _configuration.BackupSendEmailPath);
                    }
                }
            }
            catch (Exception)
            {
                if (_configuration.EnableRecoveryEmail)
                {
                    Utility.SaveEmailToPath(mimeMsg, _configuration.RecoveryEmailPath);
                }
                throw;
            }

        }
    }
}
