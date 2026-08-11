document.addEventListener('DOMContentLoaded', () => {
    initApp('frota');
    loadTratores();
    
    // Auto-refresh every 5 seconds
    setInterval(loadTratoresSilently, 5000);
});

async function loadTratores() {
    const container = document.getElementById('tratores-container');
    container.innerHTML = '<div class="empty-state" style="grid-column: 1 / -1;"><div class="loading-spinner"></div></div>';
    
    try {
        const tratores = await apiGet('/api/tratores/dashboard');
        renderTratores(tratores);
    } catch (error) {
        container.innerHTML = '<div class="empty-state" style="grid-column: 1 / -1;"><div class="empty-state-text text-danger">Erro ao carregar tratores.</div></div>';
    }
}

async function loadTratoresSilently() {
    try {
        const tratores = await apiGet('/api/tratores/dashboard');
        renderTratores(tratores);
    } catch (error) {
        console.error('Erro ao atualizar tratores:', error);
    }
}

function renderTratores(tratores) {
    const container = document.getElementById('tratores-container');
    
    if (!tratores || tratores.length === 0) {
        container.innerHTML = `
            <div class="empty-state" style="grid-column: 1 / -1;">
                <div class="empty-state-icon">🚜</div>
                <div class="empty-state-text">Nenhum trator cadastrado.</div>
            </div>`;
        return;
    }

    const existingCards = container.querySelectorAll('.trator-card');
    
    // If the number of cards has changed, or we are transitioning from an empty state, re-render everything
    if (existingCards.length !== tratores.length || existingCards.length === 0) {
        let html = '';
        tratores.forEach(t => {
            html += generateTratorCardHtml(t);
        });
        container.innerHTML = html;
    } else {
        // Update existing cards to allow smooth gauge transitions
        tratores.forEach(t => {
            const card = document.getElementById(`trator-${t.id}`);
            if (card) {
                const header = card.querySelector('.trator-card-header');
                if (header) {
                    header.innerHTML = `
                        <h3 class="trator-card-title">🚜 ${t.modelo}</h3>
                        ${getStatusBadge(t.status)}
                    `;
                }
                
                const gaugeItems = card.querySelectorAll('.gauge-item');
                if (gaugeItems.length === 6) {
                    gaugeItems[0].innerHTML = createGauge(t.temperaturaAtualMotor, 150, '°C', 'Motor', { dangerAbove: 110, warningAbove: 95 });
                    gaugeItems[1].innerHTML = createGauge(t.pressaoAtualPneus, 45, 'PSI', 'Pneus', { dangerBelow: 26, warningBelow: 28 });
                    gaugeItems[2].innerHTML = createGauge(t.nivelOleo, 100, '%', 'Óleo', { dangerBelow: 15, warningBelow: 25 });
                    gaugeItems[3].innerHTML = createGauge(t.rotacaoMotor, 4500, 'rpm', 'Rotação', { warningAbove: 3500 });
                    gaugeItems[4].innerHTML = createGauge(t.nivelCombustivel, 100, '%', 'Combustível', { dangerBelow: 10, warningBelow: 20 });
                    gaugeItems[5].innerHTML = createGauge(t.velocidade, 50, 'km/h', 'Velocidade', {});
                }
            } else {
                // Fallback just in case
                container.innerHTML = tratores.map(generateTratorCardHtml).join('');
            }
        });
    }
}

function generateTratorCardHtml(t) {
    const temp = createGauge(t.temperaturaAtualMotor, 150, '°C', 'Motor', { dangerAbove: 110, warningAbove: 95 });
    const pressao = createGauge(t.pressaoAtualPneus, 45, 'PSI', 'Pneus', { dangerBelow: 26, warningBelow: 28 });
    const oleo = createGauge(t.nivelOleo, 100, '%', 'Óleo', { dangerBelow: 15, warningBelow: 25 });
    const rpm = createGauge(t.rotacaoMotor, 4500, 'rpm', 'Rotação', { warningAbove: 3500 });
    const combustivel = createGauge(t.nivelCombustivel, 100, '%', 'Combustível', { dangerBelow: 10, warningBelow: 20 });
    const velocidade = createGauge(t.velocidade, 50, 'km/h', 'Velocidade', {});

    return `
        <div class="trator-card fade-in" id="trator-${t.id}">
            <div class="trator-card-header">
                <h3 class="trator-card-title">🚜 ${t.modelo}</h3>
                ${getStatusBadge(t.status)}
            </div>
            <div class="trator-card-body">
                <div class="gauge-grid">
                    <div class="gauge-item">${temp}</div>
                    <div class="gauge-item">${pressao}</div>
                    <div class="gauge-item">${oleo}</div>
                    <div class="gauge-item">${rpm}</div>
                    <div class="gauge-item">${combustivel}</div>
                    <div class="gauge-item">${velocidade}</div>
                </div>
            </div>
        </div>
    `;
}

async function salvarTrator() {
    const inputModelo = document.getElementById('input-modelo');
    const modelo = inputModelo.value.trim();
    
    if (!modelo) {
        showToast('Informe o modelo do trator', 'error');
        return;
    }
    
    try {
        await apiPost('/api/tratores', { modelo });
        showToast('Trator cadastrado com sucesso!', 'success');
        hideModal('modal-trator');
        inputModelo.value = '';
        loadTratores(); // Reload with spinner to show new item
    } catch (error) {
        showToast('Erro ao cadastrar trator', 'error');
    }
}
