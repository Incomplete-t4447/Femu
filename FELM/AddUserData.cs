namespace FELM
{
    public class AddUserData
    {
        private string _UserName;
        private string _Name;
        private string _PassWord;
        private string _Mail;
        private string _Color;
        private string _Type;
        private string _Adresse;
        private string _Nr;
        private int _PostNr;
        private int _TelefonNummer;
        private string _RfidNummer;





        public AddUserData(string username, string name, string password, string mail, string color, string type, string adresse, string nr, int postnr, int telefonnummer, string rfidNummer)
        {
            this._UserName = username;
            this._Name = name;
            this._PassWord = password;
            this._Mail = mail;
            this._Color = color;
            this._Type = type;
            this._Adresse = adresse;
            this._Nr = nr;
            this._PostNr = postnr;
            this._TelefonNummer = telefonnummer;
            this._RfidNummer = rfidNummer;

        }
        public string Username
        {
            get { return _UserName; }
            set { _UserName = value; }
        }

        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        public string Password
        {
            get { return _PassWord; }
            set { _PassWord = value; }
        }

        public string Mail
        {
            get { return _Mail; }
            set { _Mail = value; }
        }

        public string Color
        {
            get { return _Color; }
            set { _Color = value; }
        }
        public string Type
        {
            get { return _Type; }
            set { _Type = value; }
        }
        public string Adresse
        {
            get { return _Adresse; }
            set { _Adresse = value; }
        }
        public string Nr
        {
            get { return _Nr; }
            set { _Nr = value; }
        }
        public int PostNr
        {
            get { return _PostNr; }
            set { _PostNr = value; }
        }
        public int TelefonNummer
        {
            get { return _TelefonNummer; }
            set { _TelefonNummer = value; }
        }

        public string Rfidnummer
        {
            get { return _RfidNummer; }
            set { _RfidNummer = value; }
        }


    }
}