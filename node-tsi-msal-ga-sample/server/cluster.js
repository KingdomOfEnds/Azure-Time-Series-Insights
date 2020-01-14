'use strict'

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

const cluster = require('cluster')
const CONFIG = require('../cluster.config')

module.exports = {
    createHttpCluster: () => {
        if (cluster.isMaster) {
            let cpuCount = require('os').cpus().length
            if (CONFIG.WORKERS !== null) cpuCount = CONFIG.WORKERS
            for (let i = 0; i < cpuCount; i++) {
                cluster.fork()
            }
            cluster
                .on('fork', worker => { console.log(`Worker %d created: ${worker.id}`) })
                .on('exit', (worker) => {
                    console.error(`Worker %d died: ${worker.id}`)
                    cluster.fork()
                })
        } else require('./http').createHttpServer(CONFIG.PORT)
    }
}