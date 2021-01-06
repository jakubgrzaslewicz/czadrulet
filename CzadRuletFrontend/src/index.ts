import "./css/main.css";
import * as signalR from "@microsoft/signalr";

const divMessages: HTMLDivElement = document.querySelector("#divMessages");
const tbMessage: HTMLInputElement = document.querySelector("#tbMessage");
const btnSend: HTMLButtonElement = document.querySelector("#btnSend");
const btnStart: HTMLButtonElement = document.querySelector("#btnStart");
const UserName: HTMLInputElement = document.querySelector("#UserName");

const connection = new signalR.HubConnectionBuilder()
    .withUrl("https://czadruletapi20201210205553.azurewebsites.net/TwoPersonChatHub")
    .withAutomaticReconnect()
    .build();

connection.on("ReceiveMessage",
    (username: string, message: string) => {
        let m = document.createElement("div");

        m.innerHTML = 
            `<div class="message">
                <strong>${username}: </strong>${message}
             </div>`;
        divMessages.appendChild(m);
        divMessages.scrollTop = divMessages.scrollHeight;
    });

connection.on("UserJoined",
    () => {
        let m = document.createElement("div");

        m.innerHTML =
            `<div>Anonymous user joined your room. Say hello!</div>`;

        divMessages.appendChild(m);
        divMessages.scrollTop = divMessages.scrollHeight;
    });

connection.on("UserLeft",
    () => {
        let m = document.createElement("div");

        m.innerHTML =
            `<div>Your partner left the room.</div>`;

        divMessages.appendChild(m);
        divMessages.scrollTop = divMessages.scrollHeight;
    });

connection.on("YouHaveJoined",
    (currentUsersCount: number) => {
        let m = document.createElement("div");
        if (currentUsersCount === 2)
            m.innerHTML = `<div>You have joined a room. Say hello to another user!</div>`;
        else
            m.innerHTML = `<div>You have joined an empty room. Wait for someone to join!</div>`;

        divMessages.appendChild(m);
        divMessages.scrollTop = divMessages.scrollHeight;
    });

connection.start().catch(err => document.write(err));

tbMessage.addEventListener("keyup",
    (e: KeyboardEvent) => {
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
    connection.invoke('SendMessage', UserName.value, tbMessage.value);
    tbMessage.value = "";
}