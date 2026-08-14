import React from 'react';
import { NavLink } from 'react-router-dom';

export const Sidebar: React.FC = () => {
  const navItems = [
    { to: '/', icon: '📊', text: 'Dashboard' },
    { to: '/frota', icon: '🚜', text: 'Frota' },
    { to: '/clientes', icon: '👥', text: 'Clientes' },
    { to: '/contratos', icon: '📋', text: 'Contratos' },
    { to: '/alertas', icon: '🔔', text: 'Alertas' },
  ];

  return (
    <aside className="sidebar">
      <div className="sidebar-header">
        <div className="sidebar-logo">
          <div className="sidebar-logo-icon">🚜</div>
          <div className="sidebar-logo-text">
            TractorRental
            <span>Sistema de Gestão</span>
          </div>
        </div>
      </div>
      <nav className="sidebar-nav">
        {navItems.map(item => (
          <NavLink
            key={item.to}
            to={item.to}
            className={({ isActive }) => `nav-link ${isActive ? 'active' : ''}`}
          >
            <span className="nav-icon">{item.icon}</span>
            <span className="nav-text">{item.text}</span>
          </NavLink>
        ))}
      </nav>
      <div className="sidebar-footer">© {new Date().getFullYear()} TractorRental v2.0</div>
    </aside>
  );
};
