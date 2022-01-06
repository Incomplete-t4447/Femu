using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace FELM
{
    class ItemClass
    {
        public string VareNummer { get; set; }
        public string Beskrivelse { get; set; }
        public string Antal { get; set; }
        public string Status { get; set; }
        public string Lokation { get; set; }
        public string Ampere { get; set; }
        public string PinNummer { get; set; }
        public string Længde { get; set; }
        public string WebshopNummer { get; set; }
        public string RFIDNummer { get; set; }
        public string Note { get; set; }
        public int Favorit { get; set; }

        public List<ItemClass> Itemize(JArray array)
        {
            List<ItemClass> itemList = new List<ItemClass>();
            if (array != null)
            {
                foreach (JObject item in array)
                {
                    ItemClass itemC = new ItemClass((string)item.GetValue("varenummer"), (string)item.GetValue("beskrivelse"), (string)item.GetValue("antal"), (string)item.GetValue("status"), (string)item.GetValue("vareLokation"), (string)item.GetValue("ampere"), (string)item.GetValue("pinNr"), (string)item.GetValue("length"), (string)item.GetValue("webshopNummer"), (string)item.GetValue("rfidNummer"), (string)item.GetValue("note"), (int)item.GetValue("favorit"));
                    itemList.Add(itemC);
                }
            }

            return itemList;
        }

        public ItemClass(string vareNummer, string beskrivelse, string antal, string status, string lokation, string ampere, string pinNummer, string længde, string webshopNummer, string rFIDNummer, string note, int favorit)
        {
            VareNummer = vareNummer;
            Beskrivelse = beskrivelse;
            Antal = antal;
            Status = status;
            Lokation = lokation;
            Ampere = ampere;
            PinNummer = pinNummer;
            Længde = længde;
            WebshopNummer = webshopNummer;
            RFIDNummer = rFIDNummer;
            Note = note;
            Favorit = favorit;
        }

        public ItemClass()
        {
        }
    }
}