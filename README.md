# tournament-tracker

Next iteration of my yet-to-be-finished tournament tracker library, this time with Fable, which compiles the F# code to javascript.

The repo consists of three projects:

1. [tournament-lib](./tournament-lib/) is the core package for tournament tracking logic, written completely in F#
2. [tournament-tracker](./tournament-tracker/) is a [Fable](https://fable.io) project that compiles the core package into js and provides an easy-to-use javascript interface for clients
3. [tournament-web](./tournament-web/) is a web application that provides a simple UI for tracking tournaments by utilising the tournament-tracker Fable package (**very much WIP**)

## Requirements

- [dotnet SDK](https://www.microsoft.com/net/download/core) 5.0 or higher
- [node.js](https://nodejs.org)
- An F# editor like Visual Studio, Visual Studio Code with [Ionide](http://ionide.io/) or [JetBrains Rider](https://www.jetbrains.com/rider/)

## Testing

- Run `dotnet test` [here](./tournament-lib/test) to test the core library
- Run `npm test` [here](./tournament-tracker/) to test the Fable js bindings

## TODOs

### General

- Add Dockerfiles to projects and make the application(s) running in containers

### Core lib

- Add secondary score for Swiss pairings in case score is tied

### Fable JS bindings

- Add Typescript type definitions

### Web client

- Implement
