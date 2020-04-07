#region License
/*
 * SslStream.cs
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
using System.Net.Security;
using System.Net.Sockets;

namespace WebSocketSharp.Net.Security
{
  internal class SslStream : System.Net.Security.SslStream
  {
    #region Public Constructors

    public SslStream (NetworkStream innerStream)
      : base (innerStream)
    {
    }

    public SslStream (NetworkStream innerStream, bool leaveInnerStreamOpen)
      : base (innerStream, leaveInnerStreamOpen)
    {
    }

    public SslStream (
      NetworkStream innerStream,
      bool leaveInnerStreamOpen,
      RemoteCertificateValidationCallback userCertificateValidationCallback)
      : base (innerStream, leaveInnerStreamOpen, userCertificateValidationCallback)
    {
    }

    public SslStream (
      NetworkStream innerStream,
      bool leaveInnerStreamOpen,
      RemoteCertificateValidationCallback userCertificateValidationCallback,
      LocalCertificateSelectionCallback userCertificateSelectionCallback)
      : base (
        innerStream,
        leaveInnerStreamOpen,
        userCertificateValidationCallback,
        userCertificateSelectionCallback)
    {
    }

    #endregion

    #region Public Properties

    public bool DataAvailable {
      get {
        return ((NetworkStream) InnerStream).DataAvailable;
      }
    }

    #endregion
  }
}
