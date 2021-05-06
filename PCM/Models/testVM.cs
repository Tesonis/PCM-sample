using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PCM.Models
{
    public class testVM
    {
        public string fname { get; set; }
        public string lname { get; set; }

        public IEnumerable<Person> person { get; set; }
    }
}