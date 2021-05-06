using System;
using System.ComponentModel.DataAnnotations;

namespace PCM.Models.PCMViewModels
{
    //For form submission
    public class CostingModel
    {
        [Required]
        public string ModelID { get; set; }
        public string Modelname { get; set; }
        public string Brand { get; set; }
        public string Vendor { get; set; }
        public string Zone { get; set; }
        public string Type { get; set; }
        public string Bdm { get; set; }
        public DateTime Datecreated { get; set; }
        public DateTime LastUpdated { get; set; }
        /*Status codes
        0-not implemented
        1-draft
        2-approval submitted
        3-approved
        4-rejected
        5-implemented
             */
        public int Status { get; set; }
        public CostingModelData Data { get; set; }
        public string Rejectcomment { get; set; }
        
    }
}