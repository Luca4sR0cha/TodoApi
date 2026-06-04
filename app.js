const api = "http://localhost:5136/todoitems"; // Substitua pela porta da sua API

document.getElementById("todoForm").addEventListener("submit", function (event) {
    event.preventDefault();

    const todoItem = {
        name: document.getElementById("name").value,
        telefone: document.getElementById("telefone").value,
        email: document.getElementById("email").value,
        isComplete: document.getElementById("isComplete").checked
    };

    axios.post(apiUrl, todoItem)
        .then(function (response) {
            document.getElementById("response").innerHTML = `<p style="color:green;">Item criado com sucesso! ID: ${response.data.id}</p>`;
        })
        .catch(function (error) {
            console.error(error);
            if (error.response) {
                document.getElementById("response").innerHTML = `<p style="color:red;">Erro: ${error.response.data}</p>`;
            } else {
                document.getElementById("response").innerHTML = `<p style="color:red;">Erro: ${error.message}</p>`;
            }
        });
});