using System.Collections.Generic;
using System.Threading.Tasks;

namespace EmailTools
{
    public interface IEmailTools
    {
        Task SendEmailAsync(IEnumerable<string> emailsTo, string subject, string message);
        Task SendEmailAsync(IEnumerable<string> emailsTo, string subject, string message, Options options);
    }
}
