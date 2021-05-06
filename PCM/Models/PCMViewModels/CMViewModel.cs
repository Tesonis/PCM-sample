using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PCM.Models.PCMViewModels
{
    public class CMViewModel
    {

        public IEnumerable<SelectListItem> Brand { get; set; }
        public IEnumerable<SelectListItem> Vendor { get; set; }
        public IEnumerable<SelectListItem> PriceZone { get; set; }
        public IEnumerable<SelectListItem> CMType { get; set; }
        public IEnumerable<Branditem> Branditems { get; set; }
        public IEnumerable<Branditem> RestItemsList { get; set; }

        public CostingModel Costingmodel { get; set; }
        public int Step { get; set; }
        
        public ErrorViewModel errorvm { get; set; }
        public Itemgroup currItemgroup { get; set; }
        public IEnumerable<Itemgroup> ItemgroupList { get; set; }
        
    }
}