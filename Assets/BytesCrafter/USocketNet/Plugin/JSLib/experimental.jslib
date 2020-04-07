
//#region License
/*
 * experimental.cs
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
//#endregion

var gameJs = {
  

  ConnectToServer: function () {
    console.log('Connection called!');

    var usocketnet = {};
      usocketnet.url = '192.168.1.2';
      usocketnet.port = '3000';
      usocketnet.auth = 'SeCuReHaSkEy123';

    //Used to connect to the server yet we need to eliminate this one. ak must be in server json.
    var userInfo = {};
      userInfo.ak = 'SeCuReHaSkEy123';
      userInfo.un = 'Demoguy'; //Temporary - Move to datavice.
      userInfo.pw = '1234567'; //Temporary - Move to datavice.

    var socket = io( usocketnet.url + ':' + usocketnet.port);
      gameInstance.SendMessage('USocketManager', 'Connecting', 'ID: ' + socket.id + test );
  },

  DisconnectFromServer: function () {
    console.log('Disconnection called!');
    
  },

  CallbackReturn: function(data, callback) {
    var newss = data;
    newss.strings = newss.strings + "latinus";
    newss.integers = newss.integers * 3;
    newss.floatings = newss.floatings * 2;
    
    callback( newss );
  },

CallbackReturnee: function( data ) {
    var newss = JSON.parse( data );
    newss.strings = newss.strings + "latinus";
    newss.integers = newss.integers * 3;
    newss.floatings = newss.floatings * 2;
    gameInstance.SendMessage('USocketManager', 'OnCallbackResult', JSON.stringify( newss ) );
  },









  CallTheBrowser: function (data) {
    gameInstance.SendMessage('USocketManager', 'Connecting', 'asdasdas');
  },

  Hello: function () {
    window.alert("Hello, world!");
  },

  HelloString: function (str) {
    window.alert(Pointer_stringify(str));
  },

  PrintFloatArray: function (array, size) {
    for(var i = 0; i < size; i++)
    console.log(HEAPF32[(array >> 2) + i]);
  },

  AddNumbers: function (x, y) {
    return x + y;
  },

  StringReturnValueFunction: function () {
    var returnStr = "bla";
    var bufferSize = lengthBytesUTF8(returnStr) + 1;
    var buffer = _malloc(bufferSize);
    stringToUTF8(returnStr, buffer, bufferSize);
    return buffer;
  },

  BindWebGLTexture: function (texture) {
    GLctx.bindTexture(GLctx.TEXTURE_2D, GL.textures[texture]);
  },

};

mergeInto(LibraryManager.library, gameJs);