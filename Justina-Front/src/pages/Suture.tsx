import { useMemo, useState, useRef, useEffect } from 'react'
import GameFrame from '../components/GameFrame'
import DifficultySelector from '../components/DifficultySelector'
import ErrorFlash from '../components/ErrorFlash'
import GameActions from '../components/GameActions'
import { addResult } from '../store'
import type { Difficulty } from '../types'

const W = 600
const H = 400

function getTargets(d: Difficulty): { x: number; y: number; r: number }[] {
  const base = [
    [120, 150], [280, 120], [450, 160], [500, 280], [350, 320], [180, 300],
  ].map(([x, y]) => ({ x, y, r: 22 }))
  if (d === 'easy') return base.slice(0, 4)
  if (d === 'hard') return [...base, [300, 200], [400, 220]].map(([x, y]) => ({ x, y, r: 20 }))
  return base
}

export default function Suture() {
  const [difficulty, setDifficulty] = useState<Difficulty>('medium')
  const TARGETS = useMemo(() => getTargets(difficulty), [difficulty])
  const [order, setOrder] = useState<number[]>([])
  const [started, setStarted] = useState(false)
  const [timeMs, setTimeMs] = useState(0)
  const [perfection, setPerfection] = useState(0)
  const [finished, setFinished] = useState(false)
  const [errorFlash, setErrorFlash] = useState(false)
  const startRef = useRef(0)
  const intervalRef = useRef<number>(0)

  const start = () => {
    setOrder([])
    setStarted(true)
    setFinished(false)
    setTimeMs(0)
    setErrorFlash(false)
    startRef.current = Date.now()
    intervalRef.current = window.setInterval(() => setTimeMs(Date.now() - startRef.current), 50)
  }
  const playAgain = () => {
    setOrder([])
    setStarted(false)
    setFinished(false)
    setTimeMs(0)
    setErrorFlash(false)
  }
  const goNextLevel = () => {
    setDifficulty((d) => (d === 'easy' ? 'medium' : d === 'medium' ? 'hard' : d))
    playAgain()
  }

  const handleClick = (index: number) => {
    if (!started || finished) return
    const next = order.length
    if (index !== next) {
      setErrorFlash(true)
      return
    }
    setOrder((o) => [...o, index])
    if (order.length + 1 >= TARGETS.length) {
      window.clearInterval(intervalRef.current)
      setFinished(true)
      const t = Date.now() - startRef.current
      const perf = Math.max(60, 100 - t / 200)
      setPerfection(Math.round(perf))
      addResult({ gameId: 'suture', perfection: perf, timeMs: t, difficulty, at: '' })
    }
  }

  useEffect(() => () => clearInterval(intervalRef.current), [])

  return (
    <GameFrame
      title="Colocación de suturas"
      metrics={
        <>
          <DifficultySelector value={difficulty} onChange={setDifficulty} disabled={started && !finished} />
          <span className="metric">Tiempo: {(timeMs / 1000).toFixed(2)} s</span>
          <span className="metric">Puntos: {order.length} / {TARGETS.length}</span>
          {finished && <span className="metric">Perfección: {perfection}%</span>}
        </>
      }
    >
      <p style={{ color: 'var(--text-muted)', marginBottom: '0.5rem' }}>
        Haz clic en los círculos en orden (1 → 2 → …). Orden incorrecto = error crítico.
      </p>
      <GameActions started={started} finished={finished} difficulty={difficulty} onStart={start} onPlayAgain={playAgain} onNextLevel={goNextLevel} />
      <ErrorFlash trigger={errorFlash} onClear={() => setErrorFlash(false)} message="Orden incorrecto" className="canvas-wrap" style={{ width: '100%', maxWidth: W, height: H }}>
        <div style={{ width: W, height: H, position: 'relative', background: '#cbd5e1' }}>
          {TARGETS.map((t, i) => (
            <div
              key={i}
              onClick={() => handleClick(i)}
              style={{
                position: 'absolute',
                left: t.x - t.r,
                top: t.y - t.r,
                width: t.r * 2,
                height: t.r * 2,
                borderRadius: '50%',
                border: '3px solid var(--accent)',
                background: order.includes(i) ? 'var(--accent)' : 'transparent',
                cursor: started && !finished ? 'pointer' : 'default',
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'center',
                color: order.includes(i) ? '#fff' : 'var(--text)',
                fontWeight: 700,
              }}
            >
              {i + 1}
            </div>
          ))}
        </div>
      </ErrorFlash>
    </GameFrame>
  )
}
