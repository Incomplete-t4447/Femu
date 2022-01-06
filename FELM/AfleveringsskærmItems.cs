using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FELM
{
    class AfleveringsskærmItems
    {
        private string _navn;
        private string _hylde;
        private string _krog;
        private string _farve;
        

        public AfleveringsskærmItems(string navn, string hylde, string krog, string farve)
        {
            this._navn = navn;
            this._hylde = hylde;
            this._krog = krog;
            this._farve = farve;
        }

        public string Navn
        {
            get { return _navn; }
            set { _navn = value; }
        }

        public string Hylde
        {
            get { return _hylde; }
            set { _hylde = value; }
        }

        public string Krog
        {
            get { return _krog; }
            set { _krog = value; }
        }

        public string Farve
        {
            get { return _farve; }
            set { _farve = value; }
        }
    }
}