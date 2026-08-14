import React from 'react';

interface StatusBadgeProps {
  status: string;
}

export const StatusBadge: React.FC<StatusBadgeProps> = ({ status }) => {
  const map: Record<string, { cls: string; dot: string; label: string }> = {
    'Operacional':  { cls: 'badge-success', dot: 'green',  label: 'Operacional' },
    'Alugado':      { cls: 'badge-info',    dot: 'amber',  label: 'Alugado' },
    'EmManutencao': { cls: 'badge-danger',  dot: 'red',    label: 'Manutenção' },
    'Inativo':      { cls: 'badge-muted',   dot: 'gray',   label: 'Inativo' },
    'Ativo':        { cls: 'badge-success', dot: 'green',  label: 'Ativo' },
    'Finalizado':   { cls: 'badge-muted',   dot: 'gray',   label: 'Finalizado' },
    'Cancelado':    { cls: 'badge-danger',  dot: 'red',    label: 'Cancelado' },
  };

  const c = map[status] || { cls: 'badge-muted', dot: 'gray', label: status };

  return (
    <span className={`badge ${c.cls}`}>
      <span className={`status-dot ${c.dot}`}></span>
      {c.label}
    </span>
  );
};
