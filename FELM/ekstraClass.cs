using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FELM
{
    public class ekstraClass
    {
        private string _Id;
        private string _Beskrivelse;
        private int _Antal;
        private string _WebshopVareNummer;


        public ekstraClass(string Beskrivelse, int Antal, string WebshopVareNummer, string Id = null)
        {
            this._Id = Id;
            this._Beskrivelse = Beskrivelse;
            this._Antal = Antal;
            this._WebshopVareNummer = WebshopVareNummer;

        }

        public string Id
        {
            get { return _Id; }
            set { _Id = value; }
        }
        public string Beskrivelse
        {
            get { return _Beskrivelse; }
            set { _Beskrivelse = value; }
        }

        public int Antal
        {
            get { return _Antal; }
            set { _Antal = value; }
        }

        public string WebshopVareNummer
        {
            get { return _WebshopVareNummer; }
            set { _WebshopVareNummer = value; }
        }
    }
}
