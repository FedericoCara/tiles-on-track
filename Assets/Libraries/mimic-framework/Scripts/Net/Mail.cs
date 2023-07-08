using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.ComponentModel;

using UnityEngine;

namespace Mimic.Net{

    public class Mail{

        private string from;
        private SmtpClient smtpClient;

        private MailMessage mail;

        private Action onSuccessCallBack;        
        public Action OnSuccessCallBack{
            set{ onSuccessCallBack = value; }
        }

        private Action onErrorCallBack;
        public Action OnErrorCallBack{
            set{ onErrorCallBack = value; }
        }

        public Mail(string from, string smtpClient, string password, Action onSuccessCallBack, Action onErrorCallBack){
            this.from = from;
            this.smtpClient = new SmtpClient(smtpClient, 587);
            this.smtpClient.Credentials = new System.Net.NetworkCredential(from, password) as ICredentialsByHost;
            this.smtpClient.EnableSsl = true;
            ServicePointManager.ServerCertificateValidationCallback = ServerCertificateValidationCallback;
            this.smtpClient.SendCompleted += new SendCompletedEventHandler(SendCompletedCallback);
            
            this.onSuccessCallBack = onSuccessCallBack;
            this.onErrorCallBack = onErrorCallBack;
        }

        public void SendMail(string to, string subject, string body, List<string> attachmentsFilePaths){
            mail = new MailMessage();
 
            mail.From = new MailAddress(from);
            mail.To.Add(to);
            mail.Subject = subject;
            mail.Body = body;

            attachmentsFilePaths.ForEach(attachment => {
                if(File.Exists(attachment)){
                    mail.Attachments.Add(new Attachment(attachment));
                }
            });
            smtpClient.SendAsync(mail, "Mail");
        }

        private void SendCompletedCallback(object sender, AsyncCompletedEventArgs e) {
            foreach(Attachment attachment in mail.Attachments){
                attachment.Dispose();
            }
            mail.Dispose();
            if (e.Cancelled) {
                Debug.Log("Mail Status: ["+e.UserState+"] Send canceled.");
            }
            if (e.Error != null) {
                Debug.Log("Mail Status: ["+e.UserState+"] Error: "+ e.Error);
                if(onErrorCallBack != null)
                    onErrorCallBack();
            } else {
                Debug.Log("Mail Status: mail sent.");
                if(onSuccessCallBack != null)
                    onSuccessCallBack();
            }
        }

        private bool ServerCertificateValidationCallback(object s, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors){
            return true;
        }

    }

}