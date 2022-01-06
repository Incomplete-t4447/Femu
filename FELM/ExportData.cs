using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FELM
{
    public class ExportData
    {
        private string _StartDato;
        private string _SlutDato;
        private string _Varenummer;
        private string _Antal;

        public ExportData(string StartDato, string SlutDato, string Varenummer, string Antal)
        {
            this._StartDato = StartDato;
            this._SlutDato = SlutDato;
            this._Varenummer = Varenummer;
            this._Antal = Antal;

        }
        public string StartDato
        {
            get { return _StartDato; }
            set { _StartDato = value; }
        }

        public string SlutDato
        {
            get { return _SlutDato; }
            set { _SlutDato = value; }
        }

        public string Varenummer
        {
            get { return _Varenummer; }
            set { _Varenummer = value; }
        }
        public string Antal
        {
            get { return _Antal; }
            set { _Antal = value; }
        }
    }
}
