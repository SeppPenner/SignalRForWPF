SignalRForWPF
====================================

SignalRForWPF is an example on how to work with [ASP.Net Core SignalR](https://docs.microsoft.com/de-de/aspnet/core/signalr/introduction?view=aspnetcore-2.2)
in [WPF](https://docs.microsoft.com/en-us/dotnet/framework/wpf/getting-started/introduction-to-wpf-in-vs) aplications.
The assembly was written and tested in .Net 4.8, ASP.NetCore 2.2 and NetStandard 2.0.

[![Build status](https://ci.appveyor.com/api/projects/status/09knyo1qkj6h1s09?svg=true)](https://ci.appveyor.com/project/SeppPenner/signalrforwpf)
[![GitHub issues](https://img.shields.io/github/issues/SeppPenner/SignalRForWPF.svg)](https://github.com/SeppPenner/SignalRForWPF/issues)
[![GitHub forks](https://img.shields.io/github/forks/SeppPenner/SignalRForWPF.svg)](https://github.com/SeppPenner/SignalRForWPF/network)
[![GitHub stars](https://img.shields.io/github/stars/SeppPenner/SignalRForWPF.svg)](https://github.com/SeppPenner/SignalRForWPF/stargazers)
[![GitHub license](https://img.shields.io/badge/license-AGPL-blue.svg)](https://raw.githubusercontent.com/SeppPenner/SignalRForWPF/master/License.txt)
[![Known Vulnerabilities](https://snyk.io/test/github/SeppPenner/SignalRForWPF/badge.svg)](https://snyk.io/test/github/SeppPenner/SignalRForWPF)

## General stuff:
* The project `SignalRForWPF.Client` serves as a simple client application for sending messages via the SignalR service.
* The project `SignalRForWPF.Server` provides a SignalR service (and includes a sample website).
* The project `SignalRForWPF.Shared` contains the shared model classes of client and server.

## Additional information
This can be used in [WinForms](https://docs.microsoft.com/en-us/dotnet/framework/winforms/), too.

Change history
--------------

* **Version 1.0.0.1 (2019-05-05)** : Updated .Net version to 4.8.
* **Version 1.0.0.0 (2019-02-17)** : 1.0 release.
