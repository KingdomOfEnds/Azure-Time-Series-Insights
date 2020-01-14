'use strict'

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

window.onload = () => {

    const config = {
        auth: {
            clientId: CLIENT_CONFIG.clientId,
            authority: 'https://login.microsoftonline.com/microsoft.onmicrosoft.com/',
            validateAuthority: true,
            redirectUri: CLIENT_CONFIG.redirectURI,
            postLogoutRedirectUri: CLIENT_CONFIG.redirectURI,
            navigateToLoginRequestUrl: false
        },
        cache: {
            cacheLocation: 'localStorage',
            storeAuthStateInCookie: false
        }
    }

    let localMsalContext = new Msal.UserAgentApplication(config)

    const authenticate = () => {
        return new Promise((resolve, reject) => {
            const request = {scopes: ["https://api.timeseries.azure.com//user_impersonation"]}
            localMsalContext.loginPopup(request)
                .then(response => {
                        const token = response.accessToken
                        console.log(token)
                        console.log(localMsalContext.getCachedToken())
                        localMsalContext.login()
                        return resolve(token)
                    }
                ).catch(err => {
                console.error(`Login error encounter: ${err.toString()}! Please check your AAD settings and try again...`)
                logout(localMsalContext)
            })
        })
    }

    const login = () => {
        authenticate().then(token => {
            getAggregatesStreamedApi(token)
        })
    }

    const logout = authContent => authContent.logOut()

    document.getElementById("login").addEventListener("click", e => {
        login()
    })
    document.getElementById("logout").addEventListener("click", e => {
        logout(localMsalContext)
    })

}