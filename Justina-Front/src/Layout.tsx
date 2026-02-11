import { Link, Outlet } from 'react-router-dom'

export default function Layout({ children }: { children?: React.ReactNode }) {
  return (
    <div style={{ minHeight: '100vh', display: 'flex', flexDirection: 'column' }}>
      <header style={{
        padding: '0.75rem 1.5rem',
        background: 'var(--bg-card)',
        boxShadow: '0 1px 3px rgba(0,0,0,0.08)',
        borderBottom: '1px solid var(--border)',
        display: 'flex',
        alignItems: 'center',
        gap: '1.5rem',
      }}>
        <Link to="/" style={{ color: 'var(--accent)', fontWeight: 700, fontSize: '1.25rem' }}>
          Justina
        </Link>
        <span style={{ color: 'var(--text-muted)', fontSize: '0.9rem' }}>Simulador de cirugía renal</span>
        <nav style={{ marginLeft: 'auto', display: 'flex', gap: '1rem' }}>
          <Link to="/">Inicio</Link>
          <Link to="/results">Resultados</Link>
        </nav>
      </header>
      <main style={{ flex: 1, padding: '1rem' }}>
        {children ?? <Outlet />}
      </main>
    </div>
  )
}
