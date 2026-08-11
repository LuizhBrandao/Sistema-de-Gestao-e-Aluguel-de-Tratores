let totalAlerts = 0;
let connection = null;

document.addEventListener('DOMContentLoaded', () => {
    initApp('alertas');
    connectSignalR();
});

async function connectSignalR() {
    const statusEl = document.getElementById('conn-status');
    
    connection = new signalR.HubConnectionBuilder()
        .withUrl('/hubs/monitoramento')
        .withAutomaticReconnect()
        .build();

    connection.on('ReceberAlerta', (data) => {
        handleAlerta(data);
    });

    connection.onreconnecting(() => {
        statusEl.className = 'connection-status disconnected';
        statusEl.textContent = '● Reconectando...';
    });

    connection.onreconnected(() => {
        statusEl.className = 'connection-status connected';
        statusEl.textContent = '● Conectado';
    });

    try {
        await connection.start();
        statusEl.className = 'connection-status connected';
        statusEl.textContent = '● Conectado';
    } catch (err) {
        console.error('SignalR Connection Error: ', err);
        statusEl.className = 'connection-status disconnected';
        statusEl.textContent = '● Erro de Conexão';
        setTimeout(connectSignalR, 5000);
    }
}

function handleAlerta(data) {
    const { tratorId, temperatura, mensagem } = data;
    const container = document.getElementById('alertas-container');
    const emptyState = document.getElementById('empty-alertas');
    
    if (emptyState) {
        emptyState.remove();
    }
    
    totalAlerts++;
    document.getElementById('alert-count').textContent = totalAlerts;
    
    // Visual flash effect on header
    const header = document.getElementById('alertas-page-header');
    header.style.backgroundColor = 'rgba(239, 68, 68, 0.1)';
    setTimeout(() => {
        header.style.backgroundColor = '';
    }, 500);

    const timeString = new Date().toLocaleTimeString('pt-BR');
    
    const card = document.createElement('div');
    card.className = 'alert-card critical slide-up';
    card.innerHTML = `
        <div class="alert-icon">🔥</div>
        <div class="alert-content">
            <div class="alert-title">Alerta Trator <span class="mono">${tratorId.substring(0, 8)}</span></div>
            <div class="alert-message">${mensagem} (Temp: ${temperatura ? temperatura.toFixed(1) + '°C' : 'N/A'})</div>
        </div>
        <div class="alert-time">${timeString}</div>
    `;
    
    container.prepend(card);
}
