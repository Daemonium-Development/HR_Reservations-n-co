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
- Gesimuleerde betaling (geen echte transactie)

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