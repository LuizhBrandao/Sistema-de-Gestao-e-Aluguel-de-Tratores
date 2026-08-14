import React from 'react';
import { useOutletContext } from 'react-router-dom';

export const Alertas: React.FC = () => {
  const { alerts } = useOutletContext<any>();

  return (
    <div className="fade-in">
      <header className="page-header">
        <h1>Central de Alertas</h1>
        <p className="page-subtitle">Monitoramento de eventos críticos reportados pela telemetria.</p>
      </header>
      
      <section className="glass-card" style={{ padding: '2rem' }}>
        <div className="alerts-list">
          {!alerts || alerts.length === 0 ? (
            <div className="empty-state">
              <div className="empty-state-icon">✅</div>
              <p className="empty-state-text">Nenhum alerta crítico ativo</p>
            </div>
          ) : (
            alerts.map((a: any) => (
              <div key={a.id} className={`alert-card ${a.severity.toLowerCase()}`}>
                <div className="alert-icon">{a.severity === 'Critical' ? '⚠️' : '🔔'}</div>
                <div className="alert-content">
                  <div className="alert-title">Tractor #{a.tractorId}</div>
                  <div className="alert-message">{a.message}</div>
                </div>
                <div className="alert-time">{new Date(a.timestamp).toLocaleTimeString()}</div>
              </div>
            ))
          )}
        </div>
      </section>
    </div>
  );
};
