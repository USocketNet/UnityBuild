
namespace BytesCrafter.USocketNet.Serializables {
    [System.Serializable]
    public class WPToken
    {
        public string wpid = string.Empty;
        public string snid = string.Empty;

        public WPToken() {
            wpid = string.Empty;
            wpid = string.Empty;
        }

        public WPToken(string res_wpid, string res_snid) {
            wpid = res_wpid;
            wpid = res_snid;
        }

        public bool success {
            get {
                return wpid != string.Empty || snid != string.Empty ? false : true;
            }
        }
    }
}