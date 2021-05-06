using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PCM.Models.PCMViewModels
{
    public class IndexViewModel
    {
        public string uname { get; set; }
        public string role { get; set; }
        public IEnumerable<CostingModel> allcostingmodel { get; set; }
        public IEnumerable<CostingModel> last5updatedCM { get; set; }
        public IEnumerable<PriceChange> allpricechange { get; set; }
        public ErrorViewModel errorvm { get; set; }
    }
}