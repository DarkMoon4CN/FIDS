let path = require('path');
let webpack = require('webpack');
module.exports = {
    entry: path.resolve(__dirname, './src/index.js'),
    output: {
        path: path.resolve(__dirname, 'dist'),
        filename: 'bundle.js',
        chunkFilename:'[name].js'
    },
    module: {
        loaders: [{
            test: /\.js$/,
            exclude: /^node_modules$/,
            use: [{
                loader: 'babel-loader',
                options: {
                    presets: [
                        ['es2015', {modules: false}]
                    ],
                    plugins: ['syntax-dynamic-import']
                }
            }]
        }]
    },


};