
namespace BytesCrafter.USocketNet.Serials
{
    [System.Serializable]
    public class Ports
    {
        public string master
        {
            get {
                return masterPort.ToString();
            }
        }
        private int masterPort = 19090;

         public string chat
        {
            get {
                return chatPort.ToString();
            }
        }
        private int chatPort = 6060;

         public string game
        {
            get {
                return gamePort.ToString();
            }
        }
        private int gamePort = 9090;
    }
}