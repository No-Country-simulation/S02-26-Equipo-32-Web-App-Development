import { Link } from 'react-router-dom'
import { GAMES } from '../games'

export default function Home() {
  return (
    <div className="game-container">
      <h1 style={{ marginBottom: '0.5rem' }}>Simulador de cirugía renal</h1>
      <p style={{ color: 'var(--text-muted)', marginBottom: '2rem' }}>
        Entrena precisión y velocidad con estos minijuegos. Se prioriza la perfección sobre la rapidez donde aplique.
      </p>
      <div style={{ display: 'grid', gridTemplateColumns: 'repeat(auto-fill, minmax(280px, 1fr))', gap: '1rem' }}>
        {GAMES.map((g) => (
          <Link
            key={g.id}
            to={g.path}
            style={{
              display: 'block',
              padding: '1.25rem',
              background: 'var(--bg-card)',
              borderRadius: '12px',
              border: '1px solid var(--border)',
              color: 'inherit',
              textDecoration: 'none',
            }}
          >
            <h3 style={{ margin: '0 0 0.5rem', color: 'var(--accent)' }}>{g.title}</h3>
            <p style={{ margin: 0, fontSize: '0.9rem', color: 'var(--text-muted)' }}>{g.description}</p>
          </Link>
        ))}
      </div>
    </div>
  )
}
