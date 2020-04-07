
namespace BytesCrafter.USocketNet.Serializables 
{
    [System.Serializable]
    public class BC_USN_Token
    {
        public string wpid = string.Empty;
        public string snid = string.Empty;

        public BC_USN_Token() {
            wpid = string.Empty;
            snid = string.Empty;
        }

        public BC_USN_Token(string res_wpid, string res_snid) {
            wpid = res_wpid;
            snid = res_snid;
        }

        public bool success {
            get {
                return wpid != string.Empty || snid != string.Empty ? false : true;
            }
        }
    }
}