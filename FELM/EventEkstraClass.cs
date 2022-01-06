using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FELM
{
    class EventEkstraClass
    {
        private int _ID;
        private string _Beskrivelse;
        private string _Antal;
        private string _EventId;
        private string _EventLokation;
        private string _Status;

        public EventEkstraClass(int ID, string beskrivelse, string antal, string eventId, string eventLokation, string status)
        {
            _ID = ID;
            _Beskrivelse = beskrivelse;
            _Antal = antal;
            _EventId = eventId;
            _EventLokation = eventLokation;
            _Status = status;
        }

        public int ID
        {
            get { return _ID; }
            set { _ID = value; }
        }

        public string Beskrivelse
        {
            get { return _Beskrivelse; }
            set { _Beskrivelse = value; }
        }

        public string Antal
        {
            get { return _Antal; }
            set { _Antal = value; }
        }

        public string EventId
        {
            get { return _EventId; }
            set { _EventId = value; }
        }

        public string EventLokation
        {
            get { return _EventLokation; }
            set { _EventLokation = value; }
        }

        public string Status
        {
            get { return _Status; }
            set { _Status = value; }
        }
    }
}
