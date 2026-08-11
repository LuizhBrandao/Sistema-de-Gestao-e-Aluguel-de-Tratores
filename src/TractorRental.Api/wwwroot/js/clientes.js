document.addEventListener('DOMContentLoaded', () => {
    initApp('clientes');
    loadClientes();
});

async function loadClientes() {
    const loading = document.getElementById('loading-clientes');
    const empty = document.getElementById('empty-clientes');
    const container = document.getElementById('container-clientes');
    const tbody = document.querySelector('#tabela-clientes tbody');
    const countBadge = document.getElementById('count-clientes');

    loading.style.display = 'flex';
    empty.style.display = 'none';
    container.style.display = 'none';

    try {
        const clientes = await apiGet('/api/clientes');
        
        loading.style.display = 'none';
        countBadge.textContent = clientes.length;

        if (clientes.length === 0) {
            empty.style.display = 'flex';
        } else {
            tbody.innerHTML = '';
            clientes.forEach(cliente => {
                const tr = document.createElement('tr');
                tr.innerHTML = `
                    <td>${cliente.nome}</td>
                    <td class="mono">${cliente.documento}</td>
                    <td>${cliente.dataCadastro ? formatDate(cliente.dataCadastro) : '—'}</td>
                `;
                tbody.appendChild(tr);
            });
            container.style.display = 'block';
        }
    } catch (error) {
        loading.style.display = 'none';
        showToast('Erro ao carregar clientes', 'error');
    }
}

async function salvarCliente() {
    const nome = document.getElementById('input-nome').value.trim();
    const documento = document.getElementById('input-documento').value.trim();

    if (!nome || !documento) {
        showToast('Preencha todos os campos', 'error');
        return;
    }

    try {
        await apiPost('/api/clientes', { nome, documento });
        showToast('Cliente cadastrado com sucesso!', 'success');
        hideModal('modal-cliente');
        
        // Reset form
        document.getElementById('input-nome').value = '';
        document.getElementById('input-documento').value = '';
        
        loadClientes();
    } catch (error) {
        showToast(error.message || 'Erro ao cadastrar cliente', 'error');
    }
}
