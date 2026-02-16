import { useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { isAdmin, getCurrentUser } from '../store';

export default function AdminDashboard() {
  const navigate = useNavigate();
  const user = getCurrentUser();

  // PROTECCIÓN DE RUTA: Si entra alguien que no es admin, lo echamos fuera
  useEffect(() => {
    if (!isAdmin()) {
      navigate('/'); // Lo mandamos al home normal
    }
  }, [navigate]);

  return (
    <div style={{ padding: '2rem', background: '#f0f2f5', minHeight: '100vh' }}>
      <header style={{ display: 'flex', justifyContent: 'space-between', marginBottom: '2rem' }}>
        <h1>🛡️ Panel de Control - Administrador</h1>
        <button onClick={() => navigate('/')}>Volver al Inicio</button>
      </header>

      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fit, minmax(300px, 1fr))', gap: '1.5rem' }}>
        
        {/* Tarjeta 1: Usuarios */}
        <div style={{ background: 'white', padding: '1.5rem', borderRadius: '8px', boxShadow: '0 2px 4px rgba(0,0,0,0.1)' }}>
          <h3>👥 Gestión de Usuarios</h3>
          <p>Hay 3 doctores registrados actualmente.</p>
          <button style={{ marginTop: '10px', background: '#007bff', color: 'white', border: 'none', padding: '8px 16px', borderRadius: '4px' }}>
            Ver lista completa
          </button>
        </div>

        {/* Tarjeta 2: Reportes */}
        <div style={{ background: 'white', padding: '1.5rem', borderRadius: '8px', boxShadow: '0 2px 4px rgba(0,0,0,0.1)' }}>
          <h3>📊 Reportes Globales</h3>
          <p>Promedio de precisión en suturas: 85%</p>
          <button style={{ marginTop: '10px', background: '#28a745', color: 'white', border: 'none', padding: '8px 16px', borderRadius: '4px' }}>
            Descargar Excel
          </button>
        </div>

      </div>
      
      <div style={{ marginTop: '2rem' }}>
        <p>Logueado como: <strong>{user?.email}</strong> (Roles: {Array.isArray(user?.role) ? user?.role.join(', ') : user?.role})</p>
      </div>
    </div>
  );
}