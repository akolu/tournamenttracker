# Tournament Tracker

![build](https://github.com/akolu/tournamenttracker/actions/workflows/test.yml/badge.svg)

Next iteration of my yet-to-be-finished tournament tracker, this time with Fable, which compiles the F# code to JavaScript.

The repo consists of the following projects:

1. [Tournament.Core](./Tournament.Core/) is the core package for tournament tracking logic, written completely in F#.
2. [Tournament.Web](./Tournament.Web/) is a [Fable](https://fable.io) web application that provides a UI for the core logic, written completely in #F. Webapp available [here](https://tournamenttracker-55dc5.web.app/).

## Requirements

- [dotnet SDK](https://www.microsoft.com/net/download/core) 5.0 or higher
- [node.js](https://nodejs.org)
- An F# editor like Visual Studio, Visual Studio Code with [Ionide](http://ionide.io/) or [JetBrains Rider](https://www.jetbrains.com/rider/)

## Testing

- Run `dotnet test` [here](./Tournament.Core/test) to test the core library
- Run `npm test` [here](./Tournament.Web) to test the Fable js bindings

## TODOs

### General

- Add Dockerfiles to projects and make the application run in a container

### Core lib

- refactor: return Result from PairingAlgorithm function for better error handling

### Webapp

- feat: swap players
- feat: add tooltips / snackbars & better error handling
- feat: alternatively support standard deviation as a tiebreaker instead of strength of schedule
- feat: alternatively support player rating as a tiebreaker instead of strength of schedule
- feat: add timer for rounds
- feat: prioritize game score in case of ties on results
- feat: add modals (using React portals) instead of native confirm dialogs
- refactor: use CSS mixins to reduce code duplication e.g. in tables (hover)
- refactor: extract SASS variables to own file
- refactor: extract Round button to component to reduce code duplication
- chore: verify CSS class names follow BEM naming conventions
- chore: webpack bundle size optimization
