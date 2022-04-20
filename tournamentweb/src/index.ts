import { createTournament, addPlayers, pair } from 'tournament-tracker'

let tournament = createTournament(1)

console.log('tournament', tournament)

const tournament2 = addPlayers(['Ossi', 'Aku'], tournament)

console.log('tournament2', tournament2)

const tournament3 = addPlayers(['Juha', 'Veikka'], tournament2)

console.log('tournament3', tournament3)

const paired = pair('swiss', tournament3)

console.log('pairings', paired.rounds[0].pairings)
