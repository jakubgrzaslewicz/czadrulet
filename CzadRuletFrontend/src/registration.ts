import "./css/main.css";

const email: HTMLDivElement = document.querySelector("#email");
const userName: HTMLDivElement = document.querySelector("#login");
const pass: HTMLDivElement = document.querySelector("#pass");
const btnRegister: HTMLButtonElement = document.querySelector("#Register");

btnRegister.addEventListener("click", Register);

const Url = "https://czadruletapi20201210205553.azurewebsites.net/Users/register";

function getUser() {
    fetch(Url)
        .then(response => response.json())
        .catch(error => console.error('Unable to get items.', error));
}

function Register() {
    const user = {
        "username": userName,
        "email": email,
        "password": pass
    }

    fetch(Url, {
        method: 'POST',
        headers: {
            'Accept': 'text/plain',
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(user)
        })
        .then(response => response.json())
        .catch(error => console.error('Unable to add item.', error));
    }