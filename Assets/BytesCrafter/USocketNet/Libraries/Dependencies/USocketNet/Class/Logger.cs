using UnityEngine;

namespace BytesCrafter.USocketNet.Toolsets 
{
    public enum BC_USN_Debug
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

        public void Push(BC_USN_Debug logType, string title, string details) 
        {
            if(!usnClient.options.debugOnLog)
				return;

            switch( logType ) 
            {
                case BC_USN_Debug.Log:
                    Debug.Log(title + " - " + details);
                    break;
                case BC_USN_Debug.Warn:
                    Debug.LogWarning(title + " - " + details);
                    break;
                case BC_USN_Debug.Error:
                    Debug.LogError(title + " - " + details);
                    break;
                default:
                    break;
            }
        }
    }
}