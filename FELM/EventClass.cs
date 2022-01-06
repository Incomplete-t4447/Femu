using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace FELM
{
    public class EventClass
    {



        private string _EventId;
        private string _EventName;
        private string _StartDato;
        private string _SlutDato;
        private string _Note;
        private string _DieselstanderStart;
        private string _DielselstanderSlut;
        private string _DefektePærer;
        private string _TlfNummer;
        private string _Aktiv;
        private string _BrugerId;




        public EventClass(string EventId, string EventName, string StartDato, string SlutDato, string Note, string DieselstanderStart, string DielselstanderSlut, string DefektePærer, string TlfNummer, string Aktiv, string BrugerId)
        {
            this._EventId = EventId;
            this._EventName = EventName;
            this._StartDato = StartDato;
            this._SlutDato = SlutDato;
            this._Note = Note;
            this._DieselstanderStart = DieselstanderStart;
            this._DielselstanderSlut = DielselstanderSlut;
            this._DefektePærer = DefektePærer;
            this._TlfNummer = TlfNummer;
            this._Aktiv = Aktiv;
            this._BrugerId = BrugerId;
        }



        public string EventId
        {
            get { return _EventId; }
            set { _EventId = value; }
        }
        public string EventName
        {
            get { return _EventName; }
            set { _EventName = value; }
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
        public string Note
        {
            get { return _Note; }
            set { _Note = value; }
        }
        public string DieselstanderStart
        {
            get { return _DieselstanderStart; }
            set { _DieselstanderStart = value; }
        }
        public string DielselstanderSlut
        {
            get { return _DielselstanderSlut; }
            set { _DielselstanderSlut = value; }
        }
        public string DefektePærer
        {
            get { return _DefektePærer; }
            set { _DefektePærer = value; }
        }
        public string TlfNummer
        {
            get { return _TlfNummer; }
            set { _TlfNummer = value; }
        }
        public string Aktiv
        {
            get { return _Aktiv; }
            set { _Aktiv = value; }
        }

        public string BrugerId
        {
            get { return _BrugerId; }
            set { _BrugerId = value; }
        }
    }
}