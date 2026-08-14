import React, { useEffect, useState } from 'react';

export const Clientes: React.FC = () => {
  const [clientes, setClientes] = useState<any[]>([]);
  const [modalAberto, setModalAberto] = useState(false);
  const [formData, setFormData] = useState({ nome: '', documento: '' });
  const [formError, setFormError] = useState('');
  const [formSuccess, setFormSuccess] = useState('');
  const [enviando, setEnviando] = useState(false);

  const carregarClientes = () => {
    fetch('http://localhost:5000/api/clientes')
      .then(res => res.json())
      .then(data => setClientes(data))
      .catch(err => console.error("Erro ao carregar clientes", err));
  };

  useEffect(() => {
    carregarClientes();
  }, []);

  const handleCadastro = async (e: React.FormEvent) => {
    e.preventDefault();
    setFormError('');
    setFormSuccess('');

    if (!formData.nome || !formData.documento) {
      setFormError('Preencha todos os campos obrigatórios.');
      return;
    }

    setEnviando(true);
    try {
      const res = await fetch('http://localhost:5000/api/clientes', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(formData)
      });
      if (!res.ok) throw new Error('Erro ao cadastrar cliente');

      setFormSuccess('Cliente cadastrado com sucesso!');
      setFormData({ nome: '', documento: '' });
      carregarClientes();
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
          <h1 style={{ fontSize: '1.8rem', margin: 0 }}>Clientes</h1>
          <p className="page-subtitle" style={{ margin: 0 }}>Gerencie os locatários dos seus tratores.</p>
        </div>
        <button className="btn btn-primary" onClick={() => setModalAberto(true)}>+ Novo Cliente</button>
      </div>
      
      <div className="cards-grid" style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fill, minmax(300px, 1fr))', gap: '20px' }}>
        {clientes.length === 0 ? (
          <p className="empty-state">Nenhum cliente cadastrado.</p>
        ) : (
          clientes.map(c => (
            <div key={c.id} className="trator-card fade-in" style={{ padding: '20px' }}>
              <div style={{ display: 'flex', alignItems: 'center', gap: '15px', marginBottom: '15px' }}>
                <div style={{ width: '50px', height: '50px', borderRadius: '50%', background: 'rgba(0, 255, 204, 0.1)', color: 'var(--accent-color)', display: 'flex', alignItems: 'center', justifyContent: 'center', fontSize: '1.5rem' }}>
                  👥
                </div>
                <div>
                  <h3 style={{ margin: 0, fontSize: '1.1rem' }}>{c.nome}</h3>
                  <p style={{ margin: 0, color: 'var(--text-secondary)', fontSize: '0.85rem' }}>{new Date(c.dataCadastro).toLocaleDateString('pt-BR')}</p>
                </div>
              </div>
              <div style={{ background: 'var(--glass-bg)', padding: '10px 15px', borderRadius: '8px', border: '1px solid var(--glass-border)' }}>
                <span style={{ color: 'var(--text-secondary)', fontSize: '0.8rem', textTransform: 'uppercase' }}>Documento</span>
                <p style={{ margin: 0, fontFamily: 'monospace', fontWeight: 'bold' }}>{c.documento}</p>
              </div>
            </div>
          ))
        )}
      </div>

      {modalAberto && (
        <div className="modal-overlay active">
          <div className="modal">
            <div className="modal-header">
              <h2 className="modal-title">👥 Cadastrar Novo Cliente</h2>
              <button className="modal-close" onClick={() => setModalAberto(false)}>×</button>
            </div>
            <div className="modal-body">
              {formError && <div className="toast toast-error">{formError}</div>}
              {formSuccess && <div className="toast toast-success">{formSuccess}</div>}
              <div className="form-group">
                <label className="form-label">Nome Completo / Empresa</label>
                <input className="form-input" type="text" value={formData.nome} onChange={e => setFormData({...formData, nome: e.target.value})} placeholder="Ex: AgroTech Solutions" />
              </div>
              <div className="form-group">
                <label className="form-label">Documento (CPF/CNPJ)</label>
                <input className="form-input" type="text" value={formData.documento} onChange={e => setFormData({...formData, documento: e.target.value})} placeholder="Ex: 00.000.000/0001-00" />
              </div>
            </div>
            <div className="modal-footer">
              <button className="btn btn-secondary" onClick={() => setModalAberto(false)}>Cancelar</button>
              <button className="btn btn-primary" onClick={handleCadastro} disabled={enviando}>
                {enviando ? 'Salvando...' : 'Salvar Cliente'}
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};
