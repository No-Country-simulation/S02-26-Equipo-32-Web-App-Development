import { Link } from 'react-router-dom'
import { GAMES } from '../games'
import { Home as HomeIcon, BarChart2, ChevronRight, PlayCircle, Lock, Trophy } from 'lucide-react'
import { useProgressStore, canPlayGame, RANK_ORDER } from '../progressStore'
import type { UserRank } from '../types'

export default function Home() {
  const { rank, setRank, careerModeEnabled, setCareerMode } = useProgressStore()

  return (
    <div style={{ display: 'flex', gap: '2rem' }}>
      {/* Left Dashboard Sidebar */}
      <div style={{ 
        width: '240px', 
        flexShrink: 0,
        display: 'flex', 
        flexDirection: 'column', 
        gap: '0.5rem' 
      }}>
        <div style={{ 
          background: 'var(--bg-sidebar)', 
          borderRadius: '12px', 
          padding: '1.5rem', 
          color: 'white',
          height: '100%',
          display: 'flex',
          flexDirection: 'column'
        }}>
          <nav style={{ display: 'flex', flexDirection: 'column', gap: '0.5rem', marginBottom: '2rem' }}>
            <Link to="/" style={{ 
              display: 'flex', 
              alignItems: 'center', 
              gap: '0.75rem', 
              padding: '0.75rem', 
              background: 'rgba(255,255,255,0.1)', 
              borderRadius: '8px',
              color: 'white',
              textDecoration: 'none',
              fontWeight: 600
            }}>
              <HomeIcon size={20} />
              Inicio
            </Link>
            <Link to="/results" style={{ 
              display: 'flex', 
              alignItems: 'center', 
              gap: '0.75rem', 
              padding: '0.75rem', 
              color: 'rgba(255,255,255,0.7)', 
              textDecoration: 'none',
              fontWeight: 500
            }}>
              <BarChart2 size={20} />
              Resultados
            </Link>
          </nav>

          {/* Admin / Career Mode Controls */}
          <div style={{ 
            marginTop: 'auto', 
            paddingTop: '1rem', 
            borderTop: '1px solid rgba(255,255,255,0.1)' 
          }}>
            <h4 style={{ fontSize: '0.8rem', textTransform: 'uppercase', color: 'rgba(255,255,255,0.5)', marginBottom: '0.5rem' }}>Administración</h4>
            
            <label style={{ display: 'flex', alignItems: 'center', gap: '0.5rem', fontSize: '0.9rem', marginBottom: '1rem', cursor: 'pointer' }}>
              <input 
                type="checkbox" 
                checked={careerModeEnabled} 
                onChange={(e) => setCareerMode(e.target.checked)}
                style={{ accentColor: 'var(--accent)' }}
              />
              Modo Carrera
            </label>

            {careerModeEnabled && (
              <div style={{ display: 'flex', flexDirection: 'column', gap: '0.5rem' }}>
                <div style={{ fontSize: '0.8rem', color: 'rgba(255,255,255,0.7)' }}>Simular Rango:</div>
                <select 
                  value={rank} 
                  onChange={(e) => setRank(e.target.value as UserRank)}
                  style={{ 
                    background: 'rgba(0,0,0,0.3)', 
                    color: 'white', 
                    border: '1px solid rgba(255,255,255,0.2)', 
                    padding: '0.25rem',
                    borderRadius: '4px',
                    fontSize: '0.8rem'
                  }}
                >
                  {RANK_ORDER.map(r => (
                    <option key={r} value={r}>{r}</option>
                  ))}
                </select>
              </div>
            )}
          </div>
        </div>
      </div>

      {/* Main Content */}
      <div style={{ flex: 1 }}>
        <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1.5rem' }}>
          <h2 style={{ fontSize: '1.5rem', fontWeight: 700, margin: 0, color: 'var(--text)' }}>Escenarios de Entrenamiento</h2>
          {careerModeEnabled && (
            <div style={{ 
              background: 'var(--bg-card)', 
              padding: '0.5rem 1rem', 
              borderRadius: '20px', 
              border: '1px solid var(--accent)',
              color: 'var(--accent)',
              fontWeight: 600,
              fontSize: '0.9rem',
              display: 'flex',
              alignItems: 'center',
              gap: '0.5rem'
            }}>
              <Trophy size={16} />
              Rango Actual: {rank}
            </div>
          )}
        </div>
        
        {/* Stats Cards */}
        <div style={{ display: 'grid', gridTemplateColumns: 'repeat(3, 1fr)', gap: '1.5rem', marginBottom: '2rem' }}>
          <div className="card">
            <h3 style={{ margin: '0 0 0.5rem', fontSize: '0.9rem', color: 'var(--text-muted)' }}>Último Puntaje</h3>
            <div style={{ fontSize: '2rem', fontWeight: 700, color: 'var(--text)' }}>0%</div>
            <div style={{ fontSize: '0.9rem', color: 'var(--text-muted)' }}>-</div>
          </div>
          <div className="card">
            <h3 style={{ margin: '0 0 0.5rem', fontSize: '0.9rem', color: 'var(--text-muted)' }}>Horas Totales de Práctica</h3>
            <div style={{ fontSize: '2rem', fontWeight: 700, color: 'var(--text)' }}>0h 0m</div>
            <div style={{ fontSize: '0.9rem', color: 'var(--text-muted)' }}>0h 0m esta semana</div>
          </div>
          <div className="card">
            <h3 style={{ margin: '0 0 0.5rem', fontSize: '0.9rem', color: 'var(--text-muted)' }}>Tendencia de Error</h3>
            <div style={{ fontSize: '2rem', fontWeight: 700, color: 'var(--text)' }}>0%</div>
            <a href="#" style={{ fontSize: '0.9rem', color: 'var(--accent)', display: 'flex', alignItems: 'center', gap: '0.25rem' }}>
              Más detalles <ChevronRight size={14} />
            </a>
          </div>
        </div>

        {/* Scenarios Grid */}
        <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fill, minmax(250px, 1fr))', gap: '1.5rem' }}>
          {GAMES.map((g, index) => {
            const isLocked = careerModeEnabled && !canPlayGame(g.requiredRank, rank)
            
            return (
            <Link
              key={g.id}
              to={isLocked ? '#' : g.path}
              onClick={(e) => isLocked && e.preventDefault()}
              style={{
                display: 'block',
                background: 'var(--bg-card)',
                borderRadius: '12px',
                border: '1px solid var(--border)',
                overflow: 'hidden',
                textDecoration: 'none',
                color: 'inherit',
                transition: 'transform 0.2s, box-shadow 0.2s',
                opacity: isLocked ? 0.6 : 1,
                cursor: isLocked ? 'not-allowed' : 'pointer',
                filter: isLocked ? 'grayscale(1)' : 'none'
              }}
              onMouseEnter={(e) => {
                if (!isLocked) {
                  e.currentTarget.style.transform = 'translateY(-4px)'
                  e.currentTarget.style.boxShadow = '0 10px 15px -3px rgba(0, 0, 0, 0.1)'
                }
              }}
              onMouseLeave={(e) => {
                if (!isLocked) {
                  e.currentTarget.style.transform = 'translateY(0)'
                  e.currentTarget.style.boxShadow = 'none'
                }
              }}
            >
              {/* Thumbnail Placeholder */}
              <div style={{ 
                height: '140px', 
                background: g.image ? `url(${g.image}) center/cover no-repeat` : `linear-gradient(135deg, ${['#fca5a5', '#fcd34d', '#86efac', '#93c5fd', '#c4b5fd'][index % 5]} 0%, #e2e8f0 100%)`,
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'center',
                position: 'relative'
              }}>
                {g.image && <div style={{ position: 'absolute', inset: 0, background: 'rgba(0,0,0,0.2)' }} />}
                {isLocked ? (
                  <Lock size={40} color="white" style={{ opacity: 0.9, zIndex: 1, filter: 'drop-shadow(0 2px 4px rgba(0,0,0,0.5))' }} />
                ) : (
                  <PlayCircle size={40} color="white" style={{ opacity: 0.9, zIndex: 1, filter: 'drop-shadow(0 2px 4px rgba(0,0,0,0.5))' }} />
                )}
              </div>
              
              <div style={{ padding: '1rem' }}>
                <h3 style={{ margin: '0 0 0.25rem', fontWeight: 600, fontSize: '1.1rem' }}>{g.title}</h3>
                <div style={{ display: 'flex', alignItems: 'center', gap: '0.25rem', marginBottom: '0.5rem' }}>
                  <span style={{ fontSize: '0.8rem', color: 'var(--text-muted)' }}>
                    {careerModeEnabled && g.requiredRank ? `Req: ${g.requiredRank}` : 'Difficulty:'}
                  </span>
                  {!careerModeEnabled && (
                    <div style={{ display: 'flex', color: 'var(--text)' }}>
                      {'★'.repeat(3 + (index % 3))}{'☆'.repeat(2 - (index % 3))}
                    </div>
                  )}
                </div>
                <p style={{ margin: 0, fontSize: '0.85rem', color: 'var(--text-muted)', display: '-webkit-box', WebkitLineClamp: 2, WebkitBoxOrient: 'vertical', overflow: 'hidden' }}>
                  {g.description}
                </p>
                <div style={{ marginTop: '0.75rem', fontSize: '0.8rem', color: 'var(--text-muted)' }}>
                  Mejor Personal: -
                </div>
              </div>
            </Link>
          )})}
        </div>
      </div>
    </div>
  )
}
