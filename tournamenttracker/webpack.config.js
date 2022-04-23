// Note this only includes basic configuration for development mode.
// For a more comprehensive configuration check:
// https://github.com/fable-compiler/webpack-config-template

var path = require("path");

module.exports = {
    mode: 'development',
    entry: "./src/build/App.js",
    output: {
        path: path.join(__dirname, "./dist"),
        filename: "bundle.js",
    },
    devServer: {
        static: path.join(__dirname, 'public'),
        port: 8080,
    },
    module: {
        rules: [
            {
                test: /\.(sass|scss|css)$/,
                use: [ 'style-loader', 'css-loader', 'sass-loader' ] 
            },
        ]
    }
}