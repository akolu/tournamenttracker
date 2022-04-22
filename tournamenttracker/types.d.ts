declare module 'tournament-tracker' {
  export interface Tournament {
    players: string[]
    rounds: Round[]
  }

  export interface Round {
    number: number
    pairings: Pairing[]
    status: 'Pregame' | 'Ongoing' | 'Finished'
  }

  export interface Pairing {
    number: number
    player1: string
    player2: string
    player1Score: number
    player2Score: number
  }

  export enum PairingAlgorithm {
    Shuffle = 'Shuffle',
    Swiss = 'Swiss',
  }

  export function createTournament(rounds: number): Tournament

  export function addPlayers(players: string[], t: Tournament): Tournament

  export function pair(alg: PairingAlgorithm, t: Tournament): Tournament

  export function startRound(tablet: Tournament): Tournament

  export function finishRound(t: Tournament): Tournament

  export function score(table: number, p1Score: number, p2Score: number, t: Tournament): Tournament

  export function swap(player1: string, player2: string, t: Tournament): Tournament

  export function standings(t: Tournament): Tournament

  export function pairings(t: Tournament): Tournament
}
