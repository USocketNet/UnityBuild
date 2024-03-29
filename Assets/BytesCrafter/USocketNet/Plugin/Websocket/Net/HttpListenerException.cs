#region License
/*
 * HttpListenerException.cs
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
using System.ComponentModel;
using System.Runtime.Serialization;

namespace WebSocketSharp.Net
{
  /// <summary>
  /// The exception that is thrown when a <see cref="HttpListener"/> gets an error
  /// processing an HTTP request.
  /// </summary>
  [Serializable]
  public class HttpListenerException : Win32Exception
  {
    #region Protected Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpListenerException"/> class from
    /// the specified <see cref="SerializationInfo"/> and <see cref="StreamingContext"/>.
    /// </summary>
    /// <param name="serializationInfo">
    /// A <see cref="SerializationInfo"/> that contains the serialized object data.
    /// </param>
    /// <param name="streamingContext">
    /// A <see cref="StreamingContext"/> that specifies the source for the deserialization.
    /// </param>
    protected HttpListenerException (
      SerializationInfo serializationInfo, StreamingContext streamingContext)
      : base (serializationInfo, streamingContext)
    {
    }

    #endregion

    #region Public Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpListenerException"/> class.
    /// </summary>
    public HttpListenerException ()
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpListenerException"/> class
    /// with the specified <paramref name="errorCode"/>.
    /// </summary>
    /// <param name="errorCode">
    /// An <see cref="int"/> that identifies the error.
    /// </param>
    public HttpListenerException (int errorCode)
      : base (errorCode)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpListenerException"/> class
    /// with the specified <paramref name="errorCode"/> and <paramref name="message"/>.
    /// </summary>
    /// <param name="errorCode">
    /// An <see cref="int"/> that identifies the error.
    /// </param>
    /// <param name="message">
    /// A <see cref="string"/> that describes the error.
    /// </param>
    public HttpListenerException (int errorCode, string message)
      : base (errorCode, message)
    {
    }

    #endregion

    #region Public Properties

    /// <summary>
    /// Gets the error code that identifies the error that occurred.
    /// </summary>
    /// <value>
    /// An <see cref="int"/> that identifies the error.
    /// </value>
    public override int ErrorCode {
      get {
        return NativeErrorCode;
      }
    }

    #endregion
  }
}
