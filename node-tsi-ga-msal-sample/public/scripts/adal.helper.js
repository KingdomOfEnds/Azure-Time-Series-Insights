'use strict'

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

const authContext = new AuthenticationContext({
    clientId: CLIENT_CONFIG.clientId,
    postLogoutRedirectUri: ''
})

if (authContext.isCallback(window.location.hash)) {
    // Handle redirect after token requests
    authContext.handleWindowCallback()
    var err = authContext.getLoginError()
    if (err) {
        // TODO: Handle errors signing in and getting tokens
        document.getElementById('api_response').textContent = 'ERROR:\n\n' + err;
    }
} else {
    const user = authContext.getCachedUser();
    if (user) document.getElementById('username').textContent = 'Signed in as: ' + user.userName
    else document.getElementById('username').textContent = 'Not signed in.'
}

authContext.getTsiToken = () => {
    document.getElementById('api_response').textContent = 'Getting tsi token...'

    // Get an access token to the Microsoft TSI API
    const promise = new Promise(function(resolve,reject){
        authContext.acquireToken(
            'https://api.timeseries.azure.com/',
            (error, token) => {

                if (error || !token) {
                    // TODO: Handle error obtaining access token
                    document.getElementById('api_response').textContent = 'ERROR:\n\n' + error
                    return
                }

                // Use the access token
                document.getElementById('api_response').textContent = ''
                resolve(token)
            }
        )
    })

    return promise
}