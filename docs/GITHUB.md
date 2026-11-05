# GitHub: Vom persönlichen Repo zum geschützten Merge
## 1. Gemeinsam einrichten im Unterricht von Minute 15 bis 25
Jede Person arbeitet im eigenen Repo. Die Reviewpartner erhalten gegenseitig Schreibrechte.
GitHub Free unterstützt Branchschutz in öffentlichen Repos. Für private persönliche Repos ist ein passender Tarif nötig, etwa Pro. Kein bestehendes privates Repo ungefragt veröffentlichen. Bei Bedarf ein separates öffentliches Unterrichtsrepo erstellen.

### Neues Repo
In GitHub ein leeres Repo anlegen, ohne zusätzliche README. Lokal den INHALT von Starter in den Repo-Hauptordner kopieren, einschliesslich .github und .editorconfig. BIE.slnx liegt direkt im Hauptordner. Dann:
```powershell
git init -b main
git add .
git commit -m "Starterprojekt für Testautomatisierung"
git remote add origin https://github.com/DEIN-NAME/DEIN-REPO.git
git push -u origin main
```
Platzhalter im Link ersetzen. In GitHub Settings > Collaborators die Partnerperson hinzufügen. Einladung annehmen.
Der erste Push startet den Grundlauf. Er darf im Hintergrund laufen. In Minute 115–125 unter Actions alle drei Jobs kontrollieren, bevor die Pflichtchecks ausgewählt werden.

### Bestehendes persönliches Repo
Vorher Änderungen sichern/committen. Vorhandene Dateien nicht überschreiben. Der mitgelieferte Workflow erwartet die App im Repo-Hauptordner. Bei Namenskonflikten ein separates persönliches Übungsrepo verwenden. Bestehendes master beibehalten und unten statt main einsetzen. Workflow unterstützt beide Namen.
Die Schutzregel gilt für einen Branch, nicht nur für einen Unterordner. Vorhandene Teamregeln nicht ersetzen.

## 2. Branchschutz gemeinsam einrichten (Minute 115–125)
Settings > Branches > Add classic branch protection rule.
Falls GitHub zunächst Rulesets anbietet, die Option für klassische Branch Protection wählen.
Branch name pattern: main (oder bestehendes master).
Aktivieren:
- Require a pull request before merging.
- Require approvals: 1.
- Dismiss stale pull request approvals when new commits are pushed.
- Require status checks to pass before merging.
- Require branches to be up to date before merging.
- Checks wählen: unit-tests, integration-tests, e2e-tests. Erwartete Quelle GitHub Actions, sofern auswählbar.
- Do not allow bypassing the above settings.
Deaktiviert lassen: Allow force pushes, Allow deletions.
Nicht aktivieren: Lock branch (würde auch gewünschte Änderungen blockieren).
Regel speichern. Auch der Eigentümer muss sich jetzt im normalen Ablauf daran halten.

Checks fehlen in der Auswahl? Zuerst den vorbereiteten Workflow ausführen. Nach Registrierung neu laden. Jobnamen unverändert lassen.
Der Repo-Eigentümer kann Regeln bearbeiten oder entfernen. Branchschutz ist eine technische Teamvereinbarung, keine unveränderliche Sperre gegen den Eigentümer.

## 3. Arbeitsbranch ab Minute 60 und erster PR ab Minute 125
Vor den Testaufgaben:
```powershell
git switch -c test/bie
```
Nach abgeschlossenen Aufgaben und lokal grünen Tests:
```powershell
git add .
git commit -m "Tests auf drei Ebenen ergänzen"
./scripts/Set-Regression.ps1 -Mode On
git add src/BIE.Core/EntryPolicy.cs
git commit -m "Demo: Grenzwert versehentlich geändert"
```
Optionaler direkter Pushversuch NACH Aktivierung des Schutzes:
```powershell
git push origin HEAD:main
```
Er muss mit Protected branch / GH006 / GH013 oder einer vergleichbaren Regelmeldung abgewiesen werden. Bei bestehendem master entsprechend HEAD:main. Wenn er erfolgreich ist: Regeln prüfen und Lehrperson informieren. Keine Force-Push-Versuche.
Jetzt den Arbeitsbranch pushen:
```powershell
git push -u origin test/bie
```
Auf GitHub einen Pull Request nach master/main öffnen (Compare & pull request).
Alle drei Testsuiten laufen auf dem PR. Den roten Lauf abwarten, bevor die Korrektur gepusht wird.
Den fehlgeschlagenen Test öffnen und die verletzte Anforderung R3 benennen.

## 4. Fehler korrigieren
```powershell
./scripts/Set-Regression.ps1 -Mode Off
dotnet test BIE.slnx
git add src/BIE.Core/EntryPolicy.cs
git commit -m "Grenzwert gemäss R3 korrigieren"
git push
```
Erwartung: Alle drei Checks erfolgreich. Ohne Zustimmung bleibt der Merge blockiert.
Neue Commits nach einer Zustimmung machen diese wieder ungültig.

## 5. Partnerreview und Merge bis Minute 140 bis Minute 140 bis Minute 140 bis Minute 140 bis Minute 140
Files changed öffnen. Mit docs/REVIEW.md prüfen. Mindestens einen fachlichen Befund oder eine begründete Rückmeldung notieren.
Review changes > Approve > Submit review. Ein normaler Kommentar zählt nicht als Zustimmung.
Die Person darf den eigenen PR nicht selbst freigeben. Der Reviewer braucht Schreibrechte.
Erst mit gültigem Review und drei grünen Pflichtchecks: Merge pull request.
Danach lokal:
```powershell
git switch main
git pull --ff-only
```
Bei bestehendem master entsprechend anpassen. PR-Link und roten/grünen Lauf im Arbeitsauftrag festhalten.

## Warum drei Pflichtchecks?
Der Workflow führt jede Testsuite als eigenen, nicht optionalen Job aus. Kein continue-on-error, keine Pfadfilter, keine bedingte Auslassung der Testschritte. Reports laufen mit if: always(), damit auch Fehlernachweise verfügbar sind.
GitHub behandelt übersprungene Jobs teilweise als erfolgreich. Deshalb überspringen wir keine Testsuite. Grün garantiert trotzdem weder vollständige Testabdeckung noch korrekte Assertions.

## Quellen
- https://docs.github.com/en/repositories/configuring-branches-and-merges-in-your-repository/managing-protected-branches/about-protected-branches
- https://docs.github.com/en/actions/automating-builds-and-tests/building-and-testing-net
- https://docs.github.com/en/pull-requests/reference/status-checks
Stand der Anleitung: 05.11.2026. UI-Bezeichnungen können leicht abweichen.
