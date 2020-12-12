"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
require("./css/main.css");
var signalR = require("@microsoft/signalr");
var divMessages = document.querySelector("#divMessages");
var tbMessage = document.querySelector("#tbMessage");
var btnSend = document.querySelector("#btnSend");
var btnStart = document.querySelector("#btnStart");
var username = new Date().getTime();
var connection = new signalR.HubConnectionBuilder()
    .withUrl("https://czadruletapi20201210205553.azurewebsites.net/TwoPersonChatHub")
    .withAutomaticReconnect()
    .build();
connection.on("ReceiveMessage", function (username, message) {
    var m = document.createElement("div");
    m.innerHTML =
        "<div class=\"message-author\">" + username + "</div><div>" + message + "</div>";
    divMessages.appendChild(m);
    divMessages.scrollTop = divMessages.scrollHeight;
});
connection.start().catch(function (err) { return document.write(err); });
tbMessage.addEventListener("keyup", function (e) {
    if (e.key === "Enter") {
        send();
    }
});
btnSend.addEventListener("click", send);
btnStart.addEventListener("click", startChat);
function startChat() {
    connection.invoke('OpenChatRoom');
}
function send() {
    connection.invoke('SendMessage', "test", "Witoj");
}
