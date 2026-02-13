import { Link, useNavigate } from 'react-router-dom'

export default function Login() {
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
        <div style={{ width: '100%', maxWidth: '400px', padding: '2rem', textAlign: 'center' }}>
          <h1 style={{ marginBottom: '2rem', fontSize: '2rem' }}>Justina</h1>
          
          <form onSubmit={handleSubmit} style={{ display: 'flex', flexDirection: 'column', gap: '1.5rem', textAlign: 'left' }}>
            <div>
              <label htmlFor="username">Usuario</label>
              <input type="text" id="username" placeholder="Ingresa tu usuario" />
            </div>
            
            <div>
              <label htmlFor="password">Contraseña</label>
              <input type="password" id="password" placeholder="Ingresa tu contraseña" />
            </div>

            <button type="submit" style={{ marginTop: '1rem' }}>Iniciar Sesión</button>
            
            <div style={{ textAlign: 'center', display: 'flex', flexDirection: 'column', gap: '0.5rem', fontSize: '0.9rem' }}>
              <a href="#" style={{ color: 'var(--text-muted)' }}>¿Olvidaste tu contraseña?</a>
              <Link to="/register" style={{ color: 'var(--text-muted)' }}>Crear una cuenta</Link>
            </div>
          </form>
        </div>
      </div>
    </div>
  )
}
