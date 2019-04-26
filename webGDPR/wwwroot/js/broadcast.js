"use strict";

var connection = new signalR.HubConnectionBuilder().withUrl("/broadcastHub").build();

connection.on("ReceiveMessage", function (user, message) {
  var msg = message.replace(/&/g, "&amp;").replace(/</g, "&lt;").replace(/>/g, "&gt;");
  alert(msg);
});

connection.start().then(function () {  
}).catch(function (err) {
  return console.error(err.toString());
});