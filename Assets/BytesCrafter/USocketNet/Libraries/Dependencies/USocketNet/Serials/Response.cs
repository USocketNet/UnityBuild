﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace BytesCrafter.USocketNet.Serializables {
    [System.Serializable]
    public class Response
    {
        public string code = "error";
        public string message = string.Empty;
        public Response_Data data = null;

        public bool success {
            get {
                return code == "success" ? true : false;
            }
        }
    }

    [System.Serializable]
    public class Response_Data
    {
        public string session = string.Empty;
        public string cookie = string.Empty;
        public string avatar = string.Empty;
        public string id = string.Empty;
        public string uname = string.Empty;
        public string dname = string.Empty;
        public string email = string.Empty;
        public string[] roles;

        public  WPToken token {
            get {
                return new WPToken(id, session);
            }
        }
        
    }
}
