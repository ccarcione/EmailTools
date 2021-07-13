using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace EmailTools.ConsoleApp
{
    class Program
    {
        private static EmailTools _emailTools;
        private static List<string> _emailDestinatario = new List<string>() { "carcione.christian@gmail.com" };

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false);
            IConfiguration config = builder.Build();

            _emailTools = new EmailTools(config.GetSection("EmailTools").Get<Configuration>());
            SendEmailAsyncExample().GetAwaiter().GetResult();
            SendEmailAsyncWithAttachmentExample().GetAwaiter().GetResult();
            SendEmailAsyncWithAttachmentStreamExample().GetAwaiter().GetResult();
        }

        private static async Task SendEmailAsyncWithAttachmentStreamExample()
        {
            try
            {
                Options options = new Options();
                options.AttachmentsStream.Add(new Tuple<string, Stream>("attachmentExample.txt", File.OpenRead("attachmentExample.txt")));
                options.AttachmentsStream.Add(new Tuple<string, Stream>("att1.txt", File.OpenRead("att1.txt")));
                options.AttachmentsStream.Add(new Tuple<string, Stream>("att2.txt", File.OpenRead("att2.txt")));
                // invio notifica email con la lista appena importata.
                await _emailTools.SendEmailAsync(
                    emailsTo: _emailDestinatario,
                    subject: $"email di test con allegato {DateTime.Now.ToString()}",
                    message: $@"{System.Environment.NewLine} CANCELLA IMMEDIATAMENTE!!!!!",
                    options);
                Console.WriteLine("Email con allegati (Stream) inviata");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Errore: {e}");
                throw;
            }
        }

        private static async Task SendEmailAsyncExample()
        {
            try
            {
                // invio notifica email con la lista appena importata.
                await _emailTools.SendEmailAsync(
                    emailsTo: _emailDestinatario,
                    subject: $"semplice email di test {DateTime.Now.ToString()}",
                    message: $@"{System.Environment.NewLine} CANCELLA IMMEDIATAMENTE!!!!!");
                Console.WriteLine("Email inviata");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Errore: {e}");
                throw;
            }
        }

        private static async Task SendEmailAsyncWithAttachmentExample()
        {
            try
            {
                Options options = new Options();
                options.AttachmentsFilePath.Add("att1.txt");
                options.AttachmentsFilePath.Add("att2.txt");
                options.AttachmentsFilePath.Add("attachmentExample.txt");
                // invio notifica email con la lista appena importata.
                await _emailTools.SendEmailAsync(
                    emailsTo: _emailDestinatario,
                    subject: $"email di test con allegato {DateTime.Now.ToString()}",
                    message: $@"{System.Environment.NewLine} CANCELLA IMMEDIATAMENTE!!!!!",
                    options);
                Console.WriteLine("Email con allegati inviata");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Errore: {e}");
                throw;
            }
        }
    }
}
