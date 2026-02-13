import { Link, useLocation } from 'react-router-dom'
import { LayoutDashboard, Activity, User, Settings } from 'lucide-react'

export default function Layout({ children }: { children?: React.ReactNode }) {
  const location = useLocation()
  
  // Check if we are in a simulation view (dark mode full screen) to adjust layout if needed
  // For now, we keep the header consistent
  
  return (
    <div style={{ minHeight: '100vh', display: 'flex', flexDirection: 'column' }}>
      <header style={{
        padding: '0.75rem 1.5rem',
        background: 'var(--bg-sidebar)', // Dark slate
        borderBottom: '1px solid rgba(255,255,255,0.1)',
        display: 'flex',
        alignItems: 'center',
        gap: '1.5rem',
        color: 'white'
      }}>
        <div style={{ display: 'flex', alignItems: 'center', gap: '0.75rem' }}>
          <div style={{ width: 24, height: 24, background: 'rgba(255,255,255,0.2)', borderRadius: 4 }}></div>
          <Link to="/" style={{ color: 'white', fontWeight: 700, fontSize: '1.25rem', textDecoration: 'none' }}>
            Justina
          </Link>
        </div>

        <nav style={{ marginLeft: 'auto', display: 'flex', gap: '0.5rem' }}>
          <Link 
            to="/" 
            style={{ 
              color: 'white', 
              textDecoration: 'none', 
              padding: '0.5rem 1rem', 
              borderRadius: '20px',
              background: location.pathname === '/' ? 'rgba(255,255,255,0.1)' : 'transparent',
              fontSize: '0.9rem',
              fontWeight: 500
            }}
          >
            Tablero
          </Link>
          <Link 
            to="/results" 
            style={{ 
              color: 'white', 
              textDecoration: 'none', 
              padding: '0.5rem 1rem', 
              borderRadius: '20px',
              background: location.pathname === '/results' ? 'rgba(255,255,255,0.1)' : 'transparent',
              fontSize: '0.9rem',
              fontWeight: 500
            }}
          >
            Resultados
          </Link>
           {/* Placeholder for settings if needed */}
           {/* <Link to="/settings" ... >Settings</Link> */}
        </nav>
        
        <div style={{ width: 32, height: 32, background: 'var(--accent)', borderRadius: '50%', display: 'flex', alignItems: 'center', justifyContent: 'center', marginLeft: '1rem' }}>
          <User size={18} color="white" />
        </div>
      </header>
      <main style={{ flex: 1, padding: '2rem', maxWidth: '1400px', margin: '0 auto', width: '100%' }}>
        {children}
      </main>
    </div>
  )
}
