using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PCM.Models
{
    public class BrandTeam
    {
        public string Teamleadname { get; set; }
        public string Teamleademail { get; set; }
        public List<string> Teammembernames { get; set; }
        public List<string> Teammemberemails { get; set; }
    }
}