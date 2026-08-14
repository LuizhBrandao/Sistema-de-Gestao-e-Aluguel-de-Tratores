import React from 'react';

export const Clientes: React.FC = () => {
  return (
    <div className="fade-in">
      <header className="page-header">
        <h1>Clientes</h1>
        <p className="page-subtitle">Gerencie os locatários dos seus tratores.</p>
      </header>
      
      <section className="glass-card" style={{ padding: '2rem', textAlign: 'center' }}>
        <div className="empty-state">
          <div className="empty-state-icon">👥</div>
          <p className="empty-state-text">Módulo de Clientes em desenvolvimento</p>
          <p className="empty-state-sub">Em breve você poderá cadastrar e gerenciar perfis aqui.</p>
        </div>
      </section>
    </div>
  );
};
