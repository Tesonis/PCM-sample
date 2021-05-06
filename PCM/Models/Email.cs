using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PCM.Models
{
    public class Email
    {
        [Required]
        public EmailAddress sender { get; set; }

        [Required]
        public List<EmailAddress> recipients { get; set; }

        public List<EmailAddress> recipientsCC { get; set; }
        

        [Required]
        public string subject { get; set; }

        [Required]
        public string body { get; set; }

        public bool isHTML { get; set; }
        

        public Email()
        {
            recipients = new List<EmailAddress>();
            recipientsCC = new List<EmailAddress>();
        }

        public Email(EmailAddress sender, List<EmailAddress> recipients, List<EmailAddress> recipientsCC, List<EmailAddress> recipientsBCC, string subject, string body, bool isHTML)
        {
            this.sender = sender;
            this.recipients = recipients;
            this.recipientsCC = recipientsCC;
            this.subject = subject;
            this.body = body;
            this.isHTML = isHTML;
        }
    }

}