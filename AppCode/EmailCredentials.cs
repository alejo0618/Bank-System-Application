using System;
using System.Collections.Generic;
using System.Text;

namespace Bank_System_Application
{
    /// <summary>
    /// Simple class which stores the email credentials in a separate file.
    /// In order to keep things simple, this class should not be upload to the repository
    /// However, the actions recommended to store sensitive information in .NET app are:
    /// https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets?view=aspnetcore-5.0&tabs=windows
    /// </summary>
    public class EmailCredentials
    {
        public static string SmtpClient { get; } = "smtp.gmail.com";
        public static string MailAddress { get; } = "your_email@gmail.com";
        public static string Mail { get; } = "your_email";
        public static string Password { get; } = "P4SsW0r4!";
        public static int Port { get; } = 587;
    }
}
