# Tournamenttracker Fable

Next iteration of my yet-to-be-finished Tournament tracker library, this time with Fable, which compiles the F# code to javascript. Very much WIP.

For more infor about Fable, [check this page](https://fable.io).

## Requirements

- [dotnet SDK](https://www.microsoft.com/net/download/core) 5.0 or higher
- [node.js](https://nodejs.org)
- An F# editor like Visual Studio, Visual Studio Code with [Ionide](http://ionide.io/) or [JetBrains Rider](https://www.jetbrains.com/rider/)

## Building and running the app

- Install dependencies: `npm install`
- Start the compiler in watch mode and a development server: `npm start`
- After the first compilation is finished, in your browser open: http://localhost:8080/

Any modification you do to the F# code will be reflected in the web page after saving.

> Note: check the "scripts" section in `package.json` to see the commands triggered by the steps above.

## Bundling for release

Run the following command to compile and bundle up all your F# code into one Javascript file: `npm run build`. The compiled output ends up in the `public` folder under the name `bundle.js`.

## Testing

Test project is not compiled to javascript (what's the point?). There is a convenient test shortcut script in `package.json` that simply wraps dotnet test command with some extra parameters, but it's faster to simply type `npm t`.
