import React, { useState } from 'react';
import { useOutletContext } from 'react-router-dom';
import { Gauge } from '../components/Gauge';
import { StatusBadge } from '../components/StatusBadge';

const MARCAS_TRATOR = [
  'JohnDeere', 'MasseyFerguson', 'Valtra', 'NewHolland', 'CaseIH', 
  'Agrale', 'Caterpillar', 'Kubota', 'Outra',
] as const;

export const Frota: React.FC = () => {
  const { tratores, formData, setFormData, handleCadastro, modalAberto, setModalAberto, formError, formSuccess, enviando } = useOutletContext<any>();

  const handleChange = (e: React.ChangeEvent<HTMLInputElement | HTMLSelectElement>) => {
    setFormData((prev: any) => ({ ...prev, [e.target.name]: e.target.value }));
  };

  return (
    <div className="fade-in">
      <div className="page-header-row" style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '2rem' }}>
        <div>
          <h1 style={{ fontSize: '1.8rem', margin: 0 }}>Frota em Tempo Real</h1>
          <p className="page-subtitle" style={{ margin: 0 }}>Monitoramento de telemetria dos tratores da frota.</p>
        </div>
        <button className="btn btn-primary" onClick={() => setModalAberto(true)}>+ Cadastrar Novo Trator</button>
      </div>
      
      <div className="cards-grid" style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fill, minmax(400px, 1fr))', gap: '20px' }}>
        {tratores.length === 0 ? (
          <p className="empty-state">Nenhum trator cadastrado ou carregando...</p>
        ) : (
          tratores.map((t: any) => (
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

      {modalAberto && (
        <div className="modal-overlay active">
          <div className="modal">
            <div className="modal-header">
              <h2 className="modal-title">🚜 Cadastrar Novo Trator</h2>
              <button className="modal-close" onClick={() => setModalAberto(false)}>×</button>
            </div>
            <div className="modal-body">
              {formError && <div className="toast toast-error" style={{ position: 'relative', top: 0, right: 0, marginBottom: '1rem' }}>{formError}</div>}
              {formSuccess && <div className="toast toast-success" style={{ position: 'relative', top: 0, right: 0, marginBottom: '1rem' }}>{formSuccess}</div>}
              
              <div className="form-group">
                <label className="form-label">Marca</label>
                <select className="form-input" name="marca" value={formData.marca} onChange={handleChange}>
                  <option value="">Selecione a marca...</option>
                  {MARCAS_TRATOR.map(m => (
                    <option key={m} value={m}>{m}</option>
                  ))}
                </select>
              </div>
              <div className="form-group">
                <label className="form-label">Modelo</label>
                <input className="form-input" type="text" name="modelo" value={formData.modelo} onChange={handleChange} placeholder="Ex: 8R 370" />
              </div>
              <div className="form-group">
                <label className="form-label">Ano de Fabricação</label>
                <input className="form-input" type="number" name="anoFabricacao" value={formData.anoFabricacao} onChange={handleChange} placeholder="Ex: 2022" />
              </div>
              <div className="form-group">
                <label className="form-label">Potência (CV)</label>
                <input className="form-input" type="number" name="potenciaCv" value={formData.potenciaCv} onChange={handleChange} placeholder="Ex: 370" />
              </div>
              <div className="form-group">
                <label className="form-label">Horímetro Inicial (h)</label>
                <input className="form-input" type="number" name="horimetroInicial" value={formData.horimetroInicial} onChange={handleChange} placeholder="Ex: 1250" />
              </div>
              <div className="form-group">
                <label className="form-label">Nº de Série (PIN)</label>
                <input className="form-input" type="text" name="numeroSerie" value={formData.numeroSerie} onChange={handleChange} placeholder="Ex: 1LV..." />
              </div>
            </div>
            <div className="modal-footer">
              <button className="btn btn-secondary" onClick={() => setModalAberto(false)}>Cancelar</button>
              <button className="btn btn-primary" onClick={handleCadastro} disabled={enviando}>
                {enviando ? 'Salvando...' : 'Salvar Trator'}
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};
