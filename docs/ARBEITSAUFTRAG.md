# Gemeinsamer Start mit der BIE App
Drei Lektionen à 45 Minuten mit zwei Pausen à 5 Minuten. Gesamtdauer 145 Minuten.

Name: _____________________   Reviewpartner/in: _____________________

## Werkzeuge einrichten von Minute 5 bis 15

- .NET 10 SDK, Git und Chrome prüfen. Fehlende Werkzeuge jetzt installieren.
- GitHub anmelden oder Konto im Unterricht einrichten. Bei einer Sperre zunächst am Partnergerät mitarbeiten.

```
dotnet --list-sdks
git --version
```

## Repository übernehmen von Minute 15 bis 25

Ein leeres persönliches Repo erstellen. Den INHALT von Lernende/Starter inklusive .github kopieren. BIE.slnx liegt im Repo-Hauptordner. Bei einem vorhandenen Repo keine Dateien überschreiben; bei Konflikten separates Übungsrepo verwenden.

```
git init -b main
git add .
git commit -m "BIE Starter"
git remote add origin <URL-DEINES-REPOS>
git push -u origin main
```

URL-Platzhalter ersetzen. Partner/in unter Settings > Collaborators mit Schreibrechten einladen und die Einladung annehmen.

## App starten und Grundtests ausführen

```
dotnet restore BIE.slnx
dotnet run --project src/BIE.Web --urls http://localhost:5080
```

Browser: http://localhost:5080. App in Terminal 1 laufen lassen. In Terminal 2 ab Minute 35 dotnet test BIE.slnx starten. Erwartung: 25 Grundfälle grün. Der erste Selenium-Lauf kann den Chrome-Treiber nachladen.

# BIE verstehen und Tests einordnen
Minute 25–45 · App-Aufbau, Anforderungen und gemeinsame Ausgangsbasis

## Unser Auftrag

Das «Büro für interplanetare Einreise BIE» braucht verlässliche Entscheidungen. Du ergänzt automatisierte Tests auf drei Ebenen und schützt deinen Hauptbranch. Am Ende erhält dein Pull Request erst nach erfolgreichen Tests und einem Partnerreview die Freigabe.

Browser und JavaScript senden Entry-Daten an BIE.Web. Die API validiert Eingaben; EntryPolicy in BIE.Core entscheidet. Registry in BIE.Web speichert den Fall in SQLite. Nach dem Neuladen lädt GET das Register erneut.

## Was du heute nachweist

| Prüfung | Dein Beitrag |
| --- | --- |
| Unit | Tourismus bei genau 30 Tagen ergibt approved. |
| Integration | Der Antrag wird über die API gespeichert und korrekt ausgelesen. |
| E2E | Selenium bedient das Formular und prüft nach Neuladen die Liste. |
| Zusammenarbeit | Roter und grüner CI-Lauf, Partnerreview und Merge im eigenen PR. |

## Testebenen begründen

Ordne zu und begründe: (a) Eine Person liest den Code. (b) Eine Methode wird geprüft. (c) API und Datenbank arbeiten zusammen. (d) Ein Browser bedient die ganze App.

Antwort: __________________________________________________
Antwort: __________________________________________________

Testorakel: ANFORDERUNGEN.md. R3: Tourismus ist bis einschliesslich 30 Tage erlaubt, sofern keine andere Ablehnungsregel oder Quarantäne greift. Arbeit unterliegt nur der allgemeinen Grenze 1–365 Tage.

# Unit und Integrationstests
Arbeitsphase · Minute 60–75

## 1  Arbeitsbranch und Aufgaben

```
git switch -c test/bie
./scripts/Activate-Tasks.ps1
```

Alternativ bei gesperrten Skripten: Bei den drei Learner*.cs.todo-Dateien die Endung .todo entfernen. Erst jetzt führt MSTest diese Dateien aus. Die drei Assert.Fail-Platzhalter schlagen absichtlich fehl.

## 2  Unit Test ergänzen

Öffne tests/BIE.UnitTests/LearnerUnitTests.cs. Der Entry für Tourismus mit 30 Tagen ist vorbereitet. Rufe EntryPolicy.Evaluate(entry) auf und prüfe dessen Status gegen "approved". Ersetze den Platzhalter durch Act und Assert.

```
dotnet test tests/BIE.UnitTests
```

Begründe: Warum ist 30 aussagekräftiger als nur 10?

Antwort: __________________________________________________
Antwort: __________________________________________________

## 3  Integrationstest ergänzen

Öffne tests/BIE.IntegrationTests/LearnerIntegrationTests.cs. POST /api/cases und GET sind vorbereitet. Prüfe HTTP 201, Liste nicht null, genau einen Eintrag, Applicant gleich entry, ID grösser 0 und Decision.Status gleich "approved".

```
dotnet test tests/BIE.IntegrationTests
```

Hilfen: Assert.AreEqual(erwartet, tatsächlich), Assert.IsNotNull(records), Assert.IsTrue(...). Jeder Test erhält eine eigene SQLite-Datenbank.

## 4  Ergebnisse auswerten   75 bis 80 Minuten

Welche zusätzlichen Fehler kann der Integrationstest finden? Welche Teile des Benutzerablaufs prüft noch keiner der beiden Tests?

Antwort: __________________________________________________
Antwort: __________________________________________________

Zwischenziel: 14 Unit- und 11 Integrationsfälle grün. Der neue E2E-Test ist noch unvollständig. Pause: Minute 95–100.

# Selenium und die BIE App
Demo Minute 80–95 · eigene Umsetzung Minute 100–115

## 5  Den Selenium Test ergänzen

Datei: tests/BIE.E2ETests/LearnerE2ETests.cs. Browser, App, Datenbank und Eingaben sind vorbereitet. Ergänze die markierten Stellen.

- Klicke den Button mit der ID submit.
- Warte mit Wait.Until auf das sichtbare Element result.
- Prüfe, dass der sichtbare Entscheidungstext «GENEHMIGT» enthält.
- Nach Neuladen: Prüfe in cells[1] bis cells[4] Sam Browser, Orion, 30 Tage und GENEHMIGT.
- Ersetze Assert.Fail durch die tatsächlichen Prüfungen.

| DOM-Element | Kennung / erwarteter Wert |
| --- | --- |
| Name | id=name / Sam Browser |
| Auswahl | id=purpose / Tourismus |
| Tage | id=days / 30 |
| Absenden | id=submit |
| Ergebnis | id=result / GENEHMIGT |
| Gespeicherte Zeile | Sam Browser / Orion / 30 Tage / GENEHMIGT |

## Syntaxhilfe

```
Driver.FindElement(By.Id("submit")).Click();
Wait.Until(d => d.FindElement(By.Id("result")).Displayed);
StringAssert.Contains(element.Text, "GENEHMIGT");
Assert.AreEqual("Sam Browser", cells[1].Text);
```

```
dotnet test tests/BIE.E2ETests
```

Standardmässig läuft Chrome unsichtbar. Für eine sichtbare Demo in PowerShell: $env:SHOW_BROWSER = "1". Danach Remove-Item Env:SHOW_BROWSER.

## Kurz erklären

Weshalb prüfen wir den Eintrag nochmals nach Neuladen? Warum warten wir auf einen Zustand, statt Thread.Sleep zu verwenden?

Antwort: __________________________________________________
Antwort: __________________________________________________

Zwischenziel: 3 E2E-Fälle grün. Alle drei Aufgaben sind aktiv und enthalten aussagekräftige Assertions.

# Regression und geschützter Hauptbranch
Einrichtung Minute 115–125 · PR-Durchlauf Minute 125–140

## 6  Schutzregel einrichten

Settings > Branches > Add classic branch protection rule. Ziel main (bei vorhandenem master diesen Namen verwenden).

- Pull Request erforderlich; mindestens 1 zustimmendes Review.
- Neue Codeänderungen verwerfen vorherige Zustimmungen.
- Pflichtchecks: unit-tests, integration-tests und e2e-tests.
- Branch vor dem Merge auf aktuellen Stand bringen.
- Umgehen der Regeln auch für Administratoren deaktivieren. Force Push und Löschen nicht erlauben.

Checks fehlen? Den vorbereiteten Workflow zuerst ausführen. Eine komplette Anleitung steht in docs/GITHUB.md. GitHub Free: öffentliches Repo; private persönliche Repos brauchen einen geeigneten Tarif.

## 7  Regression sichtbar machen

```
git add .
git commit -m "Tests auf drei Ebenen ergänzen"
./scripts/Set-Regression.ps1 -Mode On
git add src/BIE.Core/EntryPolicy.cs
git commit -m "Demo: Grenzwert versehentlich geändert"
git push -u origin test/bie
```

Pull Request nach master/main öffnen. Den roten CI-Lauf vollständig abwarten. In der Mergebox nachsehen, welche Bedingungen fehlen. Optional: Vor dem Branch-Push git push origin HEAD:main versuchen (bei bestehendem master entsprechend anpassen). Bei aktivem Schutz muss die Änderung abgewiesen werden.

## 8  Fachregel korrigieren und erneut prüfen

```
./scripts/Set-Regression.ps1 -Mode Off
dotnet test BIE.slnx
git add src/BIE.Core/EntryPolicy.cs
git commit -m "Grenzwert gemäss R3 korrigieren"
git push
```

Alle drei Checks werden grün. Vor dem Review bleibt der Merge blockiert. Partner/in prüft den aktuellen Code und gibt mit Review changes > Approve frei. Erst danach mergen. Neue Änderungen verlangen ein erneutes Review.

# Partnerreview und Abschluss
Partnerreview während des PR-Durchlaufs · Lernkontrolle Minute 140–145

## Reviewcheckliste

- Unit: Prüft der Test wirklich die Grenze 30 gemäss R3?
- Integration: Prüft er Status und erneut ausgelesene Fachwerte?
- E2E: Prüft er sichtbare Entscheidung und Eintrag nach Neuladen?
- Sind alle drei Learner*.cs-Dateien aktiv und alle Platzhalter ersetzt?
- Bleiben Tests voneinander unabhängig und verwenden stabile Selektoren?
- Ist die Regression im Verlauf rot, die Korrektur anschliessend grün?

Befund wie beim statischen Testen: Fundstelle, Beobachtung, Bezug zur Anforderung und Verbesserungsvorschlag. Eine Freigabe braucht eine begründete Prüfung. Ein normaler Kommentar genügt nicht.

## Dein Reviewbefund oder deine begründete Rückmeldung

Antwort: __________________________________________________
Antwort: __________________________________________________
Antwort: __________________________________________________

## Abschlussfragen

1. Kann ein Unit-Test ein Regressionstest sein? Begründe.

Antwort: __________________________________________________
Antwort: __________________________________________________

2. Warum kann ein E2E-Test fehlschlagen, obwohl die Unit-Tests grün sind?

Antwort: __________________________________________________
Antwort: __________________________________________________

3. Alle Tests sind grün. Unter welchen Bedingungen bleibt der Merge trotzdem blockiert?

Antwort: __________________________________________________
Antwort: __________________________________________________

## Nachweise

Repo / PR: ______________________________________________________________
Roter Lauf: _______________________  Grüner Lauf: __________________________
Review von: _______________________  Merge: ______________________________

Zusatzauftrag: Prüfe 29/30/31 Tage, Kira mit Arbeit für 90 Tage, Borin mit Arbeit für 180 Tage oder den Vorrang von Ablehnung vor Quarantäne. Begründe die passende Teststufe.
