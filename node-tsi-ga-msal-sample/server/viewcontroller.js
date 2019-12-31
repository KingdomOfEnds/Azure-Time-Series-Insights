'use strict'

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

const express = require('express'),
  view = express.Router()

view
  .get('/adal', (req, res) => res.render('adal.ejs'))
  .get('/msal', (req, res) => res.render('msal.ejs'))
  .get('/test', async (req, res) => await res.send({message: "hello world!"}))

console.log(`View controller initialized!`)

module.exports = view