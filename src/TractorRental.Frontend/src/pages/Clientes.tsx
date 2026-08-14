import React, { useEffect, useState } from 'react';

export const Clientes: React.FC = () => {
  const [clientes, setClientes] = useState<any[]>([]);
  const [modalAberto, setModalAberto] = useState(false);
  const [etapa, setEtapa] = useState(1);
  
  const [formData, setFormData] = useState({
    tipoPessoa: 'Fisica',
    documento: '',
    razaoSocialOuNome: '',
    inscricaoEstadual: '',
    emailFaturamento: '',
    nomeResponsavelOperacional: '',
    telefoneOperacional: '',
    enderecoOperacao: '',
    cidadeOperacao: '',
    estadoOperacao: ''
  });

  const [formError, setFormError] = useState('');
  const [formSuccess, setFormSuccess] = useState('');
  const [enviando, setEnviando] = useState(false);

  const carregarClientes = () => {
    fetch('http://localhost:5257/api/clientes')
      .then(res => res.json())
      .then(data => setClientes(data))
      .catch(err => console.error("Erro ao carregar clientes", err));
  };

  useEffect(() => {
    carregarClientes();
  }, []);

  const resetForm = () => {
    setFormData({
      tipoPessoa: 'Fisica',
      documento: '',
      razaoSocialOuNome: '',
      inscricaoEstadual: '',
      emailFaturamento: '',
      nomeResponsavelOperacional: '',
      telefoneOperacional: '',
      enderecoOperacao: '',
      cidadeOperacao: '',
      estadoOperacao: ''
    });
    setEtapa(1);
    setFormError('');
    setFormSuccess('');
  };

  const handleAbrirModal = () => {
    resetForm();
    setModalAberto(true);
  };

  const avancarEtapa = () => {
    setFormError('');
    if (etapa === 1) {
      if (!formData.razaoSocialOuNome || !formData.documento) {
        setFormError('Nome e Documento são obrigatórios.');
        return;
      }
    } else if (etapa === 2) {
      if (!formData.emailFaturamento || !formData.nomeResponsavelOperacional || !formData.telefoneOperacional) {
        setFormError('Todos os campos de contato são obrigatórios.');
        return;
      }
    }
    setEtapa(prev => prev + 1);
  };

  const voltarEtapa = () => {
    setEtapa(prev => prev - 1);
    setFormError('');
  };

  const handleCadastro = async (e: React.FormEvent) => {
    e.preventDefault();
    setFormError('');
    setFormSuccess('');

    if (!formData.enderecoOperacao || !formData.cidadeOperacao || !formData.estadoOperacao) {
      setFormError('Todos os campos de endereço são obrigatórios.');
      return;
    }

    setEnviando(true);
    try {
      const res = await fetch('http://localhost:5257/api/clientes', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify(formData)
      });
      
      if (!res.ok) {
        const errorText = await res.text();
        let errorMessage = 'Erro ao cadastrar cliente. Verifique os dados e tente novamente.';
        try {
          const errorData = JSON.parse(errorText);
          errorMessage = errorData?.message || errorData?.title || errorData?.detail || errorMessage;
        } catch {
          console.error("Erro bruto da API:", errorText);
        }
        throw new Error(errorMessage);
      }

      setFormSuccess('Cliente cadastrado com sucesso!');
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
        <button className="btn btn-primary" onClick={handleAbrirModal}>+ Novo Cliente</button>
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
                  <h3 style={{ margin: 0, fontSize: '1.1rem' }}>{c.razaoSocialOuNome}</h3>
                  <p style={{ margin: 0, color: 'var(--text-secondary)', fontSize: '0.85rem' }}>
                    Contato: {c.contatoOperacional?.nome || 'N/A'}
                  </p>
                </div>
              </div>
              <div style={{ background: 'var(--glass-bg)', padding: '10px 15px', borderRadius: '8px', border: '1px solid var(--glass-border)', display: 'flex', flexDirection: 'column', gap: '8px' }}>
                <div>
                  <span style={{ color: 'var(--text-secondary)', fontSize: '0.8rem', textTransform: 'uppercase' }}>Documento</span>
                  <p style={{ margin: 0, fontFamily: 'monospace', fontWeight: 'bold' }}>{c.documento?.numero || 'N/A'}</p>
                </div>
                <div>
                  <span style={{ color: 'var(--text-secondary)', fontSize: '0.8rem', textTransform: 'uppercase' }}>Operação</span>
                  <p style={{ margin: 0, fontSize: '0.9rem' }}>{c.enderecoOperacao?.cidade || 'N/A'} - {c.enderecoOperacao?.estado || 'N/A'}</p>
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
              <h2 className="modal-title">👥 Cadastrar Novo Cliente</h2>
              <button className="modal-close" onClick={() => setModalAberto(false)}>×</button>
            </div>
            
            <div className="modal-body">
              {/* Progress Bar */}
              <div style={{ display: 'flex', gap: '10px', marginBottom: '20px' }}>
                <div style={{ flex: 1, height: '6px', borderRadius: '3px', background: etapa >= 1 ? 'var(--accent-color)' : 'var(--glass-border)' }}></div>
                <div style={{ flex: 1, height: '6px', borderRadius: '3px', background: etapa >= 2 ? 'var(--accent-color)' : 'var(--glass-border)' }}></div>
                <div style={{ flex: 1, height: '6px', borderRadius: '3px', background: etapa >= 3 ? 'var(--accent-color)' : 'var(--glass-border)' }}></div>
              </div>
              
              <p style={{ color: 'var(--text-secondary)', fontSize: '0.9rem', marginBottom: '20px' }}>
                {etapa === 1 && 'Passo 1: Dados Fiscais'}
                {etapa === 2 && 'Passo 2: Contato e Faturamento'}
                {etapa === 3 && 'Passo 3: Local da Operação'}
              </p>

              {formError && <div className="toast toast-error">{formError}</div>}
              {formSuccess && <div className="toast toast-success">{formSuccess}</div>}
              
              {/* Etapa 1 */}
              {etapa === 1 && (
                <div className="fade-in">
                  <div className="form-group">
                    <label className="form-label">Tipo de Pessoa</label>
                    <div style={{ display: 'flex', gap: '15px' }}>
                      <label style={{ display: 'flex', alignItems: 'center', gap: '5px', cursor: 'pointer' }}>
                        <input type="radio" name="tipoPessoa" value="Fisica" checked={formData.tipoPessoa === 'Fisica'} onChange={e => setFormData({...formData, tipoPessoa: e.target.value})} /> Física
                      </label>
                      <label style={{ display: 'flex', alignItems: 'center', gap: '5px', cursor: 'pointer' }}>
                        <input type="radio" name="tipoPessoa" value="Juridica" checked={formData.tipoPessoa === 'Juridica'} onChange={e => setFormData({...formData, tipoPessoa: e.target.value})} /> Jurídica
                      </label>
                    </div>
                  </div>
                  <div className="form-group">
                    <label className="form-label">Nome Completo / Razão Social</label>
                    <input className="form-input" type="text" value={formData.razaoSocialOuNome} onChange={e => setFormData({...formData, razaoSocialOuNome: e.target.value})} placeholder={formData.tipoPessoa === 'Juridica' ? 'Ex: AgroTech Solutions LTDA' : 'Ex: João da Silva'} />
                  </div>
                  <div className="form-group">
                    <label className="form-label">Documento ({formData.tipoPessoa === 'Juridica' ? 'CNPJ' : 'CPF'})</label>
                    <input className="form-input" type="text" value={formData.documento} onChange={e => setFormData({...formData, documento: e.target.value})} placeholder={formData.tipoPessoa === 'Juridica' ? '00.000.000/0001-00' : '000.000.000-00'} />
                  </div>
                  {formData.tipoPessoa === 'Juridica' && (
                    <div className="form-group">
                      <label className="form-label">Inscrição Estadual / CAD PRO</label>
                      <input className="form-input" type="text" value={formData.inscricaoEstadual} onChange={e => setFormData({...formData, inscricaoEstadual: e.target.value})} placeholder="Opcional se isento" />
                    </div>
                  )}
                </div>
              )}

              {/* Etapa 2 */}
              {etapa === 2 && (
                <div className="fade-in">
                  <div className="form-group">
                    <label className="form-label">E-mail para Faturamento / NFe</label>
                    <input className="form-input" type="email" value={formData.emailFaturamento} onChange={e => setFormData({...formData, emailFaturamento: e.target.value})} placeholder="financeiro@empresa.com" />
                  </div>
                  <div className="form-group">
                    <label className="form-label">Nome Responsável Operacional</label>
                    <input className="form-input" type="text" value={formData.nomeResponsavelOperacional} onChange={e => setFormData({...formData, nomeResponsavelOperacional: e.target.value})} placeholder="Quem recebe a máquina" />
                  </div>
                  <div className="form-group">
                    <label className="form-label">Telefone Responsável</label>
                    <input className="form-input" type="text" value={formData.telefoneOperacional} onChange={e => setFormData({...formData, telefoneOperacional: e.target.value})} placeholder="(00) 90000-0000" />
                  </div>
                </div>
              )}

              {/* Etapa 3 */}
              {etapa === 3 && (
                <div className="fade-in">
                  <div className="form-group">
                    <label className="form-label">Endereço da Operação (Fazenda/Obra)</label>
                    <input className="form-input" type="text" value={formData.enderecoOperacao} onChange={e => setFormData({...formData, enderecoOperacao: e.target.value})} placeholder="Rodovia BR 123, Km 45" />
                  </div>
                  <div style={{ display: 'flex', gap: '15px' }}>
                    <div className="form-group" style={{ flex: 2 }}>
                      <label className="form-label">Cidade</label>
                      <input className="form-input" type="text" value={formData.cidadeOperacao} onChange={e => setFormData({...formData, cidadeOperacao: e.target.value})} placeholder="Cidade" />
                    </div>
                    <div className="form-group" style={{ flex: 1 }}>
                      <label className="form-label">Estado (UF)</label>
                      <input className="form-input" type="text" value={formData.estadoOperacao} onChange={e => setFormData({...formData, estadoOperacao: e.target.value})} placeholder="SP" maxLength={2} />
                    </div>
                  </div>
                </div>
              )}
            </div>
            
            <div className="modal-footer" style={{ justifyContent: 'space-between' }}>
              <div>
                {etapa > 1 && <button className="btn btn-secondary" onClick={voltarEtapa}>Voltar</button>}
              </div>
              <div style={{ display: 'flex', gap: '10px' }}>
                <button className="btn btn-secondary" onClick={() => setModalAberto(false)}>Cancelar</button>
                {etapa < 3 ? (
                  <button className="btn btn-primary" onClick={avancarEtapa}>Próximo</button>
                ) : (
                  <button className="btn btn-primary" onClick={handleCadastro} disabled={enviando}>
                    {enviando ? 'Salvando...' : 'Salvar Cliente'}
                  </button>
                )}
              </div>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};
