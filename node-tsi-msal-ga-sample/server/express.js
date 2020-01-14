'use strict'

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

const express = require('express')
const app = express()
const bodyParser = require('body-parser')
const CONFIG = require('../cluster.config.js')
const path = require('path')

module.exports = {
  createServer: () => {

    app.set('views', path.join(__dirname, '..', CONFIG.PUBLIC_DIR))
      .set('view engine', 'ejs')
      .use(express.static(path.join(__dirname, '..', CONFIG.PUBLIC_DIR)))

      .use(require('morgan')('dev'))
      .use(bodyParser.json())
      .use(bodyParser.urlencoded({extended: true}))
      .use(require('cookie-parser')())

      .use('/', require('./viewcontroller'))

    const listener = app.listen(CONFIG.EXPRESS_PORT, err => {
      if (err) console.error(err)
      else console.log(`Express server listening on port ${listener.address().port}`)
    })

    return app
  }
}
