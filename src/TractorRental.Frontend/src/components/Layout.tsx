import React from 'react';
import { Outlet } from 'react-router-dom';
import { Sidebar } from './Sidebar';

export const Layout: React.FC<{ context?: any }> = ({ context }) => {
  return (
    <div className="app-container">
      <Sidebar />
      <main className="main-content">
        <Outlet context={context} />
      </main>
    </div>
  );
};
