import { Link } from 'react-router-dom'
import { getStatsForGame, getAllResults } from '../store'
import { GAMES } from '../games'
import type { Difficulty } from '../types'

const DIFF_LABEL: Record<Difficulty, string> = { easy: 'Fácil', medium: 'Medio', hard: 'Difícil' }

export default function Results() {
  const results = getAllResults()

  return (
    <div className="game-container">
      <h1>Resultados y estadísticas</h1>
      <p style={{ color: 'var(--text-muted)', marginBottom: '1.5rem' }}>
        Precisión, tiempo y número de intentos por minijuego. Útil para evaluar entrenamiento de cirujanos o robots.
      </p>
      <div style={{ display: 'flex', flexDirection: 'column', gap: '1.25rem' }}>
        {GAMES.map((game) => {
          const stats = getStatsForGame(game.id)
          const hasData = stats.attempts > 0
          return (
            <div
              key={game.id}
              style={{
                padding: '1.25rem',
                background: 'var(--bg-card)',
                borderRadius: '12px',
                border: '1px solid var(--border)',
              }}
            >
              <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', flexWrap: 'wrap', gap: '0.5rem', marginBottom: '0.75rem' }}>
                <strong>{game.title}</strong>
                <Link to={game.path}>Jugar</Link>
              </div>
              {!hasData ? (
                <span style={{ color: 'var(--text-muted)', fontSize: '0.9rem' }}>Sin partidas aún</span>
              ) : (
                <>
                  <div style={{ display: 'flex', gap: '1rem', flexWrap: 'wrap', marginBottom: '0.75rem' }}>
                    <span className="metric">Intentos: {stats.attempts}</span>
                    <span className="metric">Mejor perfección: {Math.round(stats.bestPerfection)}%</span>
                    <span className="metric">Mejor tiempo: {(stats.bestTimeMs / 1000).toFixed(2)} s</span>
                    <span className="metric">Promedio perfección: {Math.round(stats.avgPerfection)}%</span>
                    <span className="metric">Promedio tiempo: {(stats.avgTimeMs / 1000).toFixed(2)} s</span>
                  </div>
                  <div style={{ fontSize: '0.85rem', color: 'var(--text-muted)' }}>
                    Por dificultad —{' '}
                    {(['easy', 'medium', 'hard'] as const).map((d) => (
                      <span key={d}>
                        {DIFF_LABEL[d]}: {stats.byDifficulty[d].attempts} partidas
                        {stats.byDifficulty[d].attempts > 0 && (
                          <> (media {Math.round(stats.byDifficulty[d].avgPerfection)}%, {(stats.byDifficulty[d].avgTimeMs / 1000).toFixed(1)} s)</>
                        )}
                        {d !== 'hard' && ' · '}
                      </span>
                    ))}
                  </div>
                </>
              )}
            </div>
          )
        })}
      </div>
      {results.length > 0 && (
        <p style={{ marginTop: '1.5rem', color: 'var(--text-muted)', fontSize: '0.9rem' }}>
          Total: {results.length} partida(s) registrada(s). Los datos se guardan en este dispositivo.
        </p>
      )}
    </div>
  )
}
