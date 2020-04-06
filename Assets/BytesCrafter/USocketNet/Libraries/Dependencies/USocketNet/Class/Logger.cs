using UnityEngine;

namespace BytesCrafter.USocketNet.Toolsets 
{
    public enum Debugs 
    { 
        Log, 
        Warn, 
        Error 
    }

    public class BC_USN_Logger
    {
        private USocketClient usnClient = null;
        public BC_USN_Logger( USocketClient reference ) 
        {
            usnClient = reference;
        }

        public void Push(Debugs debugs, string title, string details) 
        {
            if(!usnClient.bind.debugOnLog)
				return;

            switch( debugs ) 
            {
                case Debugs.Log:
                    Debug.Log(title + " - " + details);
                    break;
                case Debugs.Warn:
                    Debug.LogWarning(title + " - " + details);
                    break;
                case Debugs.Error:
                    Debug.LogError(title + " - " + details);
                    break;
                default:
                    break;
            }
        }
    }
}