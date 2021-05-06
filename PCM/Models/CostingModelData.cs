using System.Collections.Generic;

namespace PCM.Models.PCMViewModels
{
    public class CostingModelData : CostingModel
    {
        public IEnumerable<Itemgroup> Itemgroups { get; set; }

        //General data for all item groups
        public string Currency { get; set; }
        public decimal Exchangerate { get; set; }
        public string Freightterms { get; set; }
        public decimal Cashterms { get; set; }
        public decimal Tariff { get; set; }

        //financial costing data
        public int Affectedl12cases { get; set; }
        public int Affectedn12cases { get; set; }
        public int Unaffectedl12cases { get; set; }
        public int Unaffectedn12cases { get; set; }
        public decimal Affectedl12price { get; set; }
        public decimal Affectedn12price { get; set; }
        public decimal Unaffectedl12price { get; set; }
        public decimal Unaffectedn12price { get; set; }
        public decimal Affectedl12terms { get; set; }
        public decimal Affectedn12terms { get; set; }
        public decimal Unaffectedl12terms { get; set; }
        public decimal Unaffectedn12terms { get; set; }
        public decimal Affectedl12volumerebates { get; set; }
        public decimal Affectedn12volumerebates { get; set; }
        public decimal Unaffectedl12volumerebates { get; set; }
        public decimal Unaffectedn12volumerebates { get; set; }
        public decimal Affectedl12customerprograms { get; set; }
        public decimal Affectedn12customerprograms { get; set; }
        public decimal Unaffectedl12customerprograms { get; set; }
        public decimal Unaffectedn12customerprograms { get; set; }
        public decimal Affectedl12offinvoice { get; set; }
        public decimal Affectedn12offinvoice { get; set; }
        public decimal Unaffectedl12offinvoice { get; set; }
        public decimal Unaffectedn12offinvoice { get; set; }
        public decimal Affectedl12credits { get; set; }
        public decimal Affectedn12credits { get; set; }
        public decimal Unaffectedl12credits { get; set; }
        public decimal Unaffectedn12credits { get; set; }
        public decimal Affectedl12marketingspend { get; set; }
        public decimal Affectedn12marketingspend { get; set; }
        public decimal Unaffectedl12marketingspend { get; set; }
        public decimal Unaffectedn12marketingspend { get; set; }
        public decimal Affectedl12varopex { get; set; }
        public decimal Affectedn12varopex { get; set; }
        public decimal Unaffectedl12varopex { get; set; }
        public decimal Unaffectedn12varopex { get; set; }
        public decimal L12directopex { get; set; }
        public decimal N12directopex { get; set; }

        public string Comment { get; set; }
    }
}