export type GameId =
  | 'line-precision'
  | 'reflex'
  | 'tumor-ablation'
  | 'suture'
  | 'tumor-spotting'
  | 'steady-hand'
  | 'instrument-sequence'

export interface GameResult {
  gameId: GameId
  perfection: number
  timeMs: number
  difficulty?: Difficulty
  routeAdherence?: number
  extra?: Record<string, number>
  at: string
}

export type Difficulty = 'easy' | 'medium' | 'hard'

export interface GameInfo {
  id: GameId
  title: string
  description: string
  path: string
}
