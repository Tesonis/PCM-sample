using System.Collections.Generic;

namespace PCM.Models.PCMViewModels
{
    //For form submission
    public class Itemgroup : CostingModelData
    {
        public Itemgroup() { }
        public Itemgroup(string iid, string igroupname)
        {
            id = iid;
            groupname = igroupname;
            items = new List<Branditem>();
            currentpurchasepricepercase = 0;
            proposedpurchasepricepercase = 0;
            Exchangerate = 0;
            duty = 0;
            freightcosting = 0;
            currlandedcost = 0;
            dealaccruals = 0;
            Cashterms = 0;
            otherperc = 0;
            spoilagecredits = 0;
            branddevelopmentfund = 0;
            billback = 0;
            currwholesaleppc = 0;
            propwholesaleppc = 0;
            currwholesalesuggestedppc = 0;
            propwholesalesuggestedppc = 0;
            currwholesaletrademargin = 0;
            propwholesaletrademargin = 0;
            currdsdppc = 0;
            propdsdppc = 0;
            currdsdsuggestedppc = 0;
            propdsdsuggestedppc = 0;
            currdsdtrademargin = 0;
            propdsdtrademargin = 0;
            l12monthcasevolume = 0;
            n12monthcasevolume = 0;
            l12monthsalesrevenue = 0;
            n12monthsalesrevenue = 0;
        }
        
        public string id { get; set; }
        public string groupname { get; set; }
        public IEnumerable<Branditem> items { get; set; }
        public string itemlist { get; set; }
        public string warehousecat { get; set; }
        public bool shortshelflife { get; set; }
        public int unitspercase { get; set; }
        public string unitsize { get; set; }
        public decimal currentpurchasepricepercase { get; set; }
        public decimal proposedpurchasepricepercase { get; set; }
        public decimal duty { get; set; }
        public decimal freightcosting { get; set; }
        public decimal currlandedcost { get; set; }
        public decimal dealaccruals { get; set; }
        public decimal otherperc { get; set; }
        public decimal spoilagecredits { get; set; }
        public decimal branddevelopmentfund { get; set; }
        public decimal billback { get; set; }
        public decimal currwholesaleppc { get; set; }
        public decimal propwholesaleppc {get;set;}
        public decimal currwholesalesuggestedppc { get; set; }
        public decimal propwholesalesuggestedppc { get; set; }
        public decimal currwholesaletrademargin { get; set; }
        public decimal propwholesaletrademargin { get; set; }
        public decimal currdsdppc { get; set; }
        public decimal propdsdppc { get; set; }
        public decimal currdsdsuggestedppc { get; set; }
        public decimal propdsdsuggestedppc { get; set; }
        public decimal currdsdtrademargin { get; set; }
        public decimal propdsdtrademargin { get; set; }
        public int l12monthcasevolume { get; set; }
        public int n12monthcasevolume { get; set; }
        public decimal l12monthsalesrevenue { get; set; }
        public decimal n12monthsalesrevenue { get; set; }

    }
}