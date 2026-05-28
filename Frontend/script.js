const apiUrl = "https://localhost:7298/todoitems";

async function adicionarTarefa() {

    const input = document.getElementById("nomeTarefa");

    if (input.value.trim() === "") {
        alert("Digite uma tarefa");
        return;
    }

    await axios.post(apiUrl, {
        name: input.value,
        isComplete: false
    });

    alert("Tarefa adicionada com sucesso!");

    input.value = "";
}