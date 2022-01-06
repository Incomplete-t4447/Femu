
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;

namespace FELM
{
    public class EventVare
    {
        private int _Id;
        private string _VareId;
        private string _Beskrivelse;
        private string _Ampere;
        private string _EventLokation;
        private string _EventName;
        private string _Status;
        private string _Aflevere;
        private string _WebshopVarenummer;
        private string _PinNr;
        private string _Container;

        public EventVare(int Id, string VareId, string Beskrivelse, string Ampere, string EventLokation, string EventName, string Status, string Aflevere, string WebShopVarenummer, string PinNr, string Container )
        {
            this._Id = Id;
            this._VareId = VareId;
            this._Beskrivelse = Beskrivelse;
            this._Ampere = Ampere;
            this._EventLokation = EventLokation;
            this._EventName = EventName;
            this._Status = Status;
            this._Aflevere = Aflevere;
            this._WebshopVarenummer = WebShopVarenummer;
            this._PinNr = PinNr;
            this._Container = Container;
        }
        public int Id
        {
            get { return _Id; }
            set { _Id = value; }
        }

        public string Vareid
        {
            get { return _VareId; }
            set { _VareId = value; }
        }

        public string Beskrivelse
        {
            get { return _Beskrivelse; }
            set { _Beskrivelse = value; }
        }

        public string Ampere
        {
            get { return _Ampere; }
            set { _Ampere = value; }
        }

        public string EventLokation
        {
            get { return _EventLokation; }
            set { _Beskrivelse = value; }
        }

        public string EventName
        {
            get { return _EventName; }
            set { _EventName = value; }
        }

        public string Status
        {
            get { return _Status; }
            set { _Status = value; }
        }

        public string Afleveret
        {
            get { return _Aflevere; }
            set { _Aflevere = value; }
        }

        public string WebShopVarenummer
        {
            get { return _WebshopVarenummer; }
            set { _WebshopVarenummer = value; }
        }

        public string PinNr
        {
            get { return _PinNr; }
            set { _PinNr = value; }
        }

        public string Container
        {
            get { return _Container; }
            set { _Container = value; }
        }
    }
}