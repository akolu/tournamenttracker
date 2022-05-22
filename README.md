# Tournament Tracker

Next iteration of my yet-to-be-finished tournament tracker, this time with Fable, which compiles the F# code to javascript.

The repo consists of the following projects:

1. [Tournament.Core](./Tournament.Core/) is the core package for tournament tracking logic, written completely in F#.
2. [Tournament.Web](./Tournament.Web/) is a [Fable](https://fable.io) web application that provides a UI for the core logic, written completely in #F.

## Requirements

- [dotnet SDK](https://www.microsoft.com/net/download/core) 5.0 or higher
- [node.js](https://nodejs.org)
- An F# editor like Visual Studio, Visual Studio Code with [Ionide](http://ionide.io/) or [JetBrains Rider](https://www.jetbrains.com/rider/)

## Testing

- Run `dotnet test` [here](./Tournament.Core/test) to test the core library
- Run `npm test` [here](./Tournament.Web) to test the Fable js bindings

## TODOs

### General

- Add Dockerfiles to projects and make the application(s) running in containers

### Core lib

- feat: support player rating as a tiebreaker in Swiss pairings
- feat: support secondary round score as a tiebreaker in Swiss pairings
- refactor: return Result from PairingAlgorithm function for better error handling
- refactor: use List.allPairs for PairingGenerator pairing matrix
- refactor: use List.except or List.removeAt when popping last item from list (e.g. in PairingGenerator)

### Webapp

- feat: use localstorage to persist tournament (& add possility to reset)
- feat: swap players
- feat: add tooltips / snackbars & better error handling
- feat: add timer for rounds
- feat: prioritize game score in case of ties on results
- refactor: use CSS mixins to reduce code duplication e.g. in tables (hover)
- refactor: extract SASS variables to own file
- refactor: extract Round button to component to reduce code duplication
- chore: verify CSS class names follow BEM naming conventions
