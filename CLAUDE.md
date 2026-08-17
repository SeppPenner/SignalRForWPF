# Project rules for Claude

## What this is

SignalRForWPF is a code example that shows how to talk to an [ASP.NET Core SignalR](https://learn.microsoft.com/aspnet/core/signalr/introduction)
hub from a [WPF](https://learn.microsoft.com/dotnet/desktop/wpf/) application. A WPF window sends a
message to a hub, the hub broadcasts it to every connected client, and each client appends what it
receives to a text box. The repository is **not** published as a NuGet package and has no installer:
no `GeneratePackageOnBuild`, no push script, no setup project. Consumers read the code or copy it.

One solution `src/SignalRForWPF.sln` with exactly three projects:

- `src/SignalRForWPF.Client/SignalRForWPF.Client.csproj`, `OutputType` `WinExe`, `UseWPF`, the WPF
  client. `MainWindow.xaml.cs` holds everything: the `HubConnection`, the two bound properties
  `SentText` and `ReceivedText`, the `INotifyPropertyChanged` implementation and the click handler.
  `App.xaml.cs` is an empty partial class, the startup window comes from `StartupUri` in `App.xaml`.
- `src/SignalRForWPF.Server/SignalRForWPF.Server.csproj`, SDK `Microsoft.NET.Sdk.Web`, the hub plus
  a Razor Pages sample site. `TestHub.cs` is the whole hub: one method `SendMessage` that writes to
  the console and calls `Clients.All.SendAsync("ReceiveMessage", message)`.
- `src/SignalRForWPF.Shared/SignalRForWPF.Shared.csproj`, `OutputType` `Library`, contains nothing
  but `Message.cs` with the two properties `User` and `MessageText`. Both other projects reference
  it, which is what keeps the payload type identical on both sides.

The server keeps the classic `Program` plus `Startup` split (`Host.CreateDefaultBuilder` and
`ConfigureWebHostDefaults`) instead of minimal hosting. That is the shape of the example, do not
convert it to top level statements.

`src/SignalRForWPF.Server/Pages` is the Razor Pages sample site (`Index`, `Privacy`, `Error`, the
shared layout and the cookie consent partial). `wwwroot/lib` holds checked in copies of bootstrap,
jQuery and jQuery Validation, there is no `libman.json` and no npm build step. Nothing in the site
talks to the hub, it only proves the server is a normal web application.

Repository root: `README.md` (the only user documentation), `Changelog.md`, `License.txt` (MIT),
`.gitignore`, `.gitattributes` and `src/.editorconfig`. There is no `Updating.md`, no `HowToUse.md`
and no screenshots.

## Build

```powershell
dotnet build src/SignalRForWPF.sln -c Release
```

- Single target framework in all three projects, no multi-targeting: `net10.0-windows` for the
  client, `net10.0` for server and shared.
- There are **no tests** in this repository. `dotnet test` finds no test project. Every claim about
  behaviour has to come from actually running server and client, see below.
- All build properties live directly in the three `.csproj` files and are duplicated there. There is
  **no** `Directory.Build.props`.
- `TreatWarningsAsErrors` is enabled everywhere, so every warning breaks the build, NuGet warnings
  (`NU****`) from restore included. A clean build reports zero warnings, keep it that way.
- `NU1803` (HTTP source usage during restore) is the one warning suppressed via `NoWarn`. Fix
  warnings instead of extending that list. `NuGetAudit` and `NuGetAuditMode=all` are on, so a
  vulnerable transitive package fails the build too.
- Versions come from GitVersion.MsBuild out of the git tags, for example `1.0.10-1` for the first
  commit after tag `1.0.9`. Never edit a version property or an assembly version by hand.
- Restore needs nuget.org. If a private feed is configured globally on the machine and cannot be
  reached, `NuGetAudit` turns that into `NU1900`, which `TreatWarningsAsErrors` turns into a build
  error. Then build with an explicit source:
  `dotnet build src/SignalRForWPF.sln -c Release --source https://api.nuget.org/v3/index.json`.
  The same applies to `dotnet list package --outdated`, which additionally needs `--no-restore`.

### Running it

Server and client are two processes and the example only works when both run:

1. Start the server. The client connects to `https://localhost:5001/testHub`, hard coded in
   `MainWindow.xaml.cs`, so the server has to listen there. The `SignalRTestServer` profile in
   `Properties/launchSettings.json` does exactly that, `dotnet run` uses it. Starting the published
   exe needs `ASPNETCORE_URLS=https://localhost:5001;http://localhost:5000`.
2. Start the client, type into the upper text box, press `Senden`. The lower text box fills with
   `<client id>: <text>`, the server console prints `Received message ... from client ...`.

The ASP.NET Core development certificate has to be trusted (`dotnet dev-certs https --trust`),
otherwise the client's connection attempt dies in the TLS handshake and, because of the fire and
forget start described below, the window shows nothing at all until the first click.

## Code conventions

Follow the surrounding code, it is consistent throughout every file:

- File header comment block with `<copyright file="..." company="Hämmer Electronics">` and a
  `<summary>`, then the file-scoped namespace.
- XML doc comments on every type and every member, private members included, no exceptions.
  Implementations of an interface member additionally carry `<inheritdoc cref="..."/>` and
  `<seealso cref="..."/>` pointing at that interface, see `MainWindow`.
- `Nullable`, `ImplicitUsings` and `LangVersion latest` are enabled.
- New `using` directives go into the `GlobalUsings.cs` of the respective project, inside the
  existing `#pragma warning disable IDE0065` block, never at the top of a file. The editorconfig
  requires usings inside the namespace (`csharp_using_directive_placement=inside_namespace:warning`),
  which global usings cannot satisfy, that is what the pragma is for. Do not add other pragmas. The
  comment text in that block is German because Visual Studio generated it, leave it alone.
  `SignalRForWPF.Shared/GlobalUsings.cs` contains the pragma pair and nothing else, that is on
  purpose.
- Fields, properties, methods and events are always accessed with `this.` qualification
  (`dotnet_style_qualification_for_*` at severity `warning`).
- `src/.editorconfig` also enforces braces everywhere, no multiple blank lines, four spaces, CRLF,
  UTF-8, file scoped namespaces, `System` usings sorted first and `IDE0005` as warning. Analyzer
  warnings are fixed, not silenced.
- The `.csproj` files are indented with four spaces and a blank line between the property and item
  groups, unlike the Visual Studio default.

## Known quirks

Do not silently "clean up" these, they are existing behaviour:

- **The client id is a hash of a random double.** `MainWindow` builds `clientId` from
  `SHA256.Create().ComputeHash(...)` over `new Random().NextDouble()`. It is a throwaway identifier
  for the example, not an identity, and the `SHA256` instance is deliberately never disposed. Do not
  turn this into user authentication.
- **The connection is started fire and forget.** The constructor calls `this.connection.StartAsync()`
  without awaiting it and without a continuation. If the server is down or its certificate is not
  trusted, the task faults unobserved, the window looks perfectly fine, and the failure only becomes
  visible when the first click raises
  `InvalidOperationException: The 'InvokeCoreAsync' method cannot be called if the connection is not
  active`. The `Closed` handler that reconnects after a random delay does not help here, `Closed` is
  only raised after a connection was established at least once.
- **The server URL is hard coded** as `https://localhost:5001/testHub` in `MainWindow.xaml.cs`. There
  is no configuration file on the client side, changing the address means changing that line.
- **The hub logs to the console, the framework does not.** `appsettings.json` sets the default log
  level to `Warning`, so a running server normally prints nothing but the `Console.WriteLine` in
  `TestHub.SendMessage`. Silence is not a sign that something is broken.
- **Razor Pages are registered, MVC is not.** `ConfigureServices` calls `AddRazorPages` and
  `AddSignalR`, `Configure` maps `MapRazorPages` and `MapHub<TestHub>("/testHub")` inside
  `UseEndpoints`, after `UseRouting`. Up to version 1.0.8.0 this was `AddMvc` with
  `EnableEndpointRouting = false` plus a trailing `UseMvc()`, which threw at startup because
  `UseEndpoints` requires `UseRouting`. Do not reintroduce the legacy routing mix.
- **`Program.Main` runs the host synchronously.** `Build().Run()`, not `Build().RunAsync()`. With
  `RunAsync` and no `await` (the state up to version 1.0.8.0) `Main` returned immediately, the
  process ended with exit code 0 and every startup exception was swallowed with it.
- **The client project ships a `None Update="License.txt"` item** for a file that does not exist in
  that project directory (the license sits in the repository root). The item is inert, it neither
  fails the build nor copies anything.
- **`RuntimeIdentifiers>win-x64` in the client project** without any `PublishSingleFile` or
  `SelfContained` property. It only makes the RID explicit, publishing is framework dependent.
- **The server has no SignalR package reference.** It does not need one, `Microsoft.NET.Sdk.Web`
  brings the server side of SignalR with the shared framework. Only the client references
  `Microsoft.AspNetCore.SignalR.Client`.
- **`.github/workflows` exists but is empty**, and git does not track empty directories, so a fresh
  clone does not even have the folder. The AppVeyor badge in `README.md` points at a build that is
  configured outside of this repository. There is no pipeline file here.
- **`src/SignalRForWPF.sln.DotSettings`** is tracked and holds nothing but a ReSharper user
  dictionary (`H_00E4mmer`). Leave it alone.
- **`.gitattributes` sets `* text=auto`** and every rule of the Visual Studio template below it is
  commented out. Any binary file that gets added needs its own rule.

## Releasing

1. Make the change.
2. Add an entry at the top of `Changelog.md` in the existing format:
   `* **Version 1.0.9.0 (2026-08-17)** : Short description.`
3. Commit that.
4. Tag the commit with the plain version number, no `v` prefix (`1.0.9`, `1.0.8`, ...). The existing
   tags are lightweight tags, create new ones the same way.
5. Push the commits and the tag.

The version in the `Changelog.md` has four parts (`1.0.9.0`), the tag has three (`1.0.9`).
GitVersion turns the tag into the assembly version, so an untagged commit produces something like
`1.0.9-1+Branch.master.Sha...`. There is no installer to build and no package to push, so the
release ends with the push.

## Git

- **Never amend a commit.** No `git commit --amend`, not for a typo in the message, not to add a
  forgotten file, not even when the commit is still local. Write a follow-up commit instead. The
  release versions come from tags on exact commits, an amended commit leaves its tag pointing at a
  commit that no longer exists in the branch.

## Writing style

- Commit messages are written **in English only**: short, precise subject line, explanatory body
  when needed.
- Code comments and comments in project files such as `.csproj` are **always English**, regardless
  of the language used in the conversation.
- **No em dashes or en dashes** (`—`, `–`), neither in prose, commit messages, code comments nor
  documentation. Use a regular hyphen, comma, colon, parentheses or a separate sentence.
- German texts (documentation, chat replies) always use real umlauts and ß, never ASCII
  transliterations such as `ae`, `oe`, `ue` or `ss`. Identifiers, file names and configuration keys
  stay unchanged where umlauts are technically undesirable.
