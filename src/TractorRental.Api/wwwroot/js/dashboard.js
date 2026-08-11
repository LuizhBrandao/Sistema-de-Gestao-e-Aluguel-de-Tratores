document.addEventListener('DOMContentLoaded', () => {
    initApp('dashboard');
    loadDashboard();
    
    // Auto-refresh every 10 seconds
    setInterval(loadDashboard, 10000);
    
    // SignalR Setup
    setupSignalR();
});

async function loadDashboard() {
    try {
        const [tratores, contratos, clientes] = await Promise.all([
            apiGet('/api/tratores/dashboard'),
            apiGet('/api/contratos'),
            apiGet('/api/clientes')
        ]);
        
        if (!tratores || !contratos || !clientes) return;
        
        // Count statuses
        let operacionais = 0;
        let alugados = 0;
        let manutencao = 0;
        let inativos = 0;
        
        tratores.forEach(t => {
            if (t.status === 'Operacional') operacionais++;
            else if (t.status === 'Alugado') alugados++;
            else if (t.status === 'EmManutencao') manutencao++;
            else if (t.status === 'Inativo') inativos++;
        });
        
        const total = tratores.length;
        
        document.getElementById('stat-total').textContent = total;
        document.getElementById('stat-operacionais').textContent = operacionais;
        document.getElementById('stat-alugados').textContent = alugados;
        document.getElementById('stat-manutencao').textContent = manutencao;
        
        // Render Donut Chart
        renderChart(operacionais, alugados, manutencao, inativos, total);
        
        // Render Summary
        const ativosCount = contratos.filter(c => c.status === 'Ativo').length;
        const clientesCount = clientes.length;
        
        document.getElementById('summary-container').innerHTML = `
            <div style="display:flex; flex-direction:column; gap: 20px; margin-top:20px;">
                <div style="display:flex; justify-content:space-between; align-items:center; padding:15px; background:rgba(255,255,255,0.05); border-radius:8px;">
                    <span style="font-size:1.1rem;">Contratos Ativos</span>
                    <span style="font-size:1.5rem; font-weight:600; color:#3B82F6;">${ativosCount}</span>
                </div>
                <div style="display:flex; justify-content:space-between; align-items:center; padding:15px; background:rgba(255,255,255,0.05); border-radius:8px;">
                    <span style="font-size:1.1rem;">Total de Clientes</span>
                    <span style="font-size:1.5rem; font-weight:600; color:#10B981;">${clientesCount}</span>
                </div>
            </div>
        `;
        
    } catch (error) {
        console.error("Error loading dashboard", error);
    }
}

function renderChart(operacionais, alugados, manutencao, inativos, total) {
    const container = document.getElementById('chart-container');
    
    if (total === 0) {
        container.innerHTML = `<div class="empty-state">
            <div class="empty-state-icon">📊</div>
            <div class="empty-state-text">Nenhum trator cadastrado</div>
        </div>`;
        return;
    }
    
    const segments = [
        { value: operacionais, color: '#10B981', label: 'Operacional' },
        { value: alugados, color: '#3B82F6', label: 'Alugado' },
        { value: manutencao, color: '#EF4444', label: 'Manutenção' },
        { value: inativos, color: '#64748B', label: 'Inativo' }
    ].filter(s => s.value > 0);
    
    let svg = '';
    if (typeof createDonutChart === 'function') {
        svg = createDonutChart(segments, 180);
    }
    
    let legendHtml = '<div class="chart-legend" style="margin-top:20px; display:grid; grid-template-columns: 1fr 1fr; gap:10px;">';
    segments.forEach(s => {
        legendHtml += `
            <div class="legend-item" style="display:flex; align-items:center; gap:8px;">
                <div class="legend-dot" style="width:12px; height:12px; border-radius:50%; background-color:${s.color}"></div>
                <span>${s.label}: </span>
                <span class="legend-value" style="font-weight:bold;">${s.value}</span>
            </div>
        `;
    });
    legendHtml += '</div>';
    
    container.innerHTML = `
        <div class="chart-section" style="display:flex; flex-direction:column; align-items:center;">
            ${svg}
            ${legendHtml}
        </div>
    `;
}

function setupSignalR() {
    if (typeof signalR === 'undefined') return;
    
    const connection = new signalR.HubConnectionBuilder()
        .withUrl("/hubs/monitoramento")
        .withAutomaticReconnect()
        .build();

    connection.on("ReceberAlerta", (alerta) => {
        const container = document.getElementById('recent-alerts');
        
        // Remove empty state if present
        if (container.querySelector('.empty-state')) {
            container.innerHTML = '';
        }
        
        const isCritical = alerta.temperatura > 110;
        const alertClass = isCritical ? 'critical' : 'warning';
        const icon = isCritical ? '⚠️' : '⚠️';
        const timeStr = typeof formatDate === 'function' ? formatDate(new Date().toISOString()) : new Date().toLocaleString();
        
        const alertHtml = `
            <div class="alert-card ${alertClass} fade-in" style="margin-bottom:10px; display:flex; gap:15px; align-items:center; padding:15px; border-radius:8px; background:rgba(0,0,0,0.2); border-left: 4px solid ${isCritical ? '#EF4444' : '#F59E0B'};">
                <div class="alert-icon" style="font-size:1.5rem;">${icon}</div>
                <div class="alert-content" style="flex:1;">
                    <div class="alert-title" style="font-weight:600; margin-bottom:4px;">Trator #${alerta.tratorId} - Alerta</div>
                    <div class="alert-message" style="color:var(--text-muted); font-size:0.9rem;">${alerta.mensagem}</div>
                </div>
                <div class="alert-time" style="font-size:0.8rem; color:var(--text-muted);">${timeStr}</div>
            </div>
        `;
        
        container.insertAdjacentHTML('afterbegin', alertHtml);
        
        // Keep only last 5 alerts
        const cards = container.querySelectorAll('.alert-card');
        if (cards.length > 5) {
            cards[cards.length - 1].remove();
        }
    });

    connection.start().catch(err => console.error("SignalR Connection Error: ", err));
}
