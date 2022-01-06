using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FELM
{
    public class lokationerClass
    {
        private string _EventId;
        private string _Lokation;
        private string _LokationId;

        public lokationerClass(string EventId, string Lokation, string LokationId = null)
        {
            this._EventId = EventId;
            this._Lokation = Lokation;
            this._LokationId = LokationId;
        }

        public string Lokation
        {
            get { return _Lokation; }
            set { _Lokation = value; }
        }

        public string EventId
        {
            get { return _EventId; }
            set { _EventId = value; }
        }
        public string LokationId
        {
            get { return _LokationId; }
            set { _LokationId = value; }
        }
    }
    
}
