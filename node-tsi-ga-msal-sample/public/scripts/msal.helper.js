'use strict'

// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT license.

const pageUrl = window.location.href;

const authenticate = () => {
    var endpoints = {};
    //endpoints[getTwinsInstanceRoot()] = "0b07f429-9f4b-4714-9392-cc5e8e80c8b0";

    var config = {
        auth: {
            clientId: CLIENT_CONFIG.clientId,
            authority: 'https://login.microsoftonline.com/microsoft.onmicrosoft.com/',
            validateAuthority: true,
            redirectUri: pageUrl,
            postLogoutRedirectUri: pageUrl,
            navigateToLoginRequestUrl: false
        },
        cache: {
            cacheLocation: 'localStorage',
            storeAuthStateInCookie: false
        }
    }

    console.log("Client: " + config.auth.clientId)

    const msalContext = new Msal.UserAgentApplication(config)
    const request = {
        scopes: ["0b07f429-9f4b-4714-9392-cc5e8e80c8b0/Read.Write"]
        //"0b07f429-9f4b-4714-9392-cc5e8e80c8b0/Read.Write"
        //"api://2dca3773-3d87-465a-9dc1-9e9fc44bf7e6/test"
    }

    msalContext.acquireTokenSilent(request).then(response => {
            const token = response.accessToken
            console.log(token)
            console.log(msalContext.getCachedToken())
        }
    ).catch(error => {
        console.error(`Login error encounter: ${error.toString()}! Please check your AAD settings and try again...`)
        logout()
    })

    return msalContext
}

const login = () => {
    saveStateToStorage(inputUrl, inputTenantId, inputClientId)
    const localAuthContext = authenticate()
    localAuthContext.login()
}

const logout = authContext => authContext.logOut()