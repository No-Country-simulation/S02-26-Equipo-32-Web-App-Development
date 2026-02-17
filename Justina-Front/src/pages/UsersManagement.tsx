import { useState } from 'react';
import { useNavigate } from 'react-router-dom';

// 1. Definimos cómo se ve un usuario (Esto luego vendrá de types.ts)
interface User {
  id: number;
  email: string;
  role: string[]; // Puede ser ['Admin', 'Cirujano'] o solo ['Cirujano']
  lastLogin: string;
}

// 2. Datos Falsos (MOCKS) para diseñar mientras esperas al Backend
const MOCK_USERS: User[] = [
  { id: 1, email: 'admin@justina.com', role: ['Admin', 'Cirujano'], lastLogin: '2024-02-16' },
  { id: 2, email: 'dr.perez@hospital.com', role: ['Cirujano'], lastLogin: '2024-02-15' },
  { id: 3, email: 'estudiante1@med.edu', role: ['Estudiante'], lastLogin: '2024-02-10' },
  { id: 4, email: 'dra.gomez@clinica.com', role: ['Cirujano'], lastLogin: '2024-02-14' },
];

export default function UsersManagement() {
  const navigate = useNavigate();
  const [users, setUsers] = useState<User[]>(MOCK_USERS);
  const [searchTerm, setSearchTerm] = useState('');

  // --- LÓGICA DE ACCIONES ---

  // Simula convertir a alguien en Admin
  const toggleAdminRole = (userId: number) => {
    setUsers(users.map(u => {
      if (u.id === userId) {
        const isAdmin = u.role.includes('Admin');
        const newRoles = isAdmin 
          ? u.role.filter(r => r !== 'Admin') // Quitar admin
          : [...u.role, 'Admin']; // Poner admin
        return { ...u, role: newRoles };
      }
      return u;
    }));
  };

  // Simula resetear contraseña
  const handleResetPassword = (email: string) => {
    // Aquí iría la llamada a la API real: await fetch('/api/auth/reset', ...)
    alert(`📧 Se ha enviado un correo de restablecimiento a: ${email}`);
  };

  // Filtrado por buscador
  const filteredUsers = users.filter(u => 
    u.email.toLowerCase().includes(searchTerm.toLowerCase())
  );

  return (
    <div style={{ padding: '2rem', background: '#f8fafc', width: '100%', height: '100%'}}>
      {/* CABECERA */}
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '2rem' }}>
        <div>
          <button onClick={() => navigate('/')} style={{ background: 'none', border: 'none', cursor: 'pointer', color: '#64748b', marginBottom: '0.5rem' }}>
            &larr; Volver al Panel
          </button>
          <h1 style={{ margin: 0, color: '#1e293b' }}>Gestión de Usuarios</h1>
          <p style={{ color: '#64748b' }}>Administra los accesos y roles de la plataforma Justina.</p>
        </div>
        <button style={{ background: '#0f172a', color: 'white', padding: '10px 20px', borderRadius: '8px', border: 'none', fontWeight: 'bold' }}>
          + Nuevo Usuario
        </button>
      </div>

      {/* BARRA DE BÚSQUEDA */}
      <div style={{ background: 'white', padding: '1rem', borderRadius: '8px', boxShadow: '0 1px 3px rgba(0,0,0,0.1)', marginBottom: '1.5rem' }}>
        <input 
          type="text" 
          placeholder="🔍 Buscar por correo electrónico..." 
          value={searchTerm}
          onChange={(e) => setSearchTerm(e.target.value)}
          style={{ width: '100%', padding: '10px', border: '1px solid #e2e8f0', borderRadius: '6px' }}
        />
      </div>

      {/* TABLA DE USUARIOS */}
      <div style={{ background: 'white', borderRadius: '8px', boxShadow: '0 1px 3px rgba(0,0,0,0.1)', overflow: 'hidden' }}>
        <table style={{ width: '100%', borderCollapse: 'collapse', textAlign: 'left' }}>
          <thead style={{ background: '#f1f5f9' }}>
            <tr>
              <th style={{ padding: '1rem', color: '#475569' }}>Usuario</th>
              <th style={{ padding: '1rem', color: '#475569' }}>Roles Actuales</th>
              <th style={{ padding: '1rem', color: '#475569' }}>Último Acceso</th>
              <th style={{ padding: '1rem', color: '#475569', textAlign: 'right' }}>Acciones</th>
            </tr>
          </thead>
          <tbody>
            {filteredUsers.map(user => (
              <tr key={user.id} style={{ borderBottom: '1px solid #e2e8f0' }}>
                <td style={{ padding: '1rem', fontWeight: '500' }}>{user.email}</td>
                <td style={{ padding: '1rem' }}>
                  {user.role.map(r => (
                    <span key={r} style={{ 
                      background: r === 'Admin' ? '#dbeafe' : '#f1f5f9', 
                      color: r === 'Admin' ? '#1e40af' : '#475569',
                      padding: '4px 8px', borderRadius: '12px', fontSize: '0.8rem', marginRight: '5px', fontWeight: 'bold'
                    }}>
                      {r}
                    </span>
                  ))}
                </td>
                <td style={{ padding: '1rem', color: '#64748b' }}>{user.lastLogin}</td>
                <td style={{ padding: '1rem', display: 'flex', gap: '10px', justifyContent: 'flex-end' }}>
                  
                  {/* Botón Reset Password */}
                  <button 
                    onClick={() => handleResetPassword(user.email)}
                    title="Resetear Contraseña"
                    style={{ background: 'white', border: '1px solid #cbd5e1', padding: '6px 12px', borderRadius: '6px', cursor: 'pointer' }}>
                    🔑 Reset
                  </button>

                  {/* Botón Hacer Admin/Quitar Admin */}
                  <button 
                    onClick={() => toggleAdminRole(user.id)}
                    style={{ 
                      background: user.role.includes('Admin') ? '#fee2e2' : '#dcfce7', 
                      color: user.role.includes('Admin') ? '#991b1b' : '#166534',
                      border: 'none', padding: '6px 12px', borderRadius: '6px', cursor: 'pointer', fontWeight: '600' 
                    }}>
                    {user.role.includes('Admin') ? 'Quitar Admin' : 'Hacer Admin'}
                  </button>

                </td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
}