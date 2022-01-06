using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FELM
{
    class eventUsersClass
    {
        private int _id;
        private string _brugernavn;
        private string _eventNavn;

        public eventUsersClass(int id, string brugernavn, string eventNavn)
        {
            _id = id;
            _brugernavn = brugernavn;
            _eventNavn = eventNavn;
        }

        public int id
        {
            get { return _id; }
            set { _id = value; }
        }

        public string brugernavn
        {
            get { return _brugernavn; }
            set { _brugernavn = value; }
        }

        public string eventNavn
        {
            get { return _eventNavn; }
            set { _eventNavn = value; }
        }
    }
}
