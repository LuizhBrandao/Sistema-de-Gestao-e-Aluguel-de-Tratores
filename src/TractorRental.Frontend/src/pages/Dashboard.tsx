import React, { useEffect, useState } from 'react';
import { useOutletContext } from 'react-router-dom';

export const Dashboard: React.FC = () => {
  const { tratores, alerts } = useOutletContext<any>();
  const [estatisticasExtras, setEstatisticasExtras] = useState({ clientes: 0, contratosAtivos: 0 });

  useEffect(() => {
    // Carrega dados extras do Resumo Operacional
    Promise.all([
      fetch('http://localhost:5257/api/clientes').then(res => res.json()),
      fetch('http://localhost:5257/api/contratos').then(res => res.json())
    ]).then(([clientes, contratos]) => {
      setEstatisticasExtras({
        clientes: clientes.length || 0,
        contratosAtivos: contratos.filter((c: any) => c.status === 'Ativo').length || 0
      });
    }).catch(err => console.error("Erro ao carregar estatisticas extras", err));
  }, []);

  const totalTratores = tratores.length;
  const operacionais = tratores.filter((t: any) => t.status === 'Operacional').length;
  const alugados = tratores.filter((t: any) => t.status === 'Alugado').length;
  const emManutencao = tratores.filter((t: any) => t.status === 'EmManutencao').length;
  const inativos = tratores.filter((t: any) => t.status === 'Inativo').length;

  return (
    <div className="fade-in">
      <header className="page-header" style={{ marginBottom: '2rem' }}>
        <h1 style={{ fontSize: '1.8rem', margin: 0 }}>Dashboard</h1>
        <p className="page-subtitle" style={{ margin: 0, color: 'var(--text-secondary)' }}>Visão geral da frota e operações</p>
      </header>
      
      <div className="stats-grid">
        <div className="stat-card" style={{ padding: '20px' }}>
          <div style={{ display: 'flex', alignItems: 'center', gap: '10px', marginBottom: '20px' }}>
            <div style={{ width: '32px', height: '32px', borderRadius: '6px', background: 'rgba(0, 255, 204, 0.1)', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>🚜</div>
            <span style={{ fontSize: '0.85rem', color: 'var(--text-secondary)', textTransform: 'uppercase', letterSpacing: '0.05em' }}>Total de Tratores</span>
          </div>
          <div className="stat-value" style={{ fontSize: '2.5rem' }}>{totalTratores}</div>
        </div>
        
        <div className="stat-card" style={{ padding: '20px' }}>
          <div style={{ display: 'flex', alignItems: 'center', gap: '10px', marginBottom: '20px' }}>
            <div style={{ width: '32px', height: '32px', borderRadius: '6px', background: 'rgba(16, 185, 129, 0.1)', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>✅</div>
            <span style={{ fontSize: '0.85rem', color: 'var(--text-secondary)', textTransform: 'uppercase', letterSpacing: '0.05em' }}>Operacionais</span>
          </div>
          <div className="stat-value" style={{ fontSize: '2.5rem' }}>{operacionais}</div>
        </div>

        <div className="stat-card" style={{ padding: '20px' }}>
          <div style={{ display: 'flex', alignItems: 'center', gap: '10px', marginBottom: '20px' }}>
            <div style={{ width: '32px', height: '32px', borderRadius: '6px', background: 'rgba(59, 130, 246, 0.1)', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>📋</div>
            <span style={{ fontSize: '0.85rem', color: 'var(--text-secondary)', textTransform: 'uppercase', letterSpacing: '0.05em' }}>Alugados</span>
          </div>
          <div className="stat-value" style={{ fontSize: '2.5rem' }}>{alugados}</div>
        </div>

        <div className="stat-card" style={{ padding: '20px' }}>
          <div style={{ display: 'flex', alignItems: 'center', gap: '10px', marginBottom: '20px' }}>
            <div style={{ width: '32px', height: '32px', borderRadius: '6px', background: 'rgba(239, 68, 68, 0.1)', display: 'flex', alignItems: 'center', justifyContent: 'center' }}>🔧</div>
            <span style={{ fontSize: '0.85rem', color: 'var(--text-secondary)', textTransform: 'uppercase', letterSpacing: '0.05em' }}>Em Manutenção</span>
          </div>
          <div className="stat-value" style={{ fontSize: '2.5rem' }}>{emManutencao}</div>
        </div>
      </div>
      
      <div style={{ display: 'grid', gridTemplateColumns: '1fr 1fr', gap: '20px', marginBottom: '20px' }}>
        <section className="glass-card" style={{ padding: '2rem' }}>
          <h3 style={{ fontSize: '0.85rem', color: 'var(--text-secondary)', textTransform: 'uppercase', letterSpacing: '0.05em', marginBottom: '1.5rem' }}>Distribuição da Frota</h3>
          
          {totalTratores === 0 ? (
             <div className="empty-state" style={{ minHeight: '150px' }}>Nenhum dado disponível</div>
          ) : (
            <div style={{ display: 'flex', flexDirection: 'column', gap: '15px' }}>
              <div>
                <div style={{ display: 'flex', justifyContent: 'space-between', fontSize: '0.9rem', marginBottom: '5px' }}>
                  <span>Operacionais</span>
                  <span style={{ fontWeight: 'bold' }}>{Math.round((operacionais / totalTratores) * 100)}%</span>
                </div>
                <div style={{ width: '100%', height: '8px', background: 'rgba(255,255,255,0.05)', borderRadius: '4px', overflow: 'hidden' }}>
                  <div style={{ width: `${(operacionais / totalTratores) * 100}%`, height: '100%', background: '#10B981', borderRadius: '4px' }}></div>
                </div>
              </div>
              
              <div>
                <div style={{ display: 'flex', justifyContent: 'space-between', fontSize: '0.9rem', marginBottom: '5px' }}>
                  <span>Alugados</span>
                  <span style={{ fontWeight: 'bold' }}>{Math.round((alugados / totalTratores) * 100)}%</span>
                </div>
                <div style={{ width: '100%', height: '8px', background: 'rgba(255,255,255,0.05)', borderRadius: '4px', overflow: 'hidden' }}>
                  <div style={{ width: `${(alugados / totalTratores) * 100}%`, height: '100%', background: '#3B82F6', borderRadius: '4px' }}></div>
                </div>
              </div>

              <div>
                <div style={{ display: 'flex', justifyContent: 'space-between', fontSize: '0.9rem', marginBottom: '5px' }}>
                  <span>Em Manutenção</span>
                  <span style={{ fontWeight: 'bold' }}>{Math.round((emManutencao / totalTratores) * 100)}%</span>
                </div>
                <div style={{ width: '100%', height: '8px', background: 'rgba(255,255,255,0.05)', borderRadius: '4px', overflow: 'hidden' }}>
                  <div style={{ width: `${(emManutencao / totalTratores) * 100}%`, height: '100%', background: '#EF4444', borderRadius: '4px' }}></div>
                </div>
              </div>
            </div>
          )}
        </section>

        <section className="glass-card" style={{ padding: '2rem' }}>
          <h3 style={{ fontSize: '0.85rem', color: 'var(--text-secondary)', textTransform: 'uppercase', letterSpacing: '0.05em', marginBottom: '1.5rem' }}>Resumo Operacional</h3>
          <div style={{ display: 'flex', flexDirection: 'column', gap: '20px' }}>
             <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', padding: '15px', background: 'rgba(255,255,255,0.02)', border: '1px solid var(--glass-border)', borderRadius: '8px' }}>
                <span style={{ fontSize: '1rem' }}>Contratos Ativos</span>
                <span style={{ fontSize: '1.5rem', fontWeight: 'bold', color: '#3B82F6' }}>{estatisticasExtras.contratosAtivos}</span>
             </div>
             <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', padding: '15px', background: 'rgba(255,255,255,0.02)', border: '1px solid var(--glass-border)', borderRadius: '8px' }}>
                <span style={{ fontSize: '1rem' }}>Total de Clientes</span>
                <span style={{ fontSize: '1.5rem', fontWeight: 'bold', color: '#10B981' }}>{estatisticasExtras.clientes}</span>
             </div>
          </div>
        </section>
      </div>

      <section className="glass-card" style={{ padding: '2rem' }}>
        <h3 style={{ fontSize: '0.85rem', color: 'var(--text-secondary)', textTransform: 'uppercase', letterSpacing: '0.05em', marginBottom: '1.5rem' }}>Últimos Alertas</h3>
        
        {(!alerts || alerts.length === 0) ? (
          <div className="empty-state" style={{ minHeight: '150px' }}>
            <div style={{ width: '48px', height: '48px', background: '#10B981', borderRadius: '8px', display: 'flex', alignItems: 'center', justifyContent: 'center', fontSize: '1.5rem', margin: '0 auto 15px' }}>✅</div>
            Nenhum alerta recente
          </div>
        ) : (
          <div style={{ display: 'flex', flexDirection: 'column', gap: '10px' }}>
            {alerts.slice(0, 3).map((a: any, idx: number) => (
              <div key={idx} style={{ display: 'flex', gap: '15px', alignItems: 'center', padding: '15px', borderRadius: '8px', background: 'rgba(0,0,0,0.2)', borderLeft: `4px solid ${a.temperatura && a.temperatura > 110 ? '#EF4444' : '#F59E0B'}` }}>
                <div style={{ fontSize: '1.2rem' }}>{a.temperatura && a.temperatura > 110 ? '🚨' : '⚠️'}</div>
                <div style={{ flex: 1 }}>
                  <div style={{ fontWeight: 'bold', marginBottom: '4px' }}>Trator #{a.tratorId}</div>
                  <div style={{ color: 'var(--text-secondary)', fontSize: '0.9rem' }}>{a.mensagem} {a.temperatura ? `(Temp: ${a.temperatura.toFixed(1)}°C)` : ''}</div>
                </div>
                <div style={{ fontSize: '0.8rem', color: 'var(--text-secondary)' }}>{new Date(a.timestamp).toLocaleTimeString('pt-BR')}</div>
              </div>
            ))}
          </div>
        )}
      </section>
    </div>
  );
};
