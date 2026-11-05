# BIE Fehlerhilfe

- SDK fehlt: dotnet --list-sdks prüfen; .NET 10 installieren.
- Kein Projekt gefunden: ins Repo-Verzeichnis mit BIE.slnx wechseln.
- Tests fehlen: drei .cs.todo-Dateien mit scripts/Activate-Tasks.ps1 aktivieren. Assert.Fail ist anschliessend absichtlich rot.
- Integration: HttpStatusCode.Created erwarten, nicht OK. Fehlende Werte oder falsche Zweck-/Planetnamen führen zu 400.
- Abgelehnt und Quarantäne sind gültige gespeicherte Fälle. HTTP-Status und Fachentscheidung getrennt prüfen.
- E2E: Chrome installieren, Netzwerk für Selenium Manager zulassen. SHOW_BROWSER=1 zeigt den Browser; danach Umgebungsvariable entfernen.
- Warte auf sichtbares result und richtige Fallzeile statt Thread.Sleep. Ergebnis ist GENEHMIGT in Grossbuchstaben.
- DOM-IDs: name, planet, purpose, days, blackHole, fungus, shapeshifter, validIdentity, submit, result, application-list, sample-prev, sample-next.
- Fehlerdiagnose: Screenshot, App-Log und TRX unter TestResults bzw. den Actions-Artefakten ansehen.
- Drei Checks fehlen: Workflow im Repo-Hauptordner und einmal vollständig ausführen. Branchschutz gemäss GITHUB.md konfigurieren.
