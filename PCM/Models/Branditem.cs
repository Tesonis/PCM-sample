using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace PCM.Models
{
    public class Branditem
    {
        public Branditem() { }
        public Branditem(string id, string desc, string size, int units, decimal ppc, decimal wholesale, string warehousecat, int shelflife)
        {
            this.itemID = id;
            this.itemdescription = desc;
            this.unitsize = size;
            this.unitspercase = units;
            this.purchaseprice = ppc;
            this.wholesaleprice = wholesale;
            this.warehousecat = warehousecat;
            this.shelflife = shelflife;
        }
        public string itemID { get; set; }
        public string itemdescription { get; set; }
        public string unitsize { get; set; }
        public int unitspercase { get; set; }
        public decimal purchaseprice { get; set; }
        public decimal wholesaleprice { get; set; }
        public decimal dsdprice { get; set; }
        public decimal movingaveragecost { get; set; }
        public string warehousecat { get; set; }
        public int shelflife { get; set; }
        public decimal totalsales { get; set; }
        public decimal unitssold { get; set; }
        public decimal dutypct { get; internal set; }
        public decimal retailprice { get; internal set; }
    }
}