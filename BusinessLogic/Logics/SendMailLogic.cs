using System.Net.Mail;
using BusinessLogic.Services;
using DataAccessObject.Models;
using SystemConfig = BusinessLogic.Utils.SystemConfig;

namespace BusinessLogic.Logics;

/// <summary>
/// Send mail config.
/// </summary>
public static class SendMailLogic
{
   
    /// <summary>
    /// Config send mail
    /// </summary>
    /// <param name="mailAddress"></param>
    /// <param name="title"></param>
    /// <param name="body"></param>
    /// <param name="systemConfig"></param>
    public static void Send(string mailAddress, string title, string body, IBaseService<DataAccessObject.Models.SystemConfig, string, VwSystemConfig> systemConfig)
    {
        var configs = systemConfig.Find().ToList();
        // The SMTP server is obtained from the system configuration (SYSTEM_CONFIG).
        var mailSmtp = configs.Find(c => c!.Id == SystemConfig.MailSmtp)?.Value;
        // The sender of the email is obtained from the system configuration (SYSTEM_CONFIG).
        var mailFrom = configs.Find(c => c!.Id == SystemConfig.MailFrom)?.Value;
        // The BCC email sender is obtained from the system configuration (SYSTEM_CONFIG).
        var mailToBcc = configs.Find(c => c!.Id == SystemConfig.MailToBcc)?.Value;
        // The password for the email is obtained from the system configuration (SYSTEM_CONFIG).
        var mailPassword = configs.Find(c => c!.Id == SystemConfig.MailPassword)?.Value;
        
        var client = new SmtpClient
        {
            Host = mailSmtp!,
            Port = 587,
            EnableSsl = true,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Credentials = new System.Net.NetworkCredential(mailFrom, mailPassword)
        };

        // Generate a message instance and set parameters.
        using (var message = new MailMessage())
        {
            message.From = new MailAddress(mailFrom!);
            message.To.Add(mailAddress);
            message.Subject = title;
            message.Body = body;
            message.Bcc.Add(mailToBcc!);
            message.IsBodyHtml = true;
            client.Send(message);
        }
    }

   
    /// <summary>
    /// Send mail
    /// </summary>
    /// <param name="emailTemplate"></param>
    /// <param name="mailAddress"></param>
    /// <param name="systemConfig"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static bool SendMail(EmailTemplate emailTemplate, string mailAddress, IBaseService<DataAccessObject.Models.SystemConfig, string, VwSystemConfig> systemConfig)
    {
        if (string.IsNullOrEmpty(emailTemplate.Title))
        {
            throw new Exception();
        }
        if (string.IsNullOrEmpty(emailTemplate.Body))
        {
            throw new Exception();
        }
        if (string.IsNullOrEmpty(mailAddress))
        {
            throw new Exception();
        }

        try
        {
            // Send mail
            Send(mailAddress, emailTemplate.Title, emailTemplate.Body, systemConfig);
            return true;
        }
        catch (Exception e)
        {
            throw e;
        }
    }
}