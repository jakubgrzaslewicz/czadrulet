"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
require("./css/main.css");
var email = document.querySelector("#email");
var userName = document.querySelector("#login");
var pass = document.querySelector("#pass");
var btnRegister = document.querySelector("#Register");
btnRegister.addEventListener("click", Register);
var Url = "https://czadruletapi20201210205553.azurewebsites.net/Users/register";
function getUser() {
    fetch(Url)
        .then(function (response) { return response.json(); })
        .catch(function (error) { return console.error('Unable to get items.', error); });
}
function Register() {
    var user = {
        "username": userName,
        "email": email,
        "password": pass
    };
    fetch(Url, {
        method: 'POST',
        headers: {
            'Accept': 'text/plain',
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(user)
    })
        .then(function (response) { return response.json(); })
        .catch(function (error) { return console.error('Unable to add item.', error); });
}
