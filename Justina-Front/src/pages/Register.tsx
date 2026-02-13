import { Link, useNavigate } from 'react-router-dom'

export default function Register() {
  const navigate = useNavigate()

  const handleSubmit = (e: React.FormEvent) => {
    e.preventDefault()
    // For now, just navigate to dashboard
    navigate('/')
  }

  return (
    <div style={{ 
      minHeight: '100vh', 
      display: 'flex', 
      flexDirection: 'column',
      background: 'var(--bg-card)'
    }}>
      <header style={{ padding: '1.5rem', borderBottom: '1px solid var(--border)' }}>
        <div style={{ display: 'flex', alignItems: 'center', gap: '0.5rem', fontWeight: 700, fontSize: '1.25rem', color: 'var(--text)' }}>
          <div style={{ width: 24, height: 24, background: 'var(--bg-dark)', borderRadius: 4 }}></div>
          Justina
          <div style={{ marginLeft: 'auto' }}>
            <Link to="/login" style={{ marginRight: '1rem', color: 'var(--text)', fontWeight: 600 }}>Iniciar Sesión</Link>
            <Link to="/register" style={{ padding: '0.5rem 1rem', background: 'var(--bg-dark)', color: 'white', borderRadius: '6px', textDecoration: 'none' }}>Registrarse</Link>
          </div>
        </div>
      </header>

      <div style={{ flex: 1, display: 'flex', alignItems: 'center', justifyContent: 'center' }}>
        <div style={{ width: '100%', maxWidth: '400px', padding: '2rem' }}>
          <h1 style={{ marginBottom: '2rem', fontSize: '2rem', textAlign: 'center' }}>Crea tu cuenta</h1>
          
          <form onSubmit={handleSubmit} style={{ display: 'flex', flexDirection: 'column', gap: '1.5rem' }}>
            <div>
              <label htmlFor="fullname">Nombre completo</label>
              <input type="text" id="fullname" placeholder="Ingresa tu nombre completo" />
            </div>

            <div>
              <label htmlFor="email">Correo electrónico</label>
              <input type="email" id="email" placeholder="Ingresa tu correo" />
            </div>
            
            <div>
              <label htmlFor="password">Contraseña</label>
              <input type="password" id="password" placeholder="Ingresa tu contraseña" />
            </div>

            <div>
              <label htmlFor="confirm-password">Confirmar contraseña</label>
              <input type="password" id="confirm-password" placeholder="Confirma tu contraseña" />
            </div>

            <button type="submit" style={{ marginTop: '1rem' }}>Registrarse</button>
            
            <div style={{ textAlign: 'center', fontSize: '0.9rem' }}>
              <Link to="/login" style={{ color: 'var(--text-muted)' }}>¿Ya tienes una cuenta?</Link>
            </div>
          </form>
        </div>
      </div>
    </div>
  )
}
