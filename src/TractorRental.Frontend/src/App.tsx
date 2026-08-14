import { useEffect, useState } from 'react';
import * as signalR from '@microsoft/signalr';
import { BrowserRouter, Routes, Route } from 'react-router-dom';

import { Layout } from './components/Layout';
import { Dashboard } from './pages/Dashboard';
import { Frota } from './pages/Frota';
import { Clientes } from './pages/Clientes';
import { Contratos } from './pages/Contratos';
import { Alertas } from './pages/Alertas';

interface TratorDto {
  id: string;
  marca: string;
  modelo: string;
  status: string;
  temperaturaAtualMotor: number;
  pressaoAtualPneus: number;
  nivelCombustivel: number;
  nivelOleo: number;
  rotacaoMotor: number;
  velocidade: number;
}

interface TelemetryData {
  tractorId: string;
  speed: number;
  fuelLevel: number;
  engineTemp: number;
  timestamp: string;
}

interface Alert {
  tratorId: string;
  mensagem: string;
  temperatura?: number;
  timestamp: string; // generated client-side
}

interface CadastroTratorForm {
  marca: string;
  modelo: string;
  anoFabricacao: string;
  potenciaCv: string;
  horimetroInicial: string;
  numeroSerie: string;
}

const FORM_INICIAL: CadastroTratorForm = {
  marca: '',
  modelo: '',
  anoFabricacao: '',
  potenciaCv: '',
  horimetroInicial: '0',
  numeroSerie: '',
};

function App() {
  const [tratores, setTratores] = useState<TratorDto[]>([]);
  const [alerts, setAlerts] = useState<Alert[]>([]);
  const [connectionStatus, setConnectionStatus] = useState('Disconnected');
  const [modalAberto, setModalAberto] = useState(false);
  const [formData, setFormData] = useState<CadastroTratorForm>(FORM_INICIAL);
  const [formError, setFormError] = useState('');
  const [formSuccess, setFormSuccess] = useState('');
  const [enviando, setEnviando] = useState(false);

  useEffect(() => {
    const fetchTratores = () => {
      fetch('http://localhost:5257/api/tratores/dashboard')
        .then(res => res.json())
        .then(data => setTratores(data))
        .catch(err => console.error("Error fetching tractors:", err));
    };

    fetchTratores();
    const intervalId = setInterval(fetchTratores, 5000);

    const connection = new signalR.HubConnectionBuilder()
      .withUrl("http://localhost:5257/hubs/monitoramento")
      .withAutomaticReconnect()
      .build();

    setConnectionStatus('Connecting...');

    connection.start()
      .then(() => {
        setConnectionStatus('Connected');

        connection.on("ReceberAlerta", (alerta: any) => {
          const newAlert: Alert = {
            tratorId: alerta.tratorId,
            mensagem: alerta.mensagem,
            temperatura: alerta.temperatura,
            timestamp: new Date().toISOString()
          };
          setAlerts(prev => [newAlert, ...prev].slice(0, 10)); // Keep last 10
        });
      })
      .catch(err => {
        console.error("SignalR Connection Error: ", err);
        setConnectionStatus('Error Connecting');
      });

    return () => {
      clearInterval(intervalId);
      connection.stop();
    };
  }, []);

  const handleCadastro = async (e: React.FormEvent) => {
    e.preventDefault();
    setFormError('');
    setFormSuccess('');

    if (!formData.marca || !formData.modelo || !formData.anoFabricacao || !formData.potenciaCv || !formData.numeroSerie) {
      setFormError('Preencha todos os campos obrigatórios.');
      return;
    }

    setEnviando(true);
    try {
      const response = await fetch('http://localhost:5257/api/tratores', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({
          marca: formData.marca,
          modelo: formData.modelo,
          anoFabricacao: parseInt(formData.anoFabricacao),
          potenciaCv: parseInt(formData.potenciaCv),
          horimetroInicial: parseFloat(formData.horimetroInicial) || 0,
          numeroSerie: formData.numeroSerie,
        })
      });

      if (!response.ok) {
        const errorData = await response.json();
        throw new Error(errorData.mensagem || 'Erro ao cadastrar trator');
      }

      setFormSuccess('Trator cadastrado com sucesso!');
      setFormData(FORM_INICIAL);
      
      fetch('http://localhost:5257/api/tratores/dashboard')
        .then(res => res.json())
        .then(data => setTratores(data))
        .catch(err => console.error("Error fetching tractors:", err));

      setTimeout(() => {
        setModalAberto(false);
        setFormSuccess('');
      }, 1500);
    } catch (err: any) {
      setFormError(err.message);
    } finally {
      setEnviando(false);
    }
  };

  const contextValue = {
    tratores, setTratores,
    alerts,
    formData, setFormData,
    handleCadastro,
    modalAberto, setModalAberto,
    formError, formSuccess,
    enviando
  };

  return (
    <BrowserRouter>
      <Routes>
        <Route path="/" element={<Layout context={contextValue} />}>
          <Route index element={<Dashboard />} />
          <Route path="frota" element={<Frota />} />
          <Route path="clientes" element={<Clientes />} />
          <Route path="contratos" element={<Contratos />} />
          <Route path="alertas" element={<Alertas />} />
        </Route>
      </Routes>
    </BrowserRouter>
  );
}

export default App;
