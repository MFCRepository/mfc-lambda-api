using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;

namespace MyHttpGatewayApi.Utilities
{
    public static class Email
    {
        public class MailAddress : System.Net.Mail.MailAddress
        {
            // we do this because the Mail.MailAddress doesn't have a default constructor so when
            // trying to deserialize it JSON.NET throws an error. We create our own MailAddress
            // class that inherits from Mail.MailAddress with a default constructor and the problem
            // is solved

            public MailAddress(string DisplayName, string Address) : base(Address, DisplayName)
            {
            }
        }

        [Serializable]
        public class Attachment
        {
            // have to do this idiocy because the Mail.Attachment class isn't serializable
            // so we wrap a Mail.Attachment object in a class that IS serializable and use that instead
            private System.Net.Mail.Attachment _attachment;

            public Attachment(System.Net.Mail.Attachment _attachment)
            {
                attachment = _attachment;
            }

            public Attachment(string _filename)
            {
                attachment = new System.Net.Mail.Attachment(_filename);
            }

            public Attachment(MemoryStream _ms, System.Net.Mime.ContentType _mimeType)
            {
                attachment = new System.Net.Mail.Attachment(_ms, _mimeType);
            }
            public Attachment(MemoryStream _ms, System.Net.Mime.ContentType _mimeType, string _name)
            {
                attachment = new System.Net.Mail.Attachment(_ms, _mimeType);
                attachment.Name = _name;
            }

            public Attachment(MemoryStream _ms, string _mimeType)
            {
                attachment = new System.Net.Mail.Attachment(_ms, _mimeType);
            }

            public System.Net.Mail.Attachment attachment { get; set; }

        }

        public class EmailMessage
        {
            public string subject { get; set; }

            public string body { get; set; }

        }

        /// <summary>
        /// Static class with one function, Send(params).
        /// Sends an email with the credentials retrieved using _credential_key_name parameter
        /// </summary>
        /// <param name="_to_addresses">List of addresses to send to</param>
        /// <param name="_cc_addresses">List of addresses to CC, pass null if not used</param>
        /// <param name="_bcc_addresses">List of addresses to BCC, pass null if not used</param>
        /// <param name="_subject">Email subject line</param>
        /// <param name="_body">Body of email, by default treated as HTML, can be forced to plaintext by setting conditional_is_html parameter to false</param>
        /// <param name="_attachments">List of attachments. Attachments can be MemoryStream objects or fully qualified file names. Pass null if not used</param>
        /// <param name="optional_credential_key_name">Name of credential to use for send authentication and email address, default is noreply.</param>
        /// <param name="optional_is_html">Default _body is sent as HTML, force plaintext by setting this to false</param>
        /// <returns>non "success" on any failure</returns>
        public static string Send(List<MailAddress> _to_addresses,
            List<MailAddress> _cc_addresses,
            List<MailAddress> _bcc_addresses,
            string _subject,
            string _body,
            List<Attachment> _attachments,
            string optional_credential_key_name = "noreply",
            bool optional_is_html = true)
        {
            var _outlook_365_credentials = Utilities.AmazonWebServices.SecretsManager.outlook_credentials.credentials.send_credentials.Find(x => x.name == optional_credential_key_name);
            string _smtp_host = "smtp.office365.com";
            int _port = 587;

            try {
                MailMessage _mail_message = new MailMessage()
                {
                    From = new MailAddress(_outlook_365_credentials.name, _outlook_365_credentials.address),
                    Subject = _subject,
                    Body = _body,
                    IsBodyHtml = optional_is_html
                };

                if (_to_addresses != null && _to_addresses.Count > 0)
                {
                    foreach (MailAddress _address in _to_addresses)
                    {
                        _mail_message.To.Add(_address);
                    }
                }

                if (_cc_addresses != null && _cc_addresses.Count > 0)
                {
                    foreach (MailAddress _address in _cc_addresses)
                    {
                        _mail_message.CC.Add(_address);
                    }
                }

                if (_bcc_addresses != null && _bcc_addresses.Count > 0)
                {
                    foreach (MailAddress _address in _bcc_addresses)
                    {
                        _mail_message.Bcc.Add(_address);
                    }
                }

                if (_attachments != null && _attachments.Count > 0)
                {
                    foreach (Attachment _attachment in _attachments)
                    {
                        _mail_message.Attachments.Add(_attachment.attachment);
                    }
                }

                using (SmtpClient _client = new SmtpClient()
                {
                    Host = _smtp_host,
                    Port = _port,
                    EnableSsl = true,
                    UseDefaultCredentials = false,
                    Credentials = new NetworkCredential() { 
                        UserName = _outlook_365_credentials.username, 
                        Password = _outlook_365_credentials.password 
                    }
                })
                {
                    _client.Send(_mail_message);

                }

                return "success";

            } catch (Exception _ex)
            {
                return _ex.InnerException.ToString();

            }
        }
    }
}
