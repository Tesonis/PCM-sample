using System;
using System.ComponentModel.DataAnnotations;

namespace PCM.Models.PCMViewModels
{
    public class PriceChange
    {
        [Required]
        public string pcid { get; set; }
        public string pcname { get; set; }
        public string bdm { get; set; }
        public DateTime implementdate { get; set; }
        //zone codes
        //0-all, 1-zone1, 3-zone3
        public int zoneapplied { get; set; }
        /*Status codes
        0-implemented
        1-draft
        2-approval submitted
        3-approved
        4-rejected
             */
        public int status { get; set; }
        [Required]
        public CostingModel relatedmodel { get; set; }


    }
}