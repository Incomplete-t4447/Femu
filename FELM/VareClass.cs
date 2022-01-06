using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FELM
{
    public class VareClass
    {
        private string _VareNr;
        private string _Beskrivelse;
        private string _Tilgang;
        private string _Afgang;
        private string _Ampere;
        private string _Status;
        private int _Antal;
        private string _VareLokation;
        private string _PinNr;
        private int _Længde;
        private string _Note;
        private string _WebshopVareNummer;
        private string _RFIDNummer;
        private string _QR;
        private string _Salgspris;
        private string _EventVareId;

        public VareClass(string VareNr, string Beskrivelse, string Tilgang, string Afgang, string Ampere, string Status, int Antal, string VareLokation, string PinNr, int Længde, string Note, string Salgspris, string WebshopVareNummer = null, string RFIDNummer = null, string QR = null, string EventVareId = null)
        {
            this._VareNr = VareNr;
            this._Beskrivelse = Beskrivelse;
            this._Tilgang = Tilgang;
            this._Afgang = Afgang;
            this._Ampere = Ampere;
            this._Status = Status;
            this._Antal = Antal;
            this._VareLokation = VareLokation;
            this._PinNr = PinNr;
            this._Længde = Længde;
            this._Note = Note;
            this._Salgspris = Salgspris;
            this._WebshopVareNummer = WebshopVareNummer;
            this._RFIDNummer = RFIDNummer;
            this._QR = QR;
            this._EventVareId = EventVareId;
        }

        public string VareNr
        {
            get { return _VareNr; }
            set { _VareNr = value; }
        }

        public string Beskrivelse
        {
            get { return _Beskrivelse; }
            set { _Beskrivelse = value; }
        }

        public string Tilgang
        {
            get { return _Tilgang; }
            set { _Tilgang = value; }
        }

        public string Afgang
        {
            get { return _Afgang; }
            set { _Afgang = value; }
        }

        public string Ampere
        {
            get { return _Ampere; }
            set { _Ampere = value; }
        }

        public string Note
        {
            get { return _Note; }
            set { _Note = value; }
        }

        public string Salgspris
        {
            get { return _Salgspris; }
            set { _Salgspris = value; }
        }

        public string Status
        {
            get { return _Status; }
            set { _Status = value; }
        }

        public int Antal
        {
            get { return _Antal; }
            set { _Antal = value; }
        }

        public string VareLokation
        {
            get { return _VareLokation; }
            set { _VareLokation = value; }
        }

        public string PinNr
        {
            get { return _PinNr; }
            set { _PinNr = value; }
        }

        public string WebshopVareNummer
        {
            get { return _WebshopVareNummer; }
            set { _WebshopVareNummer = value; }
        }

        public int Længde
        {
            get { return _Længde; }
            set { _Længde = value; }
        }

        public string RFIDNummer
        {
            get { return _RFIDNummer; }
            set { _RFIDNummer = value; }
        }

        public string QR
        {
            get { return _QR; }
            set { _QR = value; }
        }

        public string EventVareId
        {
            get { return _EventVareId; }
            set { _EventVareId = value; }
        }

    }
}