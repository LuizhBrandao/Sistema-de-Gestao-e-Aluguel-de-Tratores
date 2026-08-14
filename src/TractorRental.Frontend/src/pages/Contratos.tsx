import React, { useEffect, useState } from 'react';
import { StatusBadge } from '../components/StatusBadge';

export const Contratos: React.FC = () => {
  const [contratos, setContratos] = useState<any[]>([]);
  const [clientes, setClientes] = useState<any[]>([]);
  const [tratores, setTratores] = useState<any[]>([]);
  
  const [modalAberto, setModalAberto] = useState(false);
  const [formData, setFormData] = useState({ clienteId: '', tratorId: '', valorHora: '' });
  const [formError, setFormError] = useState('');
  const [formSuccess, setFormSuccess] = useState('');
  const [enviando, setEnviando] = useState(false);

  const carregarContratos = () => {
    fetch('http://localhost:5000/api/contratos')
      .then(res => res.json())
      .then(data => setContratos(data))
      .catch(err => console.error("Erro ao carregar contratos", err));
  };

  useEffect(() => {
    carregarContratos();
  }, []);

  const abrirModal = async () => {
    try {
      const resClientes = await fetch('http://localhost:5000/api/clientes');
      const dataClientes = await resClientes.json();
      setClientes(dataClientes);

      const resTratores = await fetch('http://localhost:5000/api/tratores/dashboard');
      const dataTratores = await resTratores.json();
      setTratores(dataTratores.filter((t: any) => t.status === 'Operacional'));

      setModalAberto(true);
    } catch (err) {
      console.error(err);
    }
  };

  const handleCadastro = async (e: React.FormEvent) => {
    e.preventDefault();
    setFormError('');
    setFormSuccess('');

    if (!formData.clienteId || !formData.tratorId || !formData.valorHora) {
      setFormError('Preencha todos os campos obrigatórios.');
      return;
    }

    setEnviando(true);
    try {
      const res = await fetch('http://localhost:5000/api/contratos', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          clienteId: formData.clienteId,
          tratorId: formData.tratorId,
          valorHora: parseFloat(formData.valorHora)
        })
      });
      if (!res.ok) throw new Error('Erro ao cadastrar contrato');

      setFormSuccess('Contrato gerado com sucesso!');
      setFormData({ clienteId: '', tratorId: '', valorHora: '' });
      carregarContratos();
      setTimeout(() => setModalAberto(false), 1500);
    } catch (err: any) {
      setFormError(err.message);
    } finally {
      setEnviando(false);
    }
  };

  return (
    <div className="fade-in">
      <div className="page-header-row" style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '2rem' }}>
        <div>
          <h1 style={{ fontSize: '1.8rem', margin: 0 }}>Contratos de Aluguel</h1>
          <p className="page-subtitle" style={{ margin: 0 }}>Acompanhe as locações ativas e o histórico.</p>
        </div>
        <button className="btn btn-primary" onClick={abrirModal}>+ Novo Contrato</button>
      </div>
      
      <div className="cards-grid" style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fill, minmax(350px, 1fr))', gap: '20px' }}>
        {contratos.length === 0 ? (
          <p className="empty-state">Nenhum contrato cadastrado.</p>
        ) : (
          contratos.map(c => (
            <div key={c.id} className="trator-card fade-in" style={{ padding: '20px' }}>
              <div style={{ display: 'flex', justifyContent: 'space-between', marginBottom: '15px', alignItems: 'center' }}>
                <span style={{ fontFamily: 'monospace', color: 'var(--text-secondary)' }}>ID: {c.id.substring(0,8)}</span>
                <StatusBadge status={c.status} />
              </div>
              
              <div style={{ background: 'rgba(255, 255, 255, 0.02)', padding: '15px', borderRadius: '8px', border: '1px solid var(--glass-border)', display: 'flex', flexDirection: 'column', gap: '10px' }}>
                <div>
                  <span style={{ fontSize: '0.75rem', textTransform: 'uppercase', color: 'var(--text-secondary)' }}>Cliente ID</span>
                  <p style={{ margin: 0, fontFamily: 'monospace' }}>{c.clienteId.substring(0, 8)}...</p>
                </div>
                <div>
                  <span style={{ fontSize: '0.75rem', textTransform: 'uppercase', color: 'var(--text-secondary)' }}>Trator ID</span>
                  <p style={{ margin: 0, fontFamily: 'monospace' }}>{c.tratorId.substring(0, 8)}...</p>
                </div>
              </div>

              <div style={{ display: 'flex', justifyContent: 'space-between', marginTop: '15px', padding: '0 5px' }}>
                <div>
                  <span style={{ fontSize: '0.75rem', color: 'var(--text-secondary)' }}>Data Início</span>
                  <p style={{ margin: 0, fontWeight: 'bold' }}>{new Date(c.dataInicio).toLocaleDateString('pt-BR')}</p>
                </div>
                <div style={{ textAlign: 'right' }}>
                  <span style={{ fontSize: '0.75rem', color: 'var(--text-secondary)' }}>Valor/Hora</span>
                  <p style={{ margin: 0, fontWeight: 'bold', color: 'var(--accent-color)' }}>R$ {c.valorHora.toFixed(2)}</p>
                </div>
              </div>
            </div>
          ))
        )}
      </div>

      {modalAberto && (
        <div className="modal-overlay active">
          <div className="modal">
            <div className="modal-header">
              <h2 className="modal-title">📋 Cadastrar Novo Contrato</h2>
              <button className="modal-close" onClick={() => setModalAberto(false)}>×</button>
            </div>
            <div className="modal-body">
              {formError && <div className="toast toast-error">{formError}</div>}
              {formSuccess && <div className="toast toast-success">{formSuccess}</div>}
              
              <div className="form-group">
                <label className="form-label">Cliente</label>
                <select className="form-input" value={formData.clienteId} onChange={e => setFormData({...formData, clienteId: e.target.value})}>
                  <option value="">Selecione o cliente...</option>
                  {clientes.map(cli => <option key={cli.id} value={cli.id}>{cli.nome}</option>)}
                </select>
              </div>

              <div className="form-group">
                <label className="form-label">Trator (Apenas Operacionais)</label>
                <select className="form-input" value={formData.tratorId} onChange={e => setFormData({...formData, tratorId: e.target.value})}>
                  <option value="">Selecione o trator...</option>
                  {tratores.map(t => <option key={t.id} value={t.id}>{t.marca} {t.modelo} ({t.id.substring(0,8)})</option>)}
                </select>
              </div>

              <div className="form-group">
                <label className="form-label">Valor por Hora (R$)</label>
                <input className="form-input" type="number" step="0.01" value={formData.valorHora} onChange={e => setFormData({...formData, valorHora: e.target.value})} placeholder="Ex: 150.00" />
              </div>
            </div>
            <div className="modal-footer">
              <button className="btn btn-secondary" onClick={() => setModalAberto(false)}>Cancelar</button>
              <button className="btn btn-primary" onClick={handleCadastro} disabled={enviando}>
                {enviando ? 'Processando...' : 'Gerar Contrato'}
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};
