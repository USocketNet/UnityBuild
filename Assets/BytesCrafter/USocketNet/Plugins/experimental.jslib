
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