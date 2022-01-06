using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FELM
{
    class TilbehørClass
    {
        private string _vareNavn;
        private int _antal;
        private string _lokation;
        private string _events;
        private string _status;

        public TilbehørClass(string VareNavn, int Antal, string Lokation, string Events, string Status)
        {
            this._vareNavn = VareNavn;
            this._antal = Antal;
            this._lokation = Lokation;
            this._events = Events;
            this._status = Status;
        }

        public string VareNavn
        {
            get { return _vareNavn; }
            set { _vareNavn = value; }
        }

        public int Antal
        {
            get { return _antal; }
            set { _antal = value; }
        }

        public string Lokation
        {
            get { return _lokation; }
            set { _lokation = value; }
        }
        public string Events
        {
            get { return _events; }
            set { _events = value; }
        }
        public string Status
        {
            get { return _status; }
            set { _status = value; }
        }
    }
}