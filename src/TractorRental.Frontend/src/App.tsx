import { useEffect, useState } from 'react';
import * as signalR from '@microsoft/signalr';

import { Gauge } from './components/Gauge';
import { StatusBadge } from './components/StatusBadge';

interface TratorDto {
  id: string;
  marca: string;
  modelo: string;
  status: string;
  temperaturaAtualMotor: number;
  pressaoAtualPneus: number;
  nivelCombustivel: number;
  nivelOleo: number;
  rotacaoMotor: number;
  velocidade: number;
}

interface TelemetryData {
  tractorId: string;
  speed: number;
  fuelLevel: number;
  engineTemp: number;
  timestamp: string;
}

interface Alert {
  id: string;
  tractorId: string;
  message: string;
  severity: 'Warning' | 'Critical';
  timestamp: string;
}

interface RentalContract {
  id: string;
  tractorName: string;
  customerName: string;
  startDate: string;
  endDate: string;
  status: 'Active' | 'Pending' | 'Completed';
}

const MARCAS_TRATOR = [
  'JohnDeere',
  'MasseyFerguson',
  'Valtra',
  'NewHolland',
  'CaseIH',
  'Agrale',
  'Caterpillar',
  'Kubota',
  'Outra',
] as const;

const MARCA_LABELS: Record<string, string> = {
  JohnDeere: 'John Deere',
  MasseyFerguson: 'Massey Ferguson',
  Valtra: 'Valtra',
  NewHolland: 'New Holland',
  CaseIH: 'Case IH',
  Agrale: 'Agrale',
  Caterpillar: 'Caterpillar',
  Kubota: 'Kubota',
  Outra: 'Outra',
};

interface CadastroTratorForm {
  marca: string;
  modelo: string;
  anoFabricacao: string;
  potenciaCv: string;
  horimetroInicial: string;
  numeroSerie: string;
}

const FORM_INICIAL: CadastroTratorForm = {
  marca: '',
  modelo: '',
  anoFabricacao: '',
  potenciaCv: '',
  horimetroInicial: '0',
  numeroSerie: '',
};

function App() {
  const [tratores, setTratores] = useState<TratorDto[]>([]);
  const [alerts, setAlerts] = useState<Alert[]>([]);
  const [connectionStatus, setConnectionStatus] = useState('Disconnected');
  const [modalAberto, setModalAberto] = useState(false);
  const [formData, setFormData] = useState<CadastroTratorForm>(FORM_INICIAL);
  const [formError, setFormError] = useState('');
  const [formSuccess, setFormSuccess] = useState('');
  const [enviando, setEnviando] = useState(false);

  // Dummy rental data
  const rentals: RentalContract[] = [
    { id: '1', tractorName: 'John Deere 8R', customerName: 'AgroFarm LLC', startDate: '2026-08-01', endDate: '2026-08-15', status: 'Active' },
    { id: '2', tractorName: 'Case IH Magnum', customerName: 'GreenFields Inc', startDate: '2026-08-10', endDate: '2026-08-20', status: 'Pending' },
  ];

  useEffect(() => {
    fetch('http://localhost:5000/api/tratores/dashboard')
      .then(res => res.json())
      .then(data => setTratores(data))
      .catch(err => console.error("Error fetching tractors:", err));

    const connection = new signalR.HubConnectionBuilder()
      .withUrl("http://localhost:5000/hubs/monitoramento")
      .withAutomaticReconnect()
      .build();

    setConnectionStatus('Connecting...');

    connection.start()
      .then(() => {
        setConnectionStatus('Connected');
        
        connection.on("ReceiveTelemetry", (data: TelemetryData) => {
          setTratores(prev => prev.map(t => 
            t.id === data.tractorId 
              ? { 
                  ...t, 
                  velocidade: data.speed, 
                  nivelCombustivel: data.fuelLevel, 
                  temperaturaAtualMotor: data.engineTemp 
                } 
              : t
          ));
        });

        connection.on("ReceiveAlert", (alert: Alert) => {
          setAlerts(prev => [alert, ...prev].slice(0, 10)); // Keep last 10
        });
      })
      .catch(err => {
        console.error("SignalR Connection Error: ", err);
        setConnectionStatus('Error Connecting');
      });

    return () => {
      connection.stop();
    };
  }, []);

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    setFormData(prev => ({ ...prev, [e.target.name]: e.target.value }));
  };

  const handleCadastro = async (e: React.FormEvent) => {
    e.preventDefault();
    setFormError('');
    setFormSuccess('');

    if (!formData.marca || !formData.modelo || !formData.anoFabricacao || !formData.potenciaCv || !formData.numeroSerie) {
      setFormError('Preencha todos os campos obrigatórios.');
      return;
    }

    setEnviando(true);
    try {
      const response = await fetch('http://localhost:5000/api/tratores', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          marca: formData.marca,
          modelo: formData.modelo,
          anoFabricacao: parseInt(formData.anoFabricacao),
          potenciaCv: parseInt(formData.potenciaCv),
          horimetroInicial: parseFloat(formData.horimetroInicial) || 0,
          numeroSerie: formData.numeroSerie,
        }),
      });

      if (!response.ok) {
        const err = await response.json();
        throw new Error(err.mensagem || 'Erro ao cadastrar trator.');
      }

      setFormSuccess('Trator cadastrado com sucesso!');
      setFormData(FORM_INICIAL);
      
      // Fetch latest tractors after creation
      fetch('http://localhost:5000/api/tratores/dashboard')
        .then(res => res.json())
        .then(data => setTratores(data))
        .catch(err => console.error("Error fetching tractors:", err));

      setTimeout(() => {
        setModalAberto(false);
        setFormSuccess('');
      }, 1500);
    } catch (err) {
      setFormError(err instanceof Error ? err.message : 'Erro ao cadastrar trator.');
    } finally {
      setEnviando(false);
    }
  };

  return (
    <div className="dashboard-container">
      <header className="glass-header">
        <h1>Gestão de Frota & Telemetria</h1>
        <div style={{ display: 'flex', alignItems: 'center', gap: '1rem' }}>
          <button className="btn-add-trator" onClick={() => { setModalAberto(true); setFormError(''); setFormSuccess(''); }}>
            ＋ Cadastrar Trator
          </button>
          <div className={`status-badge status-${connectionStatus.toLowerCase().replace(' ', '-')}`}>
            <span className="dot"></span>
            {connectionStatus}
          </div>
        </div>
      </header>
      
      <main className="dashboard-grid">
        <section className="glass-card telemetry-section" style={{ gridColumn: '1 / -1' }}>
          <h2>🚜 Frota em Tempo Real</h2>
          <div className="cards-grid" style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fill, minmax(400px, 1fr))', gap: '20px', marginTop: '1rem' }}>
            {tratores.length === 0 ? (
              <p className="empty-state">Nenhum trator cadastrado ou carregando...</p>
            ) : (
              tratores.map(t => (
                <div key={t.id} className="trator-card fade-in">
                  <div className="trator-card-header">
                    <h3 className="trator-card-title">🚜 {t.marca} {t.modelo}</h3>
                    <StatusBadge status={t.status} />
                  </div>
                  <div className="trator-card-body">
                    <div className="gauge-grid">
                      <Gauge value={t.temperaturaAtualMotor} max={150} unit="°C" label="Motor" thresholds={{ dangerAbove: 110, warningAbove: 95 }} />
                      <Gauge value={t.pressaoAtualPneus} max={45} unit="PSI" label="Pneus" thresholds={{ dangerBelow: 26, warningBelow: 28 }} />
                      <Gauge value={t.nivelOleo} max={100} unit="%" label="Óleo" thresholds={{ dangerBelow: 15, warningBelow: 25 }} />
                      <Gauge value={t.rotacaoMotor} max={4500} unit="rpm" label="Rotação" thresholds={{ warningAbove: 3500 }} />
                      <Gauge value={t.nivelCombustivel} max={100} unit="%" label="Combustível" thresholds={{ dangerBelow: 10, warningBelow: 20 }} />
                      <Gauge value={t.velocidade} max={50} unit="km/h" label="Velocidade" />
                    </div>
                  </div>
                </div>
              ))
            )}
          </div>
        </section>

        <section className="glass-card alerts-section">
          <h2>⚠️ Critical Alerts</h2>
          <ul className="alert-list">
            {alerts.length === 0 ? (
              <p className="empty-state">No critical alerts.</p>
            ) : (
              alerts.map(a => (
                <li key={a.id} className={`alert-item ${a.severity.toLowerCase()}`}>
                  <div className="alert-header">
                    <strong>Tractor #{a.tractorId}</strong>
                    <span className="alert-time">{new Date(a.timestamp).toLocaleTimeString()}</span>
                  </div>
                  <p>{a.message}</p>
                </li>
              ))
            )}
          </ul>
        </section>

        <section className="glass-card rentals-section">
          <h2>📝 Active Contracts</h2>
          <div className="contracts-list">
            {rentals.map(r => (
              <div key={r.id} className="contract-item">
                <div className="contract-info">
                  <strong>{r.tractorName}</strong>
                  <span>{r.customerName}</span>
                </div>
                <div className="contract-dates">
                  <small>{r.startDate} to {r.endDate}</small>
                </div>
                <span className={`badge badge-${r.status.toLowerCase()}`}>{r.status}</span>
              </div>
            ))}
          </div>
        </section>
      </main>

      {modalAberto && (
        <div className="modal-overlay" onClick={() => setModalAberto(false)}>
          <div className="modal-content" onClick={e => e.stopPropagation()}>
            <div className="modal-header">
              <h2>🚜 Cadastrar Trator</h2>
              <button className="modal-close" onClick={() => setModalAberto(false)}>✕</button>
            </div>

            {formError && <div className="form-error">{formError}</div>}
            {formSuccess && <div className="form-success">{formSuccess}</div>}

            <form onSubmit={handleCadastro}>
              <div className="form-grid">
                <div className="form-group">
                  <label htmlFor="marca">Marca (Fabricante)</label>
                  <select id="marca" name="marca" value={formData.marca} onChange={handleChange} required>
                    <option value="">Selecione a marca...</option>
                    {MARCAS_TRATOR.map(m => (
                      <option key={m} value={m}>{MARCA_LABELS[m]}</option>
                    ))}
                  </select>
                </div>

                <div className="form-group">
                  <label htmlFor="modelo">Modelo</label>
                  <input id="modelo" name="modelo" type="text" placeholder="Ex: 8R 370" value={formData.modelo} onChange={handleChange} required />
                </div>

                <div className="form-group">
                  <label htmlFor="anoFabricacao">Ano de Fabricação</label>
                  <input id="anoFabricacao" name="anoFabricacao" type="number" placeholder="Ex: 2022" min="1950" max={new Date().getFullYear() + 1} value={formData.anoFabricacao} onChange={handleChange} required />
                </div>

                <div className="form-group">
                  <label htmlFor="potenciaCv">Potência (CV)</label>
                  <input id="potenciaCv" name="potenciaCv" type="number" placeholder="Ex: 370" min="1" value={formData.potenciaCv} onChange={handleChange} required />
                </div>

                <div className="form-group">
                  <label htmlFor="horimetroInicial">Horímetro Inicial (h)</label>
                  <input id="horimetroInicial" name="horimetroInicial" type="number" placeholder="Ex: 1250" min="0" step="0.1" value={formData.horimetroInicial} onChange={handleChange} />
                </div>

                <div className="form-group">
                  <label htmlFor="numeroSerie">Nº de Série (PIN)</label>
                  <input id="numeroSerie" name="numeroSerie" type="text" placeholder="Ex: 1LV8370RCNR000123" value={formData.numeroSerie} onChange={handleChange} required />
                </div>
              </div>

              <div className="form-actions">
                <button type="button" className="btn btn-secondary" onClick={() => setModalAberto(false)}>Cancelar</button>
                <button type="submit" className="btn btn-primary" disabled={enviando}>
                  {enviando ? 'Cadastrando...' : 'Cadastrar Trator'}
                </button>
              </div>
            </form>
          </div>
        </div>
      )}
    </div>
  );
}

export default App;
