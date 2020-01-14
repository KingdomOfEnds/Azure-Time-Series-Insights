---
title: Query the GA APIs from Azure Time Series Insights using Node and MSAL.JS.
description: This sample covers how to query the GA APIs from Azure Time Series Insights using Node and MSAL.JS.
---

# Query the GA Query API from Azure Time Series Insights using tsiclient, MSAL.js, and Node

This sample covers how to query the [GA Query APIs](https://docs.microsoft.com/rest/api/time-series-insights/ga-query-api) from Azure Time Series Insights using Node and MSAL.JS.

## Recommended development environment

1. [Node.js](https://nodejs.org/en/) - Version 12.14.1+
1. [MSAL.JS](https://github.com/AzureAD/microsoft-authentication-library-for-js) - Version 1.2.0+
1. [Visual Studio Code](https://code.visualstudio.com/Download) - Version 1.41+

## Setup and configuration

1. Execute the command: `npm i` in this root directory.
1. Follow steps in [Authentication and authorization](https://docs.microsoft.com/en-us/azure/time-series-insights/time-series-insights-authentication-and-authorization) to create an application in your tenant. Record the **Tenant ID** and **Application ID**.
1. Set all the constants defined at the beginning of the sample in [client.config.js](./public/scripts/client.config.js).
1. Verify application configuration settings in [cluster.config.js](./cluster.config.js).
1. Execute the command `npm run start`in this root directory.

Test that the server is running: http://localhost:8888/test

## Description

This example demonstrates a few important Azure Active Directory and Azure Time Series Insights features:

1. Acquring an access token using MSAL.JS.
1. MSAL authentication is available: http://localhost:8888/msal
1. Return data from the [Get Environment Aggregates Streamed API](https://docs.microsoft.com/rest/api/time-series-insights/ga-query-api#get-environment-aggregates-streamed-api).

> Add graphing and visualization using the [TSI JS client SDK](https://github.com/microsoft/tsiclient/blob/master/docs/API.md).

## See also

* The [Azure Time Series Insights API reference](https://docs.microsoft.com/rest/api/time-series-insights/ga) documentation for all General Availability REST APIs.

* Follow the [Authentication and authorization](https://docs.microsoft.com/azure/time-series-insights/time-series-insights-authentication-and-authorization#summary-and-best-practices) documentation to register your application in Azure Active Directory.

* The [TSI JS client SDK](https://github.com/microsoft/tsiclient/blob/master/docs/API.md).