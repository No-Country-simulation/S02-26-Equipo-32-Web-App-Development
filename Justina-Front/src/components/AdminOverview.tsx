import { useNavigate } from 'react-router-dom';

export default function AdminOverview() {
  const navigate = useNavigate();

  return (
    <div style={{ padding: '0 1rem' }}>
      
      {/* Título y Cabecera igual que en la vista de usuario */}
      <h2 style={{ fontSize: '1.5rem', fontWeight: 'bold', marginBottom: '1.5rem', color: '#1f2937' }}>
        Panel de Administración
      </h2>

      {/* Banner de Bienvenida (Similar al Modo Demo) */}
      <div style={{ 
        background: '#e0f2fe', 
        border: '1px solid #bae6fd', 
        borderRadius: '8px', 
        padding: '1.5rem', 
        marginBottom: '2rem',
        color: '#0c4a6e'
      }}>
        <h3 style={{ margin: 0, fontWeight: 'bold' }}>👋 Bienvenido, Administrador</h3>
        <p style={{ margin: '0.5rem 0 0 0', opacity: 0.9 }}>
          Desde aquí puedes gestionar el acceso de los cirujanos, configurar nuevas pruebas y descargar los reportes de rendimiento globales.
        </p>
      </div>

      {/* GRID DE TARJETAS (Copiando el estilo de tus juegos) */}
      <div style={{ 
        display: 'grid', 
        gridTemplateColumns: 'repeat(auto-fit, minmax(300px, 1fr))', 
        gap: '1.5rem' 
      }}>

        {/* TARJETA 1: GESTIONAR USUARIOS */}
        <div style={{ 
          background: 'linear-gradient(to bottom right, #ffffff, #f3f4f6)', 
          padding: '1.5rem', 
          borderRadius: '12px', 
          boxShadow: '0 4px 6px -1px rgba(0, 0, 0, 0.1)',
          border: '1px solid #e5e7eb',
          display: 'flex',
          flexDirection: 'column',
          justifyContent: 'space-between',
          minHeight: '200px'
        }}>
          <div>
            <div style={{ width: 40, height: 40, background: '#dbeafe', borderRadius: '50%', display: 'flex', alignItems: 'center', justifyContent: 'center', marginBottom: '1rem', color: '#2563eb' }}>
              👥 {/* Icono */}
            </div>
            <h3 style={{ fontWeight: 'bold', fontSize: '1.1rem', marginBottom: '0.5rem' }}>Gestionar Usuarios</h3>
            <p style={{ color: '#6b7280', fontSize: '0.9rem' }}>
              Alta, baja y modificación de roles de médicos y estudiantes.
            </p>
          </div>
          <button 
            onClick={() => navigate('/admin/users')} // CAMBIAR LUEGO por navigate('/admin/users')
            style={{ marginTop: '1rem', background: 'white', border: '1px solid #e5e7eb', padding: '0.5rem 1rem', borderRadius: '6px', fontWeight: '600', cursor: 'pointer', color: '#374151' }}>
            Ver Usuarios &rarr;
          </button>
        </div>

        {/* TARJETA 2: GESTIONAR PRUEBAS */}
        <div style={{ 
          background: 'linear-gradient(to bottom right, #ffffff, #f3f4f6)', 
          padding: '1.5rem', 
          borderRadius: '12px', 
          boxShadow: '0 4px 6px -1px rgba(0, 0, 0, 0.1)',
          border: '1px solid #e5e7eb',
          display: 'flex',
          flexDirection: 'column',
          justifyContent: 'space-between',
          minHeight: '200px'
        }}>
          <div>
             <div style={{ width: 40, height: 40, background: '#fce7f3', borderRadius: '50%', display: 'flex', alignItems: 'center', justifyContent: 'center', marginBottom: '1rem', color: '#db2777' }}>
              ⚙️ {/* Icono */}
            </div>
            <h3 style={{ fontWeight: 'bold', fontSize: '1.1rem', marginBottom: '0.5rem' }}>Configurar Pruebas</h3>
            <p style={{ color: '#6b7280', fontSize: '0.9rem' }}>
              Ajustar parámetros de dificultad y métricas de evaluación de Justina.
            </p>
          </div>
          <button 
             onClick={() => alert("Aquí irías a la config de pruebas")}
             style={{ marginTop: '1rem', background: 'white', border: '1px solid #e5e7eb', padding: '0.5rem 1rem', borderRadius: '6px', fontWeight: '600', cursor: 'pointer', color: '#374151' }}>
            Configurar &rarr;
          </button>
        </div>

        {/* TARJETA 3: REPORTES */}
        <div style={{ 
          background: 'linear-gradient(to bottom right, #ffffff, #f3f4f6)', 
          padding: '1.5rem', 
          borderRadius: '12px', 
          boxShadow: '0 4px 6px -1px rgba(0, 0, 0, 0.1)',
          border: '1px solid #e5e7eb',
          display: 'flex',
          flexDirection: 'column',
          justifyContent: 'space-between',
          minHeight: '200px'
        }}>
          <div>
            <div style={{ width: 40, height: 40, background: '#dcfce7', borderRadius: '50%', display: 'flex', alignItems: 'center', justifyContent: 'center', marginBottom: '1rem', color: '#16a34a' }}>
              📊 {/* Icono */}
            </div>
            <h3 style={{ fontWeight: 'bold', fontSize: '1.1rem', marginBottom: '0.5rem' }}>Reportes Globales</h3>
            <p style={{ color: '#6b7280', fontSize: '0.9rem' }}>
              Analíticas de desempeño, curvas de aprendizaje y exportación de datos.
            </p>
          </div>
          <button 
             onClick={() => alert("Aquí irías a los reportes")}
             style={{ marginTop: '1rem', background: 'white', border: '1px solid #e5e7eb', padding: '0.5rem 1rem', borderRadius: '6px', fontWeight: '600', cursor: 'pointer', color: '#374151' }}>
            Ver Estadísticas &rarr;
          </button>
        </div>

      </div>
    </div>
  );
}