# Debug Diner

Start date: 03/09/2026
End date: 06/01/2026
Progress: 83%
Priority: High
Team: HR Bishes
Assignee: gravia lycan, Ran, Quindows, Lars Werner (1127685)
Status: In progress

DISCORD LINK: [https://discord.gg/a9SF6YpPRZ](https://discord.gg/a9SF6YpPRZ)
Join the discord link!

[https://github.com/graviaDaemon/HR_Reservations-n-co.git](https://github.com/graviaDaemon/HR_Reservations-n-co.git)

[https://github.com/Daemonium-Development/HR_Documentations](https://github.com/Daemonium-Development/HR_Documentations)

[Scrum Master Planning](Debug Diner/Scrum Master Planning.md)

### RESTAURANT RESERVERINGSAPPLICATIE — SCRUM PROJECTBESCHRIJVING
Project B | C# · .NET 10.0 · SQLite | Rotterdam, 2026

## 1. PROJECTINFORMATIE

| Applicatietype  | Console Application (C# / .NET10.0) |
| --- | --- |
| Database | SQLite |
| Projectmethode | Scrum |
| Versiebeheer | Github |
| Interview met PO | 16 maart 2026 |

AANLEIDING
Jake Darcy opent een duurzaam restaurant in Rotterdam gericht op
een breed publiek (studenten, gezinnen, gepensioneerden).
Hij heeft een console-reserveringsapplicatie nodig.
Gebruiksvriendelijkheid staat centraal.

Restaurant:

- 8 tafels (2p) · 5 tafels (4p) · 2 tafels (6p) · bar (8 stoelen)
- Arrangementen: 2-, 3- of 4-gangen + optioneel wijnpakket
- Menu wisselt maandelijks (vlees, vis, vegetarisch, veganistisch)

# 2. APPLICATIE FUNCTIONALITEITEN

- Registratie & login (klant + medewerker)
- Tafel reserveren op datum, tijdstip en aantal personen
- Arrangement kiezen met prijsoverzicht
- Beschikbare tafels tonen (met paginering)
- Reserveringen inzien, aanpassen en annuleren
- Beheermodule voor medewerkers
-

# 3. TECHNISCHE METHODIEKEN

Stack:

- C# / .NET 10.0
- SQLite (CRUD)
- xUnit + FluentAssertions (unit testing)
- GitHub (versiebeheer)
- Scrum
- slnX type solution

Architectuur (three-layer model):

- Presentatielaag → Console UI, pijltjesnavigatie, paginering
- Business Logic → Reserveringsregels, validatie, prijslogica
- Data Access → SQLite CRUD-operaties

ERD-entiteiten:

- Klant (id, naam, e-mail, wachtwoord, rol)
- Reservering (id, datum, tijdstip, personen, status)
- Tafel (id, type, capaciteit)
- Arrangement (id, naam, gangen, prijs, wijn)

Unit tests (minimaal):

- Beschikbaarheidcontrole tafels
- Prijsberekening arrangementen
- Validatie invoer (datum, personen, e-mail)
- Reserveringsflow (aanmaken, wijzigen, annuleren)

# 5. SCRUM WERKWIJZE

Ceremonies:

- Daily Standup → wat gedaan / wat gepland / blokkades?
- Sprint Planning → stories inschatten en verdelen
- Sprint Review → demo aan PO
- Retrospective → wat ging goed / wat kan beter?

Verplichtingen:

- Urenregistratie bijhouden per teamlid
- Interview met PO op 16 maart 2026
- Workshops: Interviewing, Presenting, Git, ERD, Testing,
Three-layer model

# 6. DEFINITION OF DONE

Een story is klaar als:

- [ ]  Code werkt in C# (.NET 10.0)
- [ ]  Unit tests geschreven en geslaagd (xUnit + FluentAssertions)
- [ ]  Code gereviewed via pull request op GitHub
- [ ]  Alle acceptatiecriteria gehaald
- [ ]  Geen bekende bugs
- [ ]  Urenregistratie bijgewerkt

========================================================

**Workshops:**

- Interviewing (Done)
- Presenting
- Git
- ERD (Entity Relation Diagram)
- Three layer model
- Testing (Unit Testing, System Testing, and User Acceptance Test)

### Action items

Documenten die verplicht zijn:

- PO interview opbouwen (al mee bezig)
    - max half uur om vragen te stellen

[Samenwerkingsovereenkomst (Pre Signatures)](Debug Diner/Samenwerkingsovereenkomst (Pre Signatures).md)%2031e3f8f3faef80efb54ed5d3c10a44f8.md)

[HR_Samenwerkingsovereenkomst-Signed.pdf](Debug%20Diner/HR_Samenwerkingsovereenkomst-Signed.pdf)

[PO Interview](Debug%20Diner/PO%20Interview.md)

[Architecture](Debug%20Diner/Architecture.md)

[Interview antwoorden](Debug%20Diner/Interview%20antwoorden.md)

[Final Class Diagram](Debug%20Diner/Final%20Class%20Diagram.md)

