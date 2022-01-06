using System;

namespace CS203Engine.Events
{
    public class OnTagReadEventArgs : EventArgs
    {
        public readonly string TagID;

        public OnTagReadEventArgs(string _TagID)
        {
            this.TagID = _TagID;
        }
    }
}
