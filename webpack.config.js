// webpack.config.js
const path = require('path');

module.exports = {
  entry: './wwwroot/css/tailwind.css', // Entry file for your Tailwind CSS
  output: {
    path: path.resolve(__dirname, 'wwwroot/css'), // Output path for bundled CSS
    filename: 'bundle.css' // Output CSS filename
  },
  module: {
    rules: [
      {
        test: /\.css$/,
        use: ['style-loader', 'css-loader', 'postcss-loader'] // Process CSS with PostCSS
      }
    ]
  },
  mode: 'development' // Use 'production' for production builds
};