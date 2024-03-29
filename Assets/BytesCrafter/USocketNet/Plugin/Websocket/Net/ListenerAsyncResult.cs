#region License
/*
 * ListenerAsyncResult.cs
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
using System.Threading;

namespace WebSocketSharp.Net
{
  internal class ListenerAsyncResult : IAsyncResult
  {
    #region Private Fields

    private AsyncCallback       _callback;
    private bool                _completed;
    private HttpListenerContext _context;
    private Exception           _exception;
    private ManualResetEvent    _waitHandle;
    private object              _state;
    private object              _sync;
    private bool                _syncCompleted;

    #endregion

    #region Internal Fields

    internal bool EndCalled;
    internal bool InGet;

    #endregion

    #region Public Constructors

    public ListenerAsyncResult (AsyncCallback callback, object state)
    {
      _callback = callback;
      _state = state;
      _sync = new object ();
    }

    #endregion

    #region Public Properties

    public object AsyncState {
      get {
        return _state;
      }
    }

    public WaitHandle AsyncWaitHandle {
      get {
        lock (_sync)
          return _waitHandle ?? (_waitHandle = new ManualResetEvent (_completed));
      }
    }

    public bool CompletedSynchronously {
      get {
        return _syncCompleted;
      }
    }

    public bool IsCompleted {
      get {
        lock (_sync)
          return _completed;
      }
    }

    #endregion

    #region Private Methods

    private static void invokeCallback (object state)
    {
      try {
        var ares = (ListenerAsyncResult) state;
        ares._callback (ares);
      }
      catch {
      }
    }

    #endregion

    #region Internal Methods

    internal void Complete (Exception exception)
    {
      _exception = InGet && (exception is ObjectDisposedException)
                   ? new HttpListenerException (500, "Listener closed.")
                   : exception;

      lock (_sync) {
        _completed = true;
        if (_waitHandle != null)
          _waitHandle.Set ();

        if (_callback != null)
          ThreadPool.QueueUserWorkItem(invokeCallback, this);
      }
    }

    internal void Complete (HttpListenerContext context)
    {
      Complete (context, false);
    }

    internal void Complete (HttpListenerContext context, bool syncCompleted)
    {
      var listener = context.Listener;
      var scheme = listener.SelectAuthenticationScheme (context);
      if (scheme == AuthenticationSchemes.None) {
        context.Response.Close (HttpStatusCode.Forbidden);
        listener.BeginGetContext (this);

        return;
      }

      var header = context.Request.Headers ["Authorization"];
      if (scheme == AuthenticationSchemes.Basic &&
          (header == null || !header.StartsWith ("basic", StringComparison.OrdinalIgnoreCase))) {
        context.Response.CloseWithAuthChallenge (
          AuthenticationChallenge.CreateBasicChallenge (listener.Realm).ToBasicString ());

        listener.BeginGetContext (this);
        return;
      }

      if (scheme == AuthenticationSchemes.Digest &&
          (header == null || !header.StartsWith ("digest", StringComparison.OrdinalIgnoreCase))) {
        context.Response.CloseWithAuthChallenge (
          AuthenticationChallenge.CreateDigestChallenge (listener.Realm).ToDigestString ());

        listener.BeginGetContext (this);
        return;
      }

      _context = context;
      _syncCompleted = syncCompleted;

      lock (_sync) {
        _completed = true;
        if (_waitHandle != null)
          _waitHandle.Set ();

        if (_callback != null)
          ThreadPool.QueueUserWorkItem(invokeCallback, this);
      }
    }

    internal HttpListenerContext GetContext ()
    {
      if (_exception != null)
        throw _exception;

      return _context;
    }

    #endregion
  }
}
