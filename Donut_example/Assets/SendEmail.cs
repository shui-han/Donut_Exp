using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

public class SendEmail : MonoBehaviour
{

        public void Emailer(string filepath)
        {
          
            MailMessage mail = new MailMessage();

            mail.From = new MailAddress("experiments.in.quest@gmail.com");
            mail.To.Add("experiments.in.quest@gmail.com"); // mailAddress
            mail.Subject = "data from quest headset";
            mail.Body = "see attached textfile";

            
            Attachment attachement = new Attachment(filepath);
            mail.Attachments.Add(attachement);

            SmtpClient smtp = new SmtpClient("smtp.gmail.com");
            smtp.Port = 587;
            smtp.Timeout = 1000;
            smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
            smtp.UseDefaultCredentials = false;

            smtp.EnableSsl = true;
            smtp.Credentials = new System.Net.NetworkCredential("experiments.in.quest@gmail.com", "tadinlab20") as ICredentialsByHost;

            ServicePointManager.ServerCertificateValidationCallback = delegate (object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors) { return true; };

            smtp.Send(mail);
    }
    
}
