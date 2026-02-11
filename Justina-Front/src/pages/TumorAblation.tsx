import { useState, useRef, useCallback, useMemo, useEffect } from 'react'
import { Canvas, useThree, useFrame } from '@react-three/fiber'
import { OrbitControls } from '@react-three/drei'
import * as THREE from 'three'
import GameFrame from '../components/GameFrame'
import DifficultySelector from '../components/DifficultySelector'
import ErrorFlash from '../components/ErrorFlash'
import GameActions from '../components/GameActions'
import { addResult } from '../store'
import type { Difficulty } from '../types'

function getParams(d: Difficulty) {
  return d === 'easy' ? { count: 5, radius: 0.45 } : d === 'medium' ? { count: 8, radius: 0.4 } : { count: 12, radius: 0.32 }
}

/** Caja 6×4×4 centrada en origen: x ∈ [-3,3], y ∈ [-2,2], z ∈ [-2,2]. */
const BOX_HALF = { x: 3, y: 2, z: 2 }

/**
 * Genera posiciones de tumores pegados a la superficie de la estructura (caja).
 * El centro de cada tumor queda justo fuera de la caja (radio + mínimo margen) para que no floten.
 */
function generateTumorPositionsOutsideStructure(count: number, radius: number): [number, number, number][] {
  const gap = 0.05
  const outX = BOX_HALF.x + radius + gap
  const outY = BOX_HALF.y + radius + gap
  const outZ = BOX_HALF.z + radius + gap
  const faces: Array<() => [number, number, number]> = [
    () => [outX, (Math.random() - 0.5) * (BOX_HALF.y * 1.6), (Math.random() - 0.5) * (BOX_HALF.z * 1.6)],
    () => [-outX, (Math.random() - 0.5) * (BOX_HALF.y * 1.6), (Math.random() - 0.5) * (BOX_HALF.z * 1.6)],
    () => [(Math.random() - 0.5) * (BOX_HALF.x * 1.6), outY, (Math.random() - 0.5) * (BOX_HALF.z * 1.6)],
    () => [(Math.random() - 0.5) * (BOX_HALF.x * 1.6), -outY, (Math.random() - 0.5) * (BOX_HALF.z * 1.6)],
    () => [(Math.random() - 0.5) * (BOX_HALF.x * 1.6), (Math.random() - 0.5) * (BOX_HALF.y * 1.6), outZ],
    () => [(Math.random() - 0.5) * (BOX_HALF.x * 1.6), (Math.random() - 0.5) * (BOX_HALF.y * 1.6), -outZ],
  ]
  return Array.from({ length: count }, () => faces[Math.floor(Math.random() * faces.length)]())
}

type Mode = 'camera' | 'laser'

function Structure({ innerRef }: { innerRef: React.RefObject<THREE.Mesh | null> }) {
  return (
    <mesh
      ref={(el) => {
        (innerRef as React.MutableRefObject<THREE.Mesh | null>).current = el
        if (el) el.userData.isStructure = true
      }}
    >
      <boxGeometry args={[6, 4, 4]} />
      <meshStandardMaterial color="#94a3b8" wireframe />
    </mesh>
  )
}

function LaserRaycaster({
  mode,
  structureRef,
  tumorRefs,
  hit,
  onBurn,
  onStructureHit,
}: {
  mode: Mode
  structureRef: React.RefObject<THREE.Mesh | null>
  tumorRefs: React.MutableRefObject<(THREE.Mesh | null)[]>
  hit: Set<number>
  onBurn: (i: number) => void
  onStructureHit: () => void
}) {
  const { camera, pointer } = useThree()
  const raycaster = useMemo(() => new THREE.Raycaster(), [])
  const mouse = useMemo(() => new THREE.Vector2(), [])
  const lastHitTumor = useRef<number | null>(null)
  const lastHitStructure = useRef(false)

  useFrame(() => {
    if (mode !== 'laser') {
      lastHitTumor.current = null
      lastHitStructure.current = false
      return
    }
    mouse.set(pointer.x, pointer.y)
    raycaster.setFromCamera(mouse, camera)
    const allObjects: THREE.Object3D[] = []
    if (structureRef.current) allObjects.push(structureRef.current)
    tumorRefs.current.forEach((m, idx) => {
      if (m && !hit.has(idx)) allObjects.push(m)
    })
    const hits = raycaster.intersectObjects(allObjects, false)
    const first = hits[0]
    if (!first) {
      lastHitTumor.current = null
      lastHitStructure.current = false
      return
    }
    const obj = first.object as THREE.Mesh & { userData: { tumorIndex?: number; isStructure?: boolean } }
    if (obj.userData.tumorIndex !== undefined) {
      const i = obj.userData.tumorIndex
      lastHitStructure.current = false
      if (lastHitTumor.current !== i) {
        lastHitTumor.current = i
        onBurn(i)
      }
      return
    }
    lastHitTumor.current = null
    if (obj.userData.isStructure) {
      if (!lastHitStructure.current) {
        lastHitStructure.current = true
        onStructureHit()
      }
    } else {
      lastHitStructure.current = false
    }
  })

  return null
}

export default function TumorAblation() {
  const [difficulty, setDifficulty] = useState<Difficulty>('medium')
  const { count: COUNT, radius: RADIUS } = getParams(difficulty)
  const [mode, setMode] = useState<Mode>('camera')
  const [started, setStarted] = useState(false)
  const [timeMs, setTimeMs] = useState(0)
  const [hitCount, setHitCount] = useState(0)
  const [gameComplete, setGameComplete] = useState(false)
  const [errorFlash, setErrorFlash] = useState(false)
  const [structureErrors, setStructureErrors] = useState(0)
  const startRef = useRef(0)
  const intervalRef = useRef<number>(0)
  const structureRef = useRef<THREE.Mesh | null>(null)
  const tumorRefs = useRef<(THREE.Mesh | null)[]>([])
  const structureErrorsRef = useRef(0)

  const positions = useMemo(
    () => generateTumorPositionsOutsideStructure(COUNT, RADIUS),
    [COUNT, RADIUS]
  )
  const [hit, setHit] = useState<Set<number>>(new Set())

  useEffect(() => {
    tumorRefs.current = new Array(COUNT).fill(null)
  }, [COUNT])

  const handleBurn = useCallback(
    (i: number) => {
      setHit((h) => new Set(h).add(i))
      setHitCount((n) => {
        const next = n + 1
        if (next >= COUNT) {
          clearInterval(intervalRef.current)
          const t = Date.now() - startRef.current
          setTimeMs(t)
          setGameComplete(true)
          const errs = structureErrorsRef.current
          const perf = Math.max(0, 100 - (t / 20000) * 20 - errs * 15)
          addResult({ gameId: 'tumor-ablation', perfection: perf, timeMs: t, difficulty, extra: { structureErrors: errs }, at: '' })
        }
        return next
      })
    },
    [COUNT]
  )

  const handleStructureHit = useCallback(() => {
    setErrorFlash(true)
    setStructureErrors((e) => {
      const next = e + 1
      structureErrorsRef.current = next
      return next
    })
  }, [])

  const start = () => {
    setStarted(true)
    setGameComplete(false)
    setHitCount(0)
    setHit(new Set())
    setStructureErrors(0)
    structureErrorsRef.current = 0
    setTimeMs(0)
    setMode('camera')
    startRef.current = Date.now()
    intervalRef.current = window.setInterval(() => setTimeMs(Date.now() - startRef.current), 100)
  }
  const playAgain = () => {
    setStarted(false)
    setGameComplete(false)
    setHitCount(0)
    setHit(new Set())
    setStructureErrors(0)
    structureErrorsRef.current = 0
    setTimeMs(0)
    setMode('camera')
  }
  const goNextLevel = () => {
    setDifficulty((d) => (d === 'easy' ? 'medium' : d === 'medium' ? 'hard' : d))
    playAgain()
  }

  return (
    <GameFrame
      title="Ablación 3D de tumores"
      metrics={
        <>
          <DifficultySelector value={difficulty} onChange={setDifficulty} disabled={started} />
          <span className="metric">Tiempo: {(timeMs / 1000).toFixed(2)} s</span>
          <span className="metric">Tumores: {hitCount} / {COUNT}</span>
          {structureErrors > 0 && (
            <span className="metric" style={{ color: 'var(--danger)' }}>Errores estructura: {structureErrors}</span>
          )}
        </>
      }
    >
      <p style={{ color: 'var(--text-muted)', marginBottom: '0.5rem' }}>
        <strong>Cámara:</strong> rota la vista. <strong>Láser:</strong> apunta con el cursor sobre los tumores para quemarlos; si tocas la estructura (rejilla) = error crítico.
      </p>
      <GameActions started={started} finished={gameComplete} difficulty={difficulty} onStart={start} onPlayAgain={playAgain} onNextLevel={goNextLevel} />
      {started && (
        <div style={{ display: 'flex', gap: '0.5rem', marginBottom: '0.5rem', flexWrap: 'wrap' }}>
          <button
            type="button"
            onClick={() => setMode('camera')}
            style={{
              background: mode === 'camera' ? 'var(--accent)' : 'var(--bg-card)',
              color: mode === 'camera' ? '#fff' : 'var(--text)',
              border: `1px solid ${mode === 'camera' ? 'var(--accent)' : 'var(--border)'}`,
            }}
          >
            Cámara (rotar)
          </button>
          <button
            type="button"
            onClick={() => setMode('laser')}
            style={{
              background: mode === 'laser' ? 'var(--danger)' : 'var(--bg-card)',
              color: mode === 'laser' ? '#fff' : 'var(--text)',
              border: `1px solid ${mode === 'laser' ? 'var(--danger)' : 'var(--border)'}`,
            }}
          >
            Láser (quemar)
          </button>
        </div>
      )}
      <ErrorFlash trigger={errorFlash} onClear={() => setErrorFlash(false)} className="canvas-wrap" style={{ width: '100%', maxWidth: 700, height: 450 }}>
        <div style={{ width: '100%', maxWidth: 700, height: 450 }}>
          <Canvas camera={{ position: [4, 2, 6], fov: 50 }}>
            <ambientLight intensity={0.6} />
            <directionalLight position={[5, 5, 5]} intensity={1} />
            <OrbitControls enabled={mode === 'camera'} />
            <Structure innerRef={structureRef} />
            {started &&
              positions.map((pos, i) => {
                if (hit.has(i)) return null
                return (
                  <mesh
                    key={i}
                    position={pos}
                    ref={(el) => {
                      if (el) {
                        tumorRefs.current[i] = el
                        el.userData.tumorIndex = i
                      }
                    }}
                  >
                    <sphereGeometry args={[RADIUS, 24, 24]} />
                    <meshStandardMaterial color="#c2410c" emissive="#7f1d1d" />
                  </mesh>
                )
              })}
            {started && (
              <LaserRaycaster
                mode={mode}
                structureRef={structureRef}
                tumorRefs={tumorRefs}
                hit={hit}
                onBurn={handleBurn}
                onStructureHit={handleStructureHit}
              />
            )}
          </Canvas>
        </div>
      </ErrorFlash>
    </GameFrame>
  )
}
