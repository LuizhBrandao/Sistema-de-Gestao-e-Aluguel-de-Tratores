// ============================================
// APP.JS — Shared Navigation & Utilities
// ============================================

const NAV_ITEMS = [
  { href: '/',               icon: '📊', text: 'Dashboard',  id: 'dashboard' },
  { href: '/tratores.html',  icon: '🚜', text: 'Frota',      id: 'frota' },
  { href: '/clientes.html',  icon: '👥', text: 'Clientes',   id: 'clientes' },
  { href: '/contratos.html', icon: '📋', text: 'Contratos',  id: 'contratos' },
  { href: '/alertas.html',   icon: '🔔', text: 'Alertas',    id: 'alertas' },
];

// ---- Initialize App ----
function initApp(activePageId) {
  const sidebar = document.getElementById('sidebar');
  if (!sidebar) return;

  const navLinks = NAV_ITEMS.map(item => `
    <a href="${item.href}" class="nav-link ${item.id === activePageId ? 'active' : ''}">
      <span class="nav-icon">${item.icon}</span>
      <span class="nav-text">${item.text}</span>
    </a>
  `).join('');

  sidebar.innerHTML = `
    <div class="sidebar-header">
      <div class="sidebar-logo">
        <div class="sidebar-logo-icon">🚜</div>
        <div class="sidebar-logo-text">
          TractorRental
          <span>Sistema de Gestão</span>
        </div>
      </div>
    </div>
    <nav class="sidebar-nav">${navLinks}</nav>
    <div class="sidebar-footer">© ${new Date().getFullYear()} TractorRental v2.0</div>
  `;

  // Mobile menu
  const mobileHeader = document.querySelector('.mobile-header');
  if (mobileHeader) {
    const toggle = mobileHeader.querySelector('.mobile-toggle');
    const overlay = document.querySelector('.mobile-overlay');
    if (toggle) {
      toggle.addEventListener('click', () => {
        sidebar.classList.toggle('open');
        overlay && overlay.classList.toggle('active');
      });
    }
    if (overlay) {
      overlay.addEventListener('click', () => {
        sidebar.classList.remove('open');
        overlay.classList.remove('active');
      });
    }
  }
}

// ---- API Helpers ----
async function apiGet(url) {
  const res = await fetch(url);
  if (!res.ok) throw new Error(`Erro ${res.status}`);
  return res.json();
}

async function apiPost(url, data) {
  const res = await fetch(url, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(data),
  });
  const result = await res.json().catch(() => ({}));
  if (!res.ok) throw result;
  return result;
}

// ---- Toast Notifications ----
function showToast(message, type = 'success') {
  const container = document.getElementById('toast-container');
  if (!container) return;
  const toast = document.createElement('div');
  toast.className = `toast toast-${type}`;
  toast.innerHTML = `<span>${type === 'success' ? '✅' : '❌'}</span> ${message}`;
  container.appendChild(toast);
  setTimeout(() => toast.remove(), 4000);
}

// ---- Modal Helpers ----
function showModal(modalId) {
  document.getElementById(modalId)?.classList.add('active');
}

function hideModal(modalId) {
  document.getElementById(modalId)?.classList.remove('active');
}

// ---- Date Formatting ----
function formatDate(dateString) {
  if (!dateString) return '—';
  return new Date(dateString).toLocaleDateString('pt-BR', {
    day: '2-digit', month: '2-digit', year: 'numeric',
    hour: '2-digit', minute: '2-digit',
  });
}

// ---- Status Badge Helper ----
function getStatusBadge(status) {
  const map = {
    'Operacional':  { cls: 'badge-success', dot: 'green',  label: 'Operacional' },
    'Alugado':      { cls: 'badge-info',    dot: 'amber',  label: 'Alugado' },
    'EmManutencao': { cls: 'badge-danger',  dot: 'red',    label: 'Manutenção' },
    'Inativo':      { cls: 'badge-muted',   dot: 'gray',   label: 'Inativo' },
    'Ativo':        { cls: 'badge-success', dot: 'green',  label: 'Ativo' },
    'Finalizado':   { cls: 'badge-muted',   dot: 'gray',   label: 'Finalizado' },
    'Cancelado':    { cls: 'badge-danger',  dot: 'red',    label: 'Cancelado' },
  };
  const c = map[status] || { cls: 'badge-muted', dot: 'gray', label: status };
  return `<span class="badge ${c.cls}"><span class="status-dot ${c.dot}"></span>${c.label}</span>`;
}

// ---- SVG Gauge Creator ----
function createGauge(value, max, unit, label, thresholds) {
  const R = 35;
  const C = 2 * Math.PI * R;
  const pct = Math.min(Math.max(value / max, 0), 1);
  const arc = C * 0.75;
  const offset = arc - (pct * arc);

  let color = 'var(--accent)';
  if (thresholds) {
    if (thresholds.dangerAbove != null && value > thresholds.dangerAbove) color = 'var(--danger)';
    else if (thresholds.warningAbove != null && value > thresholds.warningAbove) color = 'var(--warning)';
    else if (thresholds.dangerBelow != null && value < thresholds.dangerBelow) color = 'var(--danger)';
    else if (thresholds.warningBelow != null && value < thresholds.warningBelow) color = 'var(--warning)';
  }

  const display = typeof value === 'number' ? value.toFixed(1) : value;

  return `
    <div class="gauge-item">
      <svg class="gauge-svg" viewBox="0 0 100 100">
        <circle class="gauge-track" cx="50" cy="50" r="${R}"
          stroke-dasharray="${arc} ${C - arc}" transform="rotate(135 50 50)"/>
        <circle class="gauge-fill" cx="50" cy="50" r="${R}"
          stroke="${color}"
          stroke-dasharray="${arc} ${C - arc}"
          stroke-dashoffset="${offset}"
          transform="rotate(135 50 50)"/>
        <text class="gauge-text" x="50" y="46">${display}</text>
        <text class="gauge-unit" x="50" y="60">${unit}</text>
      </svg>
      <span class="gauge-label">${label}</span>
    </div>`;
}

// ---- SVG Donut Chart ----
function createDonutChart(segments, size = 140) {
  const R = 50, C = 2 * Math.PI * R;
  const total = segments.reduce((s, seg) => s + seg.value, 0);
  if (total === 0) return '<div class="empty-state"><p class="text-muted">Sem dados</p></div>';

  let cumulative = 0;
  const arcs = segments.map(seg => {
    const pct = seg.value / total;
    const dash = pct * C;
    const offset = -cumulative * C;
    cumulative += pct;
    return `<circle cx="60" cy="60" r="${R}" fill="none" stroke="${seg.color}"
      stroke-width="14" stroke-dasharray="${dash} ${C - dash}"
      stroke-dashoffset="${offset}" transform="rotate(-90 60 60)"/>`;
  }).join('');

  return `
    <div class="donut-wrap" style="width:${size}px;height:${size}px">
      <svg viewBox="0 0 120 120" width="${size}" height="${size}">${arcs}</svg>
      <div class="donut-center">
        <span class="donut-value">${total}</span>
        <span class="donut-label">Total</span>
      </div>
    </div>`;
}
