import { useState, useCallback, useRef, useEffect } from 'react'
import GameFrame from '../components/GameFrame'
import DifficultySelector from '../components/DifficultySelector'
import ErrorFlash from '../components/ErrorFlash'
import GameActions from '../components/GameActions'
import { addResult } from '../store'
import type { Difficulty } from '../types'

function getN(d: Difficulty): number {
  return d === 'easy' ? 6 : d === 'medium' ? 12 : 18
}

const W = 600
const H = 450
const TARGET_R = 18
/** Solo nivel difícil: tiempo (ms) que cada punto permanece visible antes de apagarse. */
const HARD_VISIBLE_MS = 1200
/** Solo nivel difícil: pausa (ms) entre apagar un punto y mostrar el siguiente. */
const HARD_GAP_MS = 400

export default function Reflex() {
  const [difficulty, setDifficulty] = useState<Difficulty>('medium')
  const N = getN(difficulty)
  const isBlinkMode = difficulty === 'hard'
  const [points, setPoints] = useState<{ x: number; y: number; id: number; clicked?: boolean }[]>([])
  const [visibleId, setVisibleId] = useState<number | null>(null)
  const [timeMs, setTimeMs] = useState(0)
  const [reactionTimes, setReactionTimes] = useState<number[]>([])
  const [phase, setPhase] = useState<'idle' | 'ready' | 'play'>('idle')
  const [roundCompleted, setRoundCompleted] = useState(false)
  const [clickCount, setClickCount] = useState(0)
  const [errorFlash, setErrorFlash] = useState(false)
  const [missCount, setMissCount] = useState(0)
  const timerRef = useRef<number>(0)
  const showAtRef = useRef(0)
  const reactionTimesRef = useRef<number[]>([])
  const hideTimeoutRef = useRef<number>(0)
  const nextShowTimeoutRef = useRef<number>(0)
  const currentIndexRef = useRef(0)
  const missCountRef = useRef(0)

  const spawn = useCallback(() => {
    const list: { x: number; y: number; id: number; clicked?: boolean }[] = []
    for (let i = 0; i < N; i++) {
      list.push({
        x: 40 + Math.random() * (W - 80),
        y: 40 + Math.random() * (H - 80),
        id: i,
      })
    }
    setPoints(list)
    setReactionTimes([])
    reactionTimesRef.current = []
    setClickCount(0)
    setMissCount(0)
    missCountRef.current = 0
    setVisibleId(null)
    currentIndexRef.current = 0
    setPhase('play')
    setTimeMs(0)
    showAtRef.current = Date.now()
    timerRef.current = window.setInterval(() => setTimeMs(Date.now() - showAtRef.current), 50)

    if (isBlinkMode) {
      nextShowTimeoutRef.current = window.setTimeout(() => {
        setVisibleId(0)
        showAtRef.current = Date.now()
        hideTimeoutRef.current = window.setTimeout(() => {
          setVisibleId(null)
          setMissCount((m) => { const n = m + 1; missCountRef.current = n; return n; })
          currentIndexRef.current = 1
          nextShowTimeoutRef.current = window.setTimeout(showNextHard, HARD_GAP_MS)
        }, HARD_VISIBLE_MS)
      }, 400)
    }
  }, [N, isBlinkMode])

  const showNextHard = useCallback(() => {
    const idx = currentIndexRef.current
    if (idx >= N) {
      clearInterval(timerRef.current)
      const times = reactionTimesRef.current
      const totalTime = Date.now() - showAtRef.current
      const avgRt = times.length ? times.reduce((a, b) => a + b, 0) / times.length : 0
      const misses = missCountRef.current
      const perfection = Math.max(0, 100 - (avgRt / 500) * 30 - misses * 5)
      addResult({ gameId: 'reflex', perfection, timeMs: totalTime, difficulty, extra: { avgReactionMs: avgRt, misses }, at: '' })
      setPhase('idle')
      setTimeMs(totalTime)
      setRoundCompleted(true)
      return
    }
    setVisibleId(idx)
    showAtRef.current = Date.now()
    hideTimeoutRef.current = window.setTimeout(() => {
      setVisibleId(null)
      setMissCount((m) => { const n = m + 1; missCountRef.current = n; return n; })
      currentIndexRef.current = idx + 1
      nextShowTimeoutRef.current = window.setTimeout(showNextHard, HARD_GAP_MS)
    }, HARD_VISIBLE_MS)
  }, [N])

  useEffect(() => {
    return () => {
      clearInterval(timerRef.current)
      clearTimeout(hideTimeoutRef.current)
      clearTimeout(nextShowTimeoutRef.current)
    }
  }, [])

  const handleClick = (id: number) => {
    if (phase !== 'play') return
    if (isBlinkMode && visibleId !== id) return
    const now = Date.now()
    const rt = now - showAtRef.current
    reactionTimesRef.current.push(rt)
    setReactionTimes((r) => [...r, rt])
    setPoints((p) => p.map((q) => (q.id === id ? { ...q, clicked: true } : q)))
    setClickCount((c) => {
      const next = c + 1
      if (isBlinkMode) {
        clearTimeout(hideTimeoutRef.current)
        setVisibleId(null)
        currentIndexRef.current = id + 1
        if (next >= N) {
          clearInterval(timerRef.current)
          clearTimeout(nextShowTimeoutRef.current)
          const times = reactionTimesRef.current
          const totalTime = now - showAtRef.current
          const avgRt = times.reduce((a, b) => a + b, 0) / times.length
          const misses = missCountRef.current
          const perfection = Math.max(0, 100 - (avgRt / 500) * 30 - misses * 5)
          addResult({ gameId: 'reflex', perfection, timeMs: totalTime, difficulty, extra: { avgReactionMs: avgRt, misses }, at: '' })
          setPhase('idle')
          setTimeMs(totalTime)
          setRoundCompleted(true)
        } else {
          nextShowTimeoutRef.current = window.setTimeout(showNextHard, HARD_GAP_MS)
        }
        return next
      }
      if (next >= N) {
        clearInterval(timerRef.current)
        const times = reactionTimesRef.current
        const totalTime = now - showAtRef.current
        const avgRt = times.reduce((a, b) => a + b, 0) / times.length
        const perfection = Math.max(0, 100 - (avgRt / 500) * 30 - missCount * 5)
        addResult({ gameId: 'reflex', perfection, timeMs: totalTime, difficulty, extra: { avgReactionMs: avgRt, misses: missCount }, at: '' })
        setPhase('idle')
        setTimeMs(totalTime)
        setRoundCompleted(true)
      }
      return next
    })
  }

  const playAgain = () => {
    setRoundCompleted(false)
    setPoints([])
    setPhase('idle')
  }
  const goNextLevel = () => {
    setDifficulty((d) => (d === 'easy' ? 'medium' : d === 'medium' ? 'hard' : d))
    setRoundCompleted(false)
    setPoints([])
    setPhase('idle')
  }

  const handleCanvasClick = (e: React.MouseEvent<HTMLDivElement>) => {
    if (phase !== 'play') return
    const target = e.target as HTMLElement
    if (target.getAttribute('data-target') === 'true') return
    setErrorFlash(true)
    setMissCount((m) => m + 1)
  }

  return (
    <GameFrame
      title="Reflejos - Flujo sanguíneo"
      metrics={
        <>
          <DifficultySelector value={difficulty} onChange={setDifficulty} disabled={phase === 'play'} />
          <span className="metric">Tiempo: {(timeMs / 1000).toFixed(2)} s</span>
          <span className="metric">Puntos: {clickCount} / {N}</span>
          {missCount > 0 && <span className="metric" style={{ color: 'var(--danger)' }}>Errores: {missCount}</span>}
          {reactionTimes.length > 0 && (
            <span className="metric">Reacción media: {(reactionTimes.reduce((a, b) => a + b, 0) / reactionTimes.length).toFixed(0)} ms</span>
          )}
        </>
      }
    >
      <p style={{ color: 'var(--text-muted)', marginBottom: '0.5rem' }}>
        {isBlinkMode
          ? 'Nivel difícil: los puntos se encienden y se apagan. Haz clic en cada uno mientras esté encendido para medir reflejos.'
          : 'Toca solo los puntos rojos. Clic fuera = error crítico.'}
      </p>
      {phase === 'idle' && !roundCompleted && (
        <button onClick={() => setPhase('ready')} style={{ marginBottom: '1rem' }}>Preparar</button>
      )}
      {phase === 'ready' && (
        <button onClick={spawn} style={{ marginBottom: '1rem' }}>Iniciar</button>
      )}
      <GameActions
        started={phase !== 'idle' || roundCompleted}
        finished={roundCompleted}
        difficulty={difficulty}
        showStartButton={false}
        onStart={() => setPhase('ready')}
        onPlayAgain={playAgain}
        onNextLevel={goNextLevel}
      />
      <ErrorFlash trigger={errorFlash} onClear={() => setErrorFlash(false)} className="canvas-wrap" style={{ width: '100%', maxWidth: W, height: H }}>
        <div style={{ width: W, height: H, position: 'relative', background: '#cbd5e1' }} onClick={handleCanvasClick}>
          {points.map((p) => {
            const showInHard = isBlinkMode ? visibleId === p.id : true
            const isClicked = p.clicked
            if (isBlinkMode && !showInHard && !isClicked) return null
            return (
              <button
                key={p.id}
                data-target="true"
                onClick={(e) => { e.stopPropagation(); handleClick(p.id); }}
                disabled={isClicked}
                style={{
                  position: 'absolute',
                  left: p.x - 14,
                  top: p.y - 14,
                  width: 28,
                  height: 28,
                  borderRadius: '50%',
                  background: isClicked ? 'var(--success)' : showInHard ? '#dc2626' : 'transparent',
                  border: `2px solid ${showInHard || isClicked ? '#fff' : 'transparent'}`,
                  padding: 0,
                }}
              />
            )
          })}
        </div>
      </ErrorFlash>
    </GameFrame>
  )
}