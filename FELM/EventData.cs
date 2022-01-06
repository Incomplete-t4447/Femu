using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FELM
{
    class EventData
    {
        
        private string _Name;
        private int _ID; 
        private string _Aktiv;
        private string _BrugerId;



        public EventData(string Name, int ID, string Aktiv, string BrugerId)
        {
            this._Name = Name;
            this._ID = ID;
            this._Aktiv = Aktiv;
            this._BrugerId = BrugerId;
        }

        
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }
        public int ID
        {
            get { return _ID; }
            set { _ID = value; }
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