# BIE Testautomatisierung

Unterrichtsapp «Büro für interplanetare Einreise» für M450. .NET 10 SDK, Git und Chrome erforderlich.

```powershell
dotnet restore BIE.slnx
dotnet run --project src/BIE.Web --urls http://localhost:5080
```

Browser: http://localhost:5080. Alternativ Starten.cmd oder BIE.slnx in Visual Studio öffnen.

## Aufbau

- src/BIE.Core: Entry, Verdict, Case und EntryPolicy, ohne HTTP oder Datenbank.
- src/BIE.Web: API, Registry mit SQLite und die BIE-Oberfläche mit sechs Musterakten.
- tests/BIE.UnitTests: Fachregeln und Grenzwerte.
- tests/BIE.IntegrationTests: Testserver, HTTP und echte isolierte SQLite-Datenbanken; zusätzlich Neustartprüfung.
- tests/BIE.E2ETests: Selenium mit eigenem App-Prozess, Browser, Port und Datenbestand je Test.
- docs/ARBEITSAUFTRAG.md: Unterrichtsauftrag. ANFORDERUNGEN.md ist das Testorakel.
- .github/workflows/tests.yml: getrennte Pflichtchecks unit-tests, integration-tests, e2e-tests.

```powershell
dotnet test BIE.slnx
dotnet test tests/BIE.UnitTests
dotnet test tests/BIE.IntegrationTests
dotnet test tests/BIE.E2ETests
```

Starter: 25 Grundfälle (13 Unit, 10 Integration, 2 E2E). Drei Learner*.cs.todo-Dateien aktivieren und vervollständigen. Danach 28 Fälle.

## Regression für den Unterricht

Nach Aktivierung und Ergänzung der drei Aufgaben:

```powershell
./scripts/Set-Regression.ps1 -Mode On
dotnet test BIE.slnx
./scripts/Set-Regression.ps1 -Mode Off
dotnet test BIE.slnx
```

On ändert nur die Tourismusregel von > 30 zu >= 30. Genau die drei neuen 30-Tage-Tests müssen rot werden. Die Korrektur stellt alle Tests wieder auf grün. Fachliche Erwartung nicht an den fehlerhaften Code anpassen.

Die App verwendet lokal src/BIE.Web/bie.db. Tests verwenden eigene temporäre Datenbanken. Nur erfundene Personendaten verwenden. Vor Unterrichtsbeginn einmal alle Pakete und den Chrome-Treiber laden; GitHub-Einrichtung gemäss docs/GITHUB.md.
