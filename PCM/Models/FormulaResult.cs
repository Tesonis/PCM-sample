using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PCM.Models
{
    public class FormulaResult
    {
        public string PriceType { get; set; }
        public List<ComponentRow> Components { get; set; }
    }
}