const apiUrl = "http://localhost:5136/todoitems";

async function adicionarTarefa() {
    const name = document.getElementById("nomeTarefa").value.trim();
    const telefone = document.getElementById("telefone").value.trim();
    const email = document.getElementById("email").value.trim();

    if (name === "") {
        alert("Digite uma tarefa");
        return;
    }

    try {
        await axios.post(apiUrl, {
            name: name,
            telefone: telefone,
            email: email,
            isComplete: false
        });

        alert("Tarefa adicionada com sucesso!");

        document.getElementById("nomeTarefa").value = "";
        document.getElementById("telefone").value = "";
        document.getElementById("email").value = "";

        await carregarTarefas();
    } catch (error) {
        console.error(error);
        alert("Erro ao adicionar tarefa: " + (error.response?.data ?? error.message));
    }
}

async function carregarTarefas() {
    try {
        const response = await axios.get(apiUrl);
        const lista = document.getElementById("listaTarefas");
        lista.innerHTML = "";

        response.data.forEach(todo => {
            const li = document.createElement("li");
            li.innerHTML = `
                <strong>${todo.name}</strong>
                ${todo.email ? `— ${todo.email}` : ""}
                ${todo.telefone ? `| ${todo.telefone}` : ""}
                <span style="float:right">
                    <button onclick="excluirTarefa(${todo.id})" style="background:#e53935;margin-left:8px">Excluir</button>
                </span>
            `;
            lista.appendChild(li);
        });
    } catch (error) {
        console.error("Erro ao carregar tarefas:", error);
    }
}

async function excluirTarefa(id) {
    if (!confirm("Deseja excluir esta tarefa?")) return;
    try {
        await axios.delete(`${apiUrl}/${id}`);
        await carregarTarefas();
    } catch (error) {
        alert("Erro ao excluir: " + (error.response?.data ?? error.message));
    }
}

// Carrega ao abrir a página
carregarTarefas();
