using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FELM
{
    public class kontainerClass
    {
        private int _LocationdID;
        private string _location;
        private string _description;
        private int _VareNrStart;
        private int _VareNrSlut;

        public kontainerClass(int locationdID, string location, string description, int vareNrStart, int vareNrSlut)
        {
            this._LocationdID = locationdID;
            this._location = location;
            this._description = description;
            this._VareNrStart = vareNrStart;
            this._VareNrSlut = vareNrSlut;
        }

        public int LocationdID
        {
            get { return _LocationdID; }
            set { _LocationdID = value; }
        }

        public string location
        {
            get { return _location; }
            set { _location = value; }
        }

        public string description
        {
            get { return _description; }
            set { _description = value; }
        }
        public int VareNrStart
        {
            get { return _VareNrStart; }
            set { _VareNrStart = value; }
        } 
        public int VareNrSlut
        {
            get { return _VareNrSlut; }
            set { _VareNrSlut = value; }
        }
        
    }
}
