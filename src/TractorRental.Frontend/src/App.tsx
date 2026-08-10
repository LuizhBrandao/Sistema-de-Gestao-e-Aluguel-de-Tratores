import { useEffect, useState } from 'react';
import * as signalR from '@microsoft/signalr';

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

function App() {
  const [telemetry, setTelemetry] = useState<TelemetryData[]>([]);
  const [alerts, setAlerts] = useState<Alert[]>([]);
  const [connectionStatus, setConnectionStatus] = useState('Disconnected');

  // Dummy rental data
  const rentals: RentalContract[] = [
    { id: '1', tractorName: 'John Deere 8R', customerName: 'AgroFarm LLC', startDate: '2026-08-01', endDate: '2026-08-15', status: 'Active' },
    { id: '2', tractorName: 'Case IH Magnum', customerName: 'GreenFields Inc', startDate: '2026-08-10', endDate: '2026-08-20', status: 'Pending' },
  ];

  useEffect(() => {
    const connection = new signalR.HubConnectionBuilder()
      .withUrl("http://localhost:5000/hubs/monitoramento")
      .withAutomaticReconnect()
      .build();

    setConnectionStatus('Connecting...');

    connection.start()
      .then(() => {
        setConnectionStatus('Connected');
        
        connection.on("ReceiveTelemetry", (data: TelemetryData) => {
          setTelemetry(prev => {
            const index = prev.findIndex(t => t.tractorId === data.tractorId);
            if (index >= 0) {
              const newTelemetry = [...prev];
              newTelemetry[index] = data;
              return newTelemetry;
            }
            return [...prev, data];
          });
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

  return (
    <div className="dashboard-container">
      <header className="glass-header">
        <h1>Tractor Rental & Telemetry</h1>
        <div className={`status-badge status-${connectionStatus.toLowerCase().replace(' ', '-')}`}>
          <span className="dot"></span>
          {connectionStatus}
        </div>
      </header>
      
      <main className="dashboard-grid">
        <section className="glass-card telemetry-section">
          <h2>🚜 Real-time Telemetry</h2>
          <div className="telemetry-grid">
            {telemetry.length === 0 ? (
              <p className="empty-state">Waiting for telemetry data...</p>
            ) : (
              telemetry.map(t => (
                <div key={t.tractorId} className="telemetry-item">
                  <h3>Tractor #{t.tractorId}</h3>
                  <p>Speed: {t.speed} km/h</p>
                  <p>Fuel: {t.fuelLevel}%</p>
                  <p>Temp: {t.engineTemp}°C</p>
                  <span className="timestamp">{new Date(t.timestamp).toLocaleTimeString()}</span>
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
    </div>
  );
}

export default App;
