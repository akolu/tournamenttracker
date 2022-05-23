// Note this only includes basic configuration for development mode.
// For a more comprehensive configuration check:
// https://github.com/fable-compiler/webpack-config-template

const path = require("path");

module.exports = {
    mode: 'development',
    entry: "./src/build/App.js",
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
            {
                test: /\.woff2?$/i,
                type: 'asset/resource',
                dependency: { not: ['url'] },
            }, 
        ]
    }
}