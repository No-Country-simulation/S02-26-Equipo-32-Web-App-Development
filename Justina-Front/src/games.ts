import type { GameInfo } from './types'

export const GAMES: GameInfo[] = [
  { id: 'line-precision', title: 'Precisión en línea', description: 'Sigue la ruta ideal con el cursor. Se mide perfección y tiempo.', path: '/line-precision' },
  { id: 'reflex', title: 'Reflejos - Flujo sanguíneo', description: 'Toca los puntos de sangrado lo más rápido posible.', path: '/reflex' },
  { id: 'tumor-ablation', title: 'Ablación 3D de tumores', description: 'Elimina las esferas (tumores) en el campo 3D.', path: '/tumor-ablation' },
  { id: 'suture', title: 'Colocación de suturas', description: 'Coloca puntos en los objetivos en el orden correcto.', path: '/suture' },
  { id: 'tumor-spotting', title: 'Detección de tumores', description: 'Identifica tumores en la imagen lo más rápido posible.', path: '/tumor-spotting' },
  { id: 'steady-hand', title: 'Mano estable', description: 'Mantén el cursor dentro del área sin temblor.', path: '/steady-hand' },
  { id: 'instrument-sequence', title: 'Secuencia de instrumentos', description: 'Memoriza y repite el orden de instrumentos.', path: '/instrument-sequence' },
]
