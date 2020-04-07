#region License
/*
 * HttpVersion.cs
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

namespace WebSocketSharp.Net
{
  /// <summary>
  /// Provides the HTTP version numbers.
  /// </summary>
  public class HttpVersion
  {
    #region Public Static Fields

    /// <summary>
    /// Provides a <see cref="Version"/> instance for HTTP 1.0.
    /// </summary>
    public static readonly Version Version10 = new Version (1, 0);

    /// <summary>
    /// Provides a <see cref="Version"/> instance for HTTP 1.1.
    /// </summary>
    public static readonly Version Version11 = new Version (1, 1);

    #endregion

    #region Public Constructors

    /// <summary>
    /// Initializes a new instance of the <see cref="HttpVersion"/> class.
    /// </summary>
    public HttpVersion ()
    {
    }

    #endregion
  }
}
