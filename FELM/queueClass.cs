using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FELM
{
    class queueClass
    {
        private string _eventId;
        private string _rfidNr;
        private string _eventLokation;
        private string _status;

        public queueClass(string eventId, string rfidNr, string eventLokation, string status)
        {
            _eventId = eventId;
            _rfidNr = rfidNr;
            _eventLokation = eventLokation;
            _status = status;
        }

        public string eventId
        {
            get { return _eventId; }
            set { _eventId = value; }
        }
        
        public string rfidNr
        {
            get { return _rfidNr; }
            set { _rfidNr = value; }
        }

        public string eventLokation
        {
            get { return _eventLokation; }
            set { _eventLokation = value; }
        }

        public string status
        {
            get { return _status; }
            set { _status = value; }
        }
    }
}
