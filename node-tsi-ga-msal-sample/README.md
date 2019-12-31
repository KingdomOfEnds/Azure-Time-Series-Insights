---
title: MSAL Hello World Single Page Web Application
description: 
---

# Introduction

This sample includes a simple SPA with MSAL for authentication. It uses a Node server to host the SPA.

# Setup

This article explains how to configure a custom application that calls the Azure Time Series Insights API.

> **Prerequisites**:
> 1. Node 8.8.0
> 2. An Azure Active directory account.
> 3. An Azure Time Series Insights account.

## Create Azure Active Directory Application

[Provision](https://docs.microsoft.com/azure/time-series-insights/time-series-insights-get-started) a General Availability Time Series Insights environment if you don't have one already.

Follow the [Authentication and authorization](https://docs.microsoft.com/azure/time-series-insights/time-series-insights-authentication-and-authorization#summary-and-best-practices) documentation to register your application in Azure Active Directory.

## Deploy and launch the Time Series Insights Node examples
 
1. In the root directory (where this README.md is located), enter the Bash command: `npm i` to download the required Node dependencies.
1. Verify that your Azure Active Directory settings are configured correctly in the EJS views.
1. Configure the Node cluster in `cluster.config.js`.
1. run the Bash command: `npm run start` to launch the Node cluster.
1. View [http://localhost:8888/adal](http://localhost:8888/adal) and [http://localhost:8888/msal](http://localhost:8888/msal).

## See also

* The [Azure Time Series Insights API reference](https://docs.microsoft.com/rest/api/time-series-insights/ga) documentation for all General Availability REST APIs.

* Follow the [Authentication and authorization](https://docs.microsoft.com/azure/time-series-insights/time-series-insights-authentication-and-authorization#summary-and-best-practices) documentation to register your application in Azure Active Directory.

* The [TSI JS client SDK](https://github.com/microsoft/tsiclient/blob/master/docs/API.md).
