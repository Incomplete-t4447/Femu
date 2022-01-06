using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FELM
{
    public class EventHistorik
    {

        private string _EventId;
        private string _EventName;



        public EventHistorik(string EventId, string EventName)
        {
            this._EventId = EventId;
            this._EventName = EventName;
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

    }
}
