import { useRef, useEffect, useState, useCallback } from 'react'
import GameFrame from '../components/GameFrame'
import DifficultySelector from '../components/DifficultySelector'
import ErrorFlash from '../components/ErrorFlash'
import GameActions from '../components/GameActions'
import { addResult } from '../store'
import type { Difficulty } from '../types'

const W = 500
const H = 400
const CENTER = { x: W / 2, y: H / 2 }

function getParams(d: Difficulty) {
  return d === 'easy'
    ? { zoneR: 50, durationMs: 3000, amplitude: 18, speed: 0.0022 }
    : d === 'medium'
    ? { zoneR: 35, durationMs: 5000, amplitude: 38, speed: 0.0028 }
    : { zoneR: 22, durationMs: 7000, amplitude: 58, speed: 0.0035 }
}

/** Centro de la zona en un instante; más difícil = más amplitud y velocidad. */
function getZoneCenter(elapsedMs: number, amplitude: number, speed: number): { x: number; y: number } {
  const t = elapsedMs * speed
  return {
    x: CENTER.x + amplitude * Math.sin(t),
    y: CENTER.y + amplitude * 0.75 * Math.cos(t * 1.3),
  }
}

const CANVAS_BG = '#cbd5e1'

export default function SteadyHand() {
  const [difficulty, setDifficulty] = useState<Difficulty>('medium')
  const params = getParams(difficulty)
  const { zoneR: ZONE_R, durationMs: DURATION_MS, amplitude, speed } = params
  const canvasRef = useRef<HTMLCanvasElement>(null)
  const [pos, setPos] = useState(CENTER)
  const [zoneCenter, setZoneCenter] = useState(CENTER)
  const [started, setStarted] = useState(false)
  const [finished, setFinished] = useState(false)
  const [timeHeld, setTimeHeld] = useState(0)
  const [exits, setExits] = useState(0)
  const [perfection, setPerfection] = useState(0)
  const [errorFlash, setErrorFlash] = useState(false)
  const startRef = useRef(0)
  const insideRef = useRef(true)
  const driftRef = useRef<number[]>([])

  const draw = useCallback((ctx: CanvasRenderingContext2D) => {
    ctx.fillStyle = CANVAS_BG
    ctx.fillRect(0, 0, W, H)
    ctx.fillStyle = 'rgba(56, 189, 248, 0.25)'
    ctx.beginPath()
    ctx.arc(zoneCenter.x, zoneCenter.y, ZONE_R, 0, Math.PI * 2)
    ctx.fill()
    ctx.strokeStyle = 'var(--accent)'
    ctx.lineWidth = 2
    ctx.stroke()
    ctx.fillStyle = 'var(--accent)'
    ctx.beginPath()
    ctx.arc(pos.x, pos.y, 10, 0, Math.PI * 2)
    ctx.fill()
  }, [pos, zoneCenter, ZONE_R])

  useEffect(() => {
    const c = canvasRef.current
    if (!c) return
    const ctx = c.getContext('2d')
    if (!ctx) return
    draw(ctx)
  }, [draw])

  useEffect(() => {
    if (!started || finished) return
    const id = setInterval(() => {
      const elapsed = Date.now() - startRef.current
      setZoneCenter(getZoneCenter(elapsed, amplitude, speed))
      if (elapsed >= DURATION_MS) {
        setFinished(true)
        const perf = Math.max(0, 100 - exits * 12 - (driftRef.current.length ? driftRef.current.reduce((a, b) => a + b, 0) / driftRef.current.length / 2 : 0))
        setPerfection(Math.round(perf))
        addResult({ gameId: 'steady-hand', perfection: perf, timeMs: DURATION_MS, difficulty, extra: { exits, avgDrift: driftRef.current.length ? driftRef.current.reduce((a, b) => a + b, 0) / driftRef.current.length : 0 }, at: '' })
        return
      }
      setTimeHeld(elapsed)
    }, 50)
    return () => clearInterval(id)
  }, [started, finished, exits, DURATION_MS, amplitude, speed])

  const onMouseMove = (e: React.MouseEvent<HTMLCanvasElement>) => {
    const canvas = canvasRef.current
    if (!canvas) return
    const rect = canvas.getBoundingClientRect()
    const x = (e.clientX - rect.left) * (canvas.width / rect.width)
    const y = (e.clientY - rect.top) * (canvas.height / rect.height)
    setPos({ x, y })
    if (!started || finished) return
    const elapsed = Date.now() - startRef.current
    const center = getZoneCenter(elapsed, amplitude, speed)
    const d = Math.hypot(x - center.x, y - center.y)
    driftRef.current.push(d)
    if (d > ZONE_R) {
      if (insideRef.current) {
        insideRef.current = false
        setExits((ex) => ex + 1)
        setErrorFlash(true)
      }
    } else {
      insideRef.current = true
    }
  }

  const start = () => {
    setStarted(true)
    setFinished(false)
    setTimeHeld(0)
    setExits(0)
    setErrorFlash(false)
    setZoneCenter(CENTER)
    driftRef.current = []
    insideRef.current = true
    startRef.current = Date.now()
  }
  const playAgain = () => {
    setStarted(false)
    setFinished(false)
    setTimeHeld(0)
    setExits(0)
    setErrorFlash(false)
    setZoneCenter(CENTER)
    driftRef.current = []
    insideRef.current = true
  }
  const goNextLevel = () => {
    setDifficulty((d) => (d === 'easy' ? 'medium' : d === 'medium' ? 'hard' : d))
    playAgain()
  }

  return (
    <GameFrame
      title="Mano estable"
      metrics={
        <>
          <DifficultySelector value={difficulty} onChange={setDifficulty} disabled={started && !finished} />
          <span className="metric">Tiempo: {(timeHeld / 1000).toFixed(1)} / {(DURATION_MS / 1000).toFixed(0)} s</span>
          <span className="metric">Salidas de zona: {exits}</span>
          {finished && <span className="metric">Perfección: {perfection}%</span>}
        </>
      }
    >
      <p style={{ color: 'var(--text-muted)', marginBottom: '0.5rem' }}>
        Sigue el círculo con el cursor sin salir. El círculo se mueve; a más dificultad, más movimiento. Cada salida = error crítico.
      </p>
      <GameActions started={started} finished={finished} difficulty={difficulty} onStart={start} onPlayAgain={playAgain} onNextLevel={goNextLevel} />
      <ErrorFlash trigger={errorFlash} onClear={() => setErrorFlash(false)} className="canvas-wrap" style={{ width: '100%', maxWidth: W, height: H }}>
        <canvas ref={canvasRef} width={W} height={H} onMouseMove={onMouseMove} style={{ cursor: 'none' }} />
      </ErrorFlash>
    </GameFrame>
  )
}
