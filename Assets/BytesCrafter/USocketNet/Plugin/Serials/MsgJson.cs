
#region License
/*
 * MsgJson.cs
 *
 * Copyright (c) 2020 Bytes Crafter
 *
 * Permission is hereby granted to any person obtaining a copy from our store
 * of this software and associated documentation files (the "Software"), to deal
 * in the Software with restriction to the rights to modify, merge, publish, 
 * distribute, sublicense, and/or sell copies of the Software, and to permit 
 * persons to whom the Software is furnished to do so, subject to the following 
 * conditions:
 *
 * The above copyright notice and this permission notice shall be included in
 * all copies or substantial portions of the Software.
 *
 * THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 * IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 * FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 * AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 * LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 * OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 * THE SOFTWARE.
 */
#endregion

using System;

[Serializable]
public class MsgJson
{
    public string sender { get; private set; }
    public string username { get; private set; }
    public string message { get; private set; }
    public string receiver { get; private set; }
    public string datestamp { get; private set; }

    public MsgJson(string _myuname, string _message, string _datestamp)
    {
        username = _myuname;
        message = _message;
        datestamp = _datestamp;
    } 

    public MsgJson(MsgRaw msgRaw)
    {
        username = msgRaw.u;
        sender = msgRaw.s;
        message = msgRaw.m;
        receiver = msgRaw.r;
        datestamp = msgRaw.d;
    } 
}

[Serializable]
public class MsgRaw
{
    /// <summary>
    /// Username.
    /// </summary>
    public string u;
    /// <summary>
    /// Sender.
    /// </summary>
    public string s;
    /// <summary>
    /// Message.
    /// </summary>
    public string m;
    /// <summary>
    /// Receiver.
    /// </summary>
    public string r;
    /// <summary>
    /// Datestamp.
    /// </summary>
    public string d;

    public MsgRaw(string msgContent) {
        m = msgContent;
    }

    public MsgRaw(string msgContent, string userID) {
        m = msgContent;
        r = userID;
    }
}