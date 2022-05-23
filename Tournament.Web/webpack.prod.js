// Note this only includes basic configuration for development mode.
// For a more comprehensive configuration check:
// https://github.com/fable-compiler/webpack-config-template

const path = require("path");
const MiniCssExtractPlugin = require("mini-css-extract-plugin");

module.exports = {
    mode: 'production',
    entry: "./src/build/App.js",
    output: {
        path: path.join(__dirname, "./dist"),
        filename: "main.js",
    },
    module: {
        rules: [
            {
                test: /\.(sass|scss|css)$/,
                use: [ MiniCssExtractPlugin.loader, 'css-loader', 'sass-loader' ] 
            },
            {
                test: /\.woff2?$/i,
                type: 'asset/resource',
                dependency: { not: ['url'] },
            }, 
        ]
    },
    plugins: [ new MiniCssExtractPlugin() ]
}