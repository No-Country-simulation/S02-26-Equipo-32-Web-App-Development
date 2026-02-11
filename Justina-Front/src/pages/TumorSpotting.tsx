import { useMemo, useState, useRef, useEffect } from 'react'
import GameFrame from '../components/GameFrame'
import DifficultySelector from '../components/DifficultySelector'
import ErrorFlash from '../components/ErrorFlash'
import GameActions from '../components/GameActions'
import { addResult } from '../store'
import type { Difficulty } from '../types'

const W = 500
const H = 350
const TUMOR_R = 20

function getSpots(d: Difficulty): { x: number; y: number }[] {
  const base = [
    { x: 0.25, y: 0.3 }, { x: 0.7, y: 0.5 }, { x: 0.5, y: 0.75 },
  ]
  if (d === 'easy') return base.slice(0, 2)
  if (d === 'hard') return [...base, { x: 0.15, y: 0.6 }, { x: 0.85, y: 0.35 }]
  return base
}

export default function TumorSpotting() {
  const [difficulty, setDifficulty] = useState<Difficulty>('medium')
  const spots = useMemo(() => getSpots(difficulty), [difficulty])
  const [clicked, setClicked] = useState<Set<number>>(new Set())
  const [wrong, setWrong] = useState(0)
  const [started, setStarted] = useState(false)
  const [timeMs, setTimeMs] = useState(0)
  const [done, setDone] = useState(false)
  const [errorFlash, setErrorFlash] = useState(false)
  const startRef = useRef(0)
  const intervalRef = useRef<number>(0)

  const start = () => {
    setClicked(new Set())
    setWrong(0)
    setStarted(true)
    setDone(false)
    setErrorFlash(false)
    setTimeMs(0)
    startRef.current = Date.now()
    intervalRef.current = window.setInterval(() => setTimeMs(Date.now() - startRef.current), 50)
  }
  const playAgain = () => {
    setClicked(new Set())
    setWrong(0)
    setStarted(false)
    setDone(false)
    setErrorFlash(false)
    setTimeMs(0)
  }
  const goNextLevel = () => {
    setDifficulty((d) => (d === 'easy' ? 'medium' : d === 'medium' ? 'hard' : d))
    playAgain()
  }

  const handleClick = (e: React.MouseEvent<HTMLDivElement>) => {
    if (!started || done) return
    const rect = e.currentTarget.getBoundingClientRect()
    const x = (e.clientX - rect.left) / rect.width
    const y = (e.clientY - rect.top) / rect.height
    const hitRadius = difficulty === 'easy' ? 0.1 : difficulty === 'hard' ? 0.06 : 0.08
    let hit = -1
    spots.forEach((s, i) => {
      if (clicked.has(i)) return
      if (Math.hypot(x - s.x, y - s.y) < hitRadius) hit = i
    })
    if (hit >= 0) {
      const next = new Set(clicked).add(hit)
      setClicked(next)
      if (next.size >= spots.length) {
        window.clearInterval(intervalRef.current)
        setDone(true)
        const t = Date.now() - startRef.current
        const perf = Math.max(0, 100 - wrong * 15 - t / 300)
        addResult({ gameId: 'tumor-spotting', perfection: perf, timeMs: t, difficulty, extra: { wrongClicks: wrong }, at: '' })
      }
    } else {
      setWrong((w) => w + 1)
      setErrorFlash(true)
    }
  }

  useEffect(() => () => clearInterval(intervalRef.current), [])

  return (
    <GameFrame
      title="Detección de tumores"
      metrics={
        <>
          <DifficultySelector value={difficulty} onChange={setDifficulty} disabled={started && !done} />
          <span className="metric">Tiempo: {(timeMs / 1000).toFixed(2)} s</span>
          <span className="metric">Tumores: {clicked.size} / {spots.length}</span>
          {wrong > 0 && <span className="metric" style={{ color: 'var(--danger)' }}>Errores: {wrong}</span>}
        </>
      }
    >
      <p style={{ color: 'var(--text-muted)', marginBottom: '0.5rem' }}>
        Haz clic solo en los tumores (manchas). Clic incorrecto = error crítico.
      </p>
      <GameActions started={started} finished={done} difficulty={difficulty} onStart={start} onPlayAgain={playAgain} onNextLevel={goNextLevel} />
      <ErrorFlash trigger={errorFlash} onClear={() => setErrorFlash(false)} className="canvas-wrap" style={{ width: '100%', maxWidth: W, height: H }}>
        <div style={{ width: '100%', height: '100%', cursor: 'crosshair' }} onClick={handleClick}>
          <svg width={W} height={H} viewBox="0 0 1 1" preserveAspectRatio="none" style={{ display: 'block', width: '100%', height: '100%' }}>
            <rect width="1" height="1" fill="#cbd5e1" />
            {spots.map((s, i) => (
              <circle
                key={i}
                cx={s.x}
                cy={s.y}
                r={difficulty === 'hard' ? 0.045 : 0.06}
                fill={clicked.has(i) ? 'var(--success)' : '#475569'}
              />
            ))}
          </svg>
        </div>
      </ErrorFlash>
    </GameFrame>
  )
}
