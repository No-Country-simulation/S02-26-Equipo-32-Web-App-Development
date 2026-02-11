import { useState, useRef, useEffect } from 'react'
import GameFrame from '../components/GameFrame'
import DifficultySelector from '../components/DifficultySelector'
import ErrorFlash from '../components/ErrorFlash'
import GameActions from '../components/GameActions'
import { addResult } from '../store'
import type { Difficulty } from '../types'

const NAMES = ['Bisturí', 'Pinza', 'Tijeras', 'Sutura', 'Aspirador', 'Gancho', 'Electrodo']
const COLORS = ['#64748b', '#0ea5e9', '#eab308', '#3b82f6', '#a855f7', '#ec4899', '#06b6d4']

function getN(d: Difficulty): number {
  return d === 'easy' ? 3 : d === 'medium' ? 5 : 7
}

export default function InstrumentSequence() {
  const [difficulty, setDifficulty] = useState<Difficulty>('medium')
  const N = getN(difficulty)
  const [phase, setPhase] = useState<'idle' | 'show' | 'input'>('idle')
  const [sequence, setSequence] = useState<number[]>([])
  const [input, setInput] = useState<number[]>([])
  const [timeMs, setTimeMs] = useState(0)
  const [perfection, setPerfection] = useState(0)
  const [done, setDone] = useState(false)
  const [errorFlash, setErrorFlash] = useState(false)
  const seqRef = useRef<number[]>([])
  const startRef = useRef(0)
  const intervalRef = useRef<number>(0)

  const start = () => {
    const seq = Array.from({ length: N }, (_, i) => i)
    for (let i = seq.length - 1; i > 0; i--) {
      const j = Math.floor(Math.random() * (i + 1));
      [seq[i], seq[j]] = [seq[j], seq[i]]
    }
    seqRef.current = seq
    setSequence(seq)
    setInput([])
    setPhase('show')
    setDone(false)
    setErrorFlash(false)
    setTimeMs(0)
    setTimeout(() => {
      setPhase('input')
      startRef.current = Date.now()
      intervalRef.current = window.setInterval(() => setTimeMs(Date.now() - startRef.current), 50)
    }, difficulty === 'easy' ? 2500 : difficulty === 'hard' ? 4000 : 3000)
  }

  const handleClick = (i: number) => {
    if (phase !== 'input' || done) return
    const next = [...input, i]
    if (i !== seqRef.current[next.length - 1]) {
      setErrorFlash(true)
      return
    }
    setInput(next)
    if (next.length === N) {
      window.clearInterval(intervalRef.current)
      setDone(true)
      const perf = 100 - (Date.now() - startRef.current) / 500
      setPerfection(Math.round(Math.max(0, perf)))
      addResult({ gameId: 'instrument-sequence', perfection: Math.max(0, perf), timeMs: Date.now() - startRef.current, difficulty, at: '' })
    }
  }

  const playAgain = () => {
    setPhase('idle')
    setSequence([])
    setInput([])
    setDone(false)
    setErrorFlash(false)
    setTimeMs(0)
  }
  const goNextLevel = () => {
    setDifficulty((d) => (d === 'easy' ? 'medium' : d === 'medium' ? 'hard' : d))
    playAgain()
  }

  useEffect(() => () => clearInterval(intervalRef.current), [])

  return (
    <GameFrame
      title="Secuencia de instrumentos"
      metrics={
        <>
          <DifficultySelector value={difficulty} onChange={setDifficulty} disabled={phase !== 'idle'} />
          <span className="metric">Tiempo: {(timeMs / 1000).toFixed(2)} s</span>
          <span className="metric">Entrada: {input.length} / {N}</span>
          {done && <span className="metric">Perfección: {perfection}%</span>}
        </>
      }
    >
      <p style={{ color: 'var(--text-muted)', marginBottom: '0.5rem' }}>
        Memoriza el orden y repítelo. Instrumento incorrecto = error crítico.
      </p>
      <GameActions started={phase !== 'idle'} finished={done} difficulty={difficulty} onStart={start} onPlayAgain={playAgain} onNextLevel={goNextLevel} />
      {phase === 'show' && (
        <div style={{ display: 'flex', gap: '0.5rem', flexWrap: 'wrap', marginBottom: '1rem' }}>
          {sequence.map((i, idx) => (
            <span key={idx} style={{ padding: '0.5rem 1rem', background: COLORS[i], color: '#fff', borderRadius: 8 }}>{NAMES[i]}</span>
          ))}
        </div>
      )}
      {phase === 'input' && (
        <ErrorFlash trigger={errorFlash} onClear={() => setErrorFlash(false)} message="Secuencia incorrecta">
          <div style={{ display: 'flex', gap: '0.5rem', flexWrap: 'wrap' }}>
            {NAMES.slice(0, N).map((name, i) => (
              <button
                key={i}
                onClick={() => handleClick(i)}
                disabled={done}
                style={{ background: COLORS[i], color: '#fff' }}
              >
                {name}
              </button>
            ))}
          </div>
        </ErrorFlash>
      )}
      {input.length > 0 && (
        <p style={{ marginTop: '1rem', color: 'var(--text-muted)' }}>
          Tu secuencia: {input.map((i) => NAMES[i]).join(' → ')}
        </p>
      )}
    </GameFrame>
  )
}