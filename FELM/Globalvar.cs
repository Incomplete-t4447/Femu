using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FELM
{
    class Globalvar
    {
        private bool _UpdateEventList = false;
        private bool _ScannerOn = false;
        private bool _scannerOn = false;
        private bool _noNet = false;

        public bool updateEventList
        {
            get { return _UpdateEventList; }
            set { _UpdateEventList = value; }
        }
        public bool ScannerOn
        {
            get { return _ScannerOn; }
            set { _ScannerOn = value; }
        }

        public bool noNet
        {
            get { return _noNet; }
            set { _noNet = value; }
        }
        public static bool scannerOn { get; set; }


        public static bool UpdateEventListe { get; set; }


        public static bool NoNet { get; set; }

        //  public static bool UpdateEventList { get; set; }
    }

}
