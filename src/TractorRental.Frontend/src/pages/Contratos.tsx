import React from 'react';

export const Contratos: React.FC = () => {
  return (
    <div className="fade-in">
      <header className="page-header">
        <h1>Contratos de Aluguel</h1>
        <p className="page-subtitle">Acompanhe as locações ativas e o histórico.</p>
      </header>
      
      <section className="glass-card" style={{ padding: '2rem', textAlign: 'center' }}>
        <div className="empty-state">
          <div className="empty-state-icon">📋</div>
          <p className="empty-state-text">Módulo de Contratos em desenvolvimento</p>
          <p className="empty-state-sub">Em breve você poderá gerenciar as datas de aluguel por aqui.</p>
        </div>
      </section>
    </div>
  );
};
