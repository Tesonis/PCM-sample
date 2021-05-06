using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PCM.Models
{
    public class EmailAddress
    {
        [Required]
        [EmailAddress]
        public string emailAddress { get; set; }

        public string displayName { get; set; }
        public string username { get; set; }
        public string role { get; set; }
        public EmailAddress() { }

        public EmailAddress(string emailAddress, string displayName)
        {
            this.emailAddress = emailAddress;
            this.displayName = displayName;
        }
    }

}