using System;
using UnityEngine;

[Serializable]
public class MsgJson
{
    public string sndr {
        get {
            return processJson.sndr;
        }
    }
    public string msg;
    public string rcvr;
    public string date {
        get {
            return processJson.date;
        }
    }
    public string json;
    private MsgRaw processJson
    {
        get {
            return JsonUtility.FromJson<MsgRaw>(json);
        }
    } 
}

[Serializable]
public class MsgRaw
{
    public string sndr;
    public string msg;
    public string rcvr;
    public string date;
}