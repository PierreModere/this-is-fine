mergeInto(LibraryManager.library, {
  savePlayerData: function (playerID, roomCode) {
    localStorage.setItem("playerID", UTF8ToString(playerID));
    localStorage.setItem("roomCode", UTF8ToString(roomCode));
  },
  getPlayerDataFromLocalStorage: function (property) {
    const returnStr = localStorage.getItem(UTF8ToString(property));
    if (returnStr != null) {
      var bufferSize = lengthBytesUTF8(returnStr) + 1;
      var buffer = _malloc(bufferSize);
      stringToUTF8(returnStr, buffer, bufferSize);
      return buffer;
    }
    return null;
  },

  console: function (str) {
    window.alert(UTF8ToString(str));
  },

  clearLocalStorage: function () {
    localStorage.clear();
    console.log("localstorage vidé !");
  },

  restartGame: function () {
    window.location.reload();
  },

  vibrate: function () {
    const canVibrate = window.navigator.vibrate;
    if (canVibrate) window.navigator.vibrate(200);
  },
});
