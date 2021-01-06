"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
require("./css/main.css");
var signalR = require("@microsoft/signalr");
var divMessages = document.querySelector("#divMessages");
var tbMessage = document.querySelector("#tbMessage");
var btnSend = document.querySelector("#btnSend");
var btnStart = document.querySelector("#btnStart");
var UserName = document.querySelector("#UserName");
var connection = new signalR.HubConnectionBuilder()
    .withUrl("https://czadruletapi20201210205553.azurewebsites.net/TwoPersonChatHub")
    .withAutomaticReconnect()
    .build();
connection.on("ReceiveMessage", function (username, message) {
    var m = document.createElement("div");
    m.innerHTML =
        "<div class=\"message\">\n                <strong>" + username + ": </strong>" + message + "\n             </div>";
    divMessages.appendChild(m);
    divMessages.scrollTop = divMessages.scrollHeight;
});
connection.on("UserJoined", function () {
    var m = document.createElement("div");
    m.innerHTML =
        "<div>Anonymous user joined your room. Say hello!</div>";
    divMessages.appendChild(m);
    divMessages.scrollTop = divMessages.scrollHeight;
});
connection.on("UserLeft", function () {
    var m = document.createElement("div");
    m.innerHTML =
        "<div>Your partner left the room.</div>";
    divMessages.appendChild(m);
    divMessages.scrollTop = divMessages.scrollHeight;
});
connection.on("YouHaveJoined", function (currentUsersCount) {
    var m = document.createElement("div");
    if (currentUsersCount === 2)
        m.innerHTML = "<div>You have joined a room. Say hello to another user!</div>";
    else
        m.innerHTML = "<div>You have joined an empty room. Wait for someone to join!</div>";
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
    divMessages.innerHTML = '';
}
function send() {
    console.log(UserName);
    connection.invoke('SendMessage', UserName.value, tbMessage.value);
}
