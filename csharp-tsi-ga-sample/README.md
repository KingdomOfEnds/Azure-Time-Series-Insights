---
title: Call the Query API from Azure Time Series Insights GA environments using C# and MSAL.NET
description: This sample covers how to call the Query API from Azure Time Series Insights GA environments using C# and MSAL.NET
---

# Query the GA Reference Data Management API from Azure Time Series Insights using C# and MSAL.NET

This C# example demonstrates how to call the Query APIs from Azure Time Series Insights GA environments using [MSAL.NET](https://github.com/AzureAD/microsoft-authentication-library-for-dotnet) as described in the [Query data from the Azure Time Series Insights GA environment using C#](https://docs.microsoft.com/azure/time-series-insights/time-series-insights-query-data-csharp) article.

## Recommended development environment

1. [Visual Studio 2019](https://visualstudio.microsoft.com/vs/) - Version 16.4.2+
1. [NuGet](https://www.nuget.org/)
1. [MSAL.NET](https://www.nuget.org/packages/Microsoft.Identity.Client/) - Version 4.7.1

## Setup and configuration

1. Configure your NuGet package path.
1. Follow steps in [Authentication and authorization](https://docs.microsoft.com/azure/time-series-insights/time-series-insights-authentication-and-authorization) to create an application in your tenant. Record the **Application ID**, **Redirect URIs**, and Time Series Insights environment information. Use `http://localhost:8080/` as the **Redirect URI**.
1. Replace all **#PLACEHOLDER#** values with the appropriate information from the preceding step in [Program.cs](./Program.cs).
1. Run the sample from within Visual Studio 2019.

## Description

This example demonstrates a few important Azure Active Directory and Azure Time Series Insights features:

1. Acquiring an access token using MSAL.NET **ConfidentialClientApplication**.
1. Sequential API operations against the [GA Reference Data Management API](https://docs.microsoft.com/rest/api/time-series-insights/ga-query).

Read [Create a reference data set for your Time Series Insights environment using the Azure portal](https://docs.microsoft.com/azure/time-series-insights/time-series-insights-query-data-csharp) to learn more.

## See also

* The [Azure Time Series Insights API reference](https://docs.microsoft.com/rest/api/time-series-insights/ga) documentation for all General Availability REST APIs.

* Follow the [Authentication and authorization](https://docs.microsoft.com/azure/time-series-insights/time-series-insights-authentication-and-authorization#summary-and-best-practices) documentation to register your application in Azure Active Directory.

* The [TSI JS client SDK](https://github.com/microsoft/tsiclient/blob/master/docs/API.md).
