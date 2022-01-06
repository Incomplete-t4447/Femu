using System.Collections.Generic;

namespace FELM
{
    public class ComboClass
    {
        private List<string> _Comboolist;

        public ComboClass(List<string> CombooList)
        {
            this._Comboolist = CombooList;
        }

        public List<string> CombooList
        {
            get { return _Comboolist; }
            set { _Comboolist = value; }
        }
    }
}
