using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FELM
{
    public class lokalitionEventClass
    {
        private string _lokalition;
        private string _event;
        public lokalitionEventClass(string Lokalition, string Event)
        {
            this._lokalition = Lokalition;
            this._event = Event;
        }
        public string Lokalition
        {
            get { return _lokalition; }
            set { _lokalition = value; }
        }
        public string Event
        {
            get { return _event; }
            set { _event = value; }
        }
    }
}
