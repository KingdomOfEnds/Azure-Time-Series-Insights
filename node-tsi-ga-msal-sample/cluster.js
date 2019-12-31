'use strict'

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

const cluster = require('./server/cluster')

cluster.createHttpCluster()