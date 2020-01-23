'use strict'

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

window.onload = () => {

    const config = {
        auth: {
            clientId: CLIENT_CONFIG.clientId,
            authority: 'https://login.microsoftonline.com/microsoft.onmicrosoft.com/oauth2/authorize?resource=https://api.timeseries.azure.com/',
            validateAuthority: true,
            redirectUri: CLIENT_CONFIG.redirectURI,
            postLogoutRedirectUri: CLIENT_CONFIG.redirectURI,
            navigateToLoginRequestUrl: false
        }
    }

    // Can replace with MSAL cache module
    let inMemoryEncapsulatedStorage = {
        "rawToken": ""
    }

    let localMsalContext = new Msal.UserAgentApplication(config)

    const authenticate = () => {
        return new Promise((resolve, reject) => {
            const request = {scopes: ["https://api.timeseries.azure.com//user_impersonation"]}
            localMsalContext.loginPopup(request)
                .then(response => {
                        const token = response.idToken.rawIdToken
                        console.log(`Token acquired: ${token} ...`)
                        inMemoryEncapsulatedStorage["rawToken"] = token
                        console.log(`Token saved to in-memory cache: ${JSON.stringify(inMemoryEncapsulatedStorage)} ...`)
                        return resolve(token)
                    }
                ).catch(err => {
                    console.error(`Login error encounter: ${err.toString()}! Please check your AAD settings and try again...`)
                    logout()
                }
            )
        })
    }

    const login = () => {
        console.log("Logging in ...")
        authenticate().then(token => {
            getAggregatesStreamedApi(token).then(success => {
                console.log(`Message received: ${JSON.stringify(success)} ...`)
            })
        })
    }

    const logout = () => {
        console.log("Logging out ... erasing in-memory token cache...")
        inMemoryEncapsulatedStorage["rawToken"] = ""
    }

    document.getElementById("login").addEventListener("click", e => login())
    document.getElementById("logout").addEventListener("click", e => logout())
}