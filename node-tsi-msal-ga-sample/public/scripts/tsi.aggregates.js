'use strict'

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

const getAggregatesStreamedApi = accessToken => {
    return new Promise((resolve, reject) => {

        const webSocket = new WebSocket("wss://10000000-0000-0000-0000-100000000108.env.timeseries.azure.com/aggregates?api-version=2016-12-12")

        webSocket.onmessage = e => {
            const message = JSON.parse(e.data ? e.data : false)
            if (message.content && message.content.length)  {
                document.getElementById('sampleAggregates').innerHTML = JSON.stringify(message.content[0])
                return(resolve(message.content[0]))
            }
        }

        webSocket.onopen = () =>
        {
            let messageObject = {}
            messageObject['headers'] = {'Authorization': 'Bearer ' + accessToken};
            const contentObject = {
                predicate: {predicateString: ''},
                searchSpan: {from: "2017-04-30T23:00:00.000Z", to: "2017-05-01T00:00:00.000Z"},
                aggregates: [
                    {
                        dimension: {dateHistogram: {input: {builtInProperty: "$ts"}, breaks: {size: "1m"}}},
                        measures: [{count: {}}]
                    }
                ]
            }
            messageObject['content'] = contentObject
            console.log(`Sending message: ${JSON.stringify(contentObject)} ...`)
            webSocket.send(JSON.stringify(messageObject))
        }
    })
}