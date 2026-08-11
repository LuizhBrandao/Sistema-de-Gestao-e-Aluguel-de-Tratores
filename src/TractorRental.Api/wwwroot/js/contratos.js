document.addEventListener('DOMContentLoaded', () => {
    initApp('contratos');
    loadContratos();
});

async function loadContratos() {
    const loading = document.getElementById('loading-contratos');
    const empty = document.getElementById('empty-contratos');
    const container = document.getElementById('container-contratos');
    const tbody = document.querySelector('#tabela-contratos tbody');
    const countAtivos = document.getElementById('count-ativos');
    const countFinalizados = document.getElementById('count-finalizados');

    loading.style.display = 'flex';
    empty.style.display = 'none';
    container.style.display = 'none';

    try {
        const contratos = await apiGet('/api/contratos');
        
        loading.style.display = 'none';

        if (contratos.length === 0) {
            empty.style.display = 'flex';
            countAtivos.textContent = '0';
            countFinalizados.textContent = '0';
        } else {
            let ativos = 0;
            let finalizados = 0;
            
            tbody.innerHTML = '';
            contratos.forEach(contrato => {
                if (contrato.status === 'Ativo') ativos++;
                else finalizados++;
                
                const tr = document.createElement('tr');
                tr.innerHTML = `
                    <td class="mono">${contrato.id.substring(0, 8)}</td>
                    <td class="mono">${contrato.clienteId.substring(0, 8)}</td>
                    <td class="mono">${contrato.tratorId.substring(0, 8)}</td>
                    <td>${formatDate(contrato.dataInicio)}</td>
                    <td>R$ ${contrato.valorHora.toFixed(2)}</td>
                    <td>${getStatusBadge(contrato.status)}</td>
                `;
                tbody.appendChild(tr);
            });
            
            countAtivos.textContent = ativos;
            countFinalizados.textContent = finalizados;
            
            container.style.display = 'block';
        }
    } catch (error) {
        loading.style.display = 'none';
        showToast('Erro ao carregar contratos', 'error');
    }
}

async function openNovoContrato() {
    try {
        const selectCliente = document.getElementById('select-cliente');
        const selectTrator = document.getElementById('select-trator');
        
        // Fetch data
        const [clientes, tratores] = await Promise.all([
            apiGet('/api/clientes'),
            apiGet('/api/tratores/dashboard')
        ]);
        
        // Populate Clientes
        selectCliente.innerHTML = '<option value="">Selecione um cliente...</option>';
        clientes.forEach(c => {
            const option = document.createElement('option');
            option.value = c.id;
            option.textContent = `${c.nome} (${c.documento})`;
            selectCliente.appendChild(option);
        });
        
        // Populate Tratores (only Operacional)
        selectTrator.innerHTML = '<option value="">Selecione um trator...</option>';
        const tratoresOperacionais = tratores.filter(t => t.status === 'Operacional');
        tratoresOperacionais.forEach(t => {
            const option = document.createElement('option');
            option.value = t.id;
            option.textContent = `${t.modelo} (${t.id.substring(0,8)})`;
            selectTrator.appendChild(option);
        });
        
        showModal('modal-contrato');
    } catch (error) {
        showToast('Erro ao carregar dados para o formulário', 'error');
    }
}

async function salvarContrato() {
    const clienteId = document.getElementById('select-cliente').value;
    const tratorId = document.getElementById('select-trator').value;
    const valorStr = document.getElementById('input-valor').value;
    
    if (!clienteId || !tratorId || !valorStr) {
        showToast('Preencha todos os campos', 'error');
        return;
    }
    
    const valorHora = parseFloat(valorStr);

    try {
        await apiPost('/api/contratos', { clienteId, tratorId, valorHora });
        showToast('Contrato criado com sucesso!', 'success');
        hideModal('modal-contrato');
        
        // Reset form
        document.getElementById('select-cliente').value = '';
        document.getElementById('select-trator').value = '';
        document.getElementById('input-valor').value = '';
        
        loadContratos();
    } catch (error) {
        showToast(error.message || 'Erro ao criar contrato', 'error');
    }
}
