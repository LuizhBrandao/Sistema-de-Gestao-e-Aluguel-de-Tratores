import React from 'react';

export const Dashboard: React.FC = () => {
  return (
    <div className="fade-in">
      <header className="page-header">
        <h1>Visão Geral</h1>
        <p className="page-subtitle">Acompanhe as estatísticas principais da sua frota.</p>
      </header>
      
      <div className="stats-grid">
        <div className="stat-card accent-info">
          <div className="stat-icon blue">🚜</div>
          <div className="stat-value">12</div>
          <div className="stat-label">Total de Tratores</div>
        </div>
        <div className="stat-card accent-warning">
          <div className="stat-icon amber">📋</div>
          <div className="stat-value">8</div>
          <div className="stat-label">Contratos Ativos</div>
        </div>
        <div className="stat-card accent-danger">
          <div className="stat-icon red">🔔</div>
          <div className="stat-value">3</div>
          <div className="stat-label">Alertas Críticos</div>
        </div>
        <div className="stat-card accent-success">
          <div className="stat-icon green">✅</div>
          <div className="stat-value">100%</div>
          <div className="stat-label">Operacionalidade</div>
        </div>
      </div>
      
      <section className="glass-card" style={{ padding: '2rem' }}>
        <h2>Bem-vindo ao TractorRental v2.0</h2>
        <p className="text-secondary" style={{ marginTop: '1rem' }}>
          Utilize o menu lateral para navegar entre as diferentes áreas do sistema.
          Acesse a aba <strong>Frota</strong> para visualizar a telemetria em tempo real e os novos gráficos circulares que adicionamos!
        </p>
      </section>
    </div>
  );
};
