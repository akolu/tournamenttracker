# tournament-tracker

Next iteration of my yet-to-be-finished tournament tracker library, this time with Fable, which compiles the F# code to javascript.

The repo consists of three projects:

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

- Add secondary score for Swiss pairings in case score is tied

### Webapp
