# BIE Fachliche Anforderungen

Das Testorakel für Unit-, Integrations- und E2E-Tests. Die Regeln sind deterministisch.

- R1: Name nach Trim 1–60 Zeichen; Planet Schleimora, Orion oder Kepler-186f; Zweck Tourismus, Diplomatie oder Arbeit.
- R2: Aufenthalt als ganze Zahl von 1 bis 365 Tagen einschliesslich. Ungültige Eingaben ergeben HTTP 400, ohne Speicherung.
- R3: Tourismus über 30 Tage wird abgelehnt; genau 30 Tage sind erlaubt, sofern keine andere Ablehnungsregel oder Quarantäne greift. Für Arbeit und Diplomatie gilt nur R2.
- R4: Mitgeführtes schwarzes Loch führt zur Ablehnung.
- R5: Formwandler ohne gültigen Identitätsnachweis werden abgelehnt. Ohne Formwandlerstatus löst ein fehlender Nachweis allein keine Ablehnung aus.
- R6: Ansteckender Weltraumpilz führt zur Quarantäne, sofern keine Ablehnung greift. Priorität: schwarzes Loch, Identität, Tourismusdauer, Pilz, Genehmigung.
- R7: Jeder gültige Fall wird gespeichert, auch Ablehnung und Quarantäne. POST /api/cases liefert HTTP 201, Fall-ID, Applicant und Decision sowie Location /api/cases/{id}.
- R8: GET /api/cases liefert alle gespeicherten Fälle, neueste zuerst; GET /api/cases/{id} liefert den Fall oder HTTP 404. SQLite-Daten bleiben nach App-Neustart erhalten.
- R9: Die Oberfläche zeigt GENEHMIGT, ABGELEHNT oder QUARANTÄNE. Nach Neuladen bleibt der gespeicherte Fall im Register sichtbar.
- R10: Sechs Musterakten sind vorwärts und rückwärts im Umlauf durchblätterbar. Genau eine Karte und ihr Porträt mit Scanstreifen sind sichtbar. «Akte übernehmen» setzt alle Formularwerte inklusive vier Sicherheitsangaben und entfernt ein altes Prüfergebnis. Blättern allein verändert das Formular nicht.

## Musterakten und erwartete Entscheidungen

| Person | Planet | Zweck | Tage | Besonderheit | Entscheidung |
| --- | --- | --- | --- | --- | --- |
| Zorg Blubber III | Schleimora | Tourismus | 5 | keine | approved |
| Nox Taschenuniversum | Orion | Diplomatie | 3 | schwarzes Loch | rejected |
| Spora Flausch | Kepler-186f | Tourismus | 12 | Pilz | quarantine |
| Lexa Vielgesicht | Orion | Tourismus | 30 | Formwandler, gültige Identität | approved |
| Kira Lichtleiter | Kepler-186f | Arbeit | 90 | keine | approved |
| Borin Schaltkreis | Orion | Arbeit | 180 | keine | approved |

Alle Musterakten haben einen gültigen Nachweis. Nicht genannte Sicherheitsmerkmale sind false.
Ungültig (HTTP 400) und fachlich abgelehnt (HTTP 201, Status rejected) sind verschieden.
