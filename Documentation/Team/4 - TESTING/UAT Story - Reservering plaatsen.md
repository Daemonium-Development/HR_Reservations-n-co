# User Acceptance Test Story — Debug Diner

**Versie:** 1.0  
**Datum:** Juni 2026  
**Gebaseerd op:** Codebase `HR_Reservations-n-co` — branch `main`

---

## Context

De UAT is gebaseerd op de werkelijke applicatieflow zoals geïmplementeerd in de codebase.
Relevante views: `RegisterView.cs`, `LoginView.cs`, `CreateReservationsView.cs`, `ReservationsView.cs`.

Tijdsloten worden gegenereerd van **17:00 t/m 22:00** in stappen van **15 minuten**.  
Een reservering duurt altijd **2 uur** (hardcoded in `CreateReservationsView.cs`).  
Een tafel wordt als bezet beschouwd als een bestaande reservering met status `Pending` of `Confirmed` overlapt met het gewenste tijdvenster.

---

## User Acceptance Test Story: Restaurantreservering plaatsen

### Beschrijving

Als klant van Debug Diner wil ik een account kunnen aanmaken, inloggen en een tafelreservering kunnen plaatsen voor een dineravond, zodat ik zeker ben van een plek in het restaurant.

---

### Scenario 1 — Succesvol registreren

**Startconditie:** De applicatie is gestart en het welkomstscherm is zichtbaar.

| Stap | Actie (tester) | Verwacht resultaat |
|---|---|---|
| 1 | Start de applicatie via `dotnet run --project DebugDiner` | Welkomstscherm verschijnt met navigatiemenu |
| 2 | Navigeer naar **Register** | Registratiescherm verschijnt met velden: Name, Email, Password |
| 3 | Vul een naam in (bijv. `Jan de Tester`) | Naam wordt zichtbaar in het invoerveld |
| 4 | Vul een geldig emailadres in (bijv. `jan@test.nl`) | Email wordt zichtbaar in het invoerveld |
| 5 | Vul een wachtwoord in (bijv. `Test1234!`) | Wachtwoord verschijnt gemaskeerd |
| 6 | Klik op **Register** | Succesbericht: doorverwijzing naar loginscherm |

**Acceptatiecriterium:** Account is aangemaakt; loginscherm wordt getoond.

---

### Scenario 2 — Registratie met ontbrekende velden

**Startconditie:** Registratiescherm is geopend.

| Stap | Actie (tester) | Verwacht resultaat |
|---|---|---|
| 1 | Laat het veld **Email** leeg | — |
| 2 | Vul naam en wachtwoord wel in | — |
| 3 | Klik op **Register** | Foutmelding: `"All fields are required."` |
| 4 | Vul alsnog het emailveld in en klik op **Register** | Succesvol geregistreerd |

**Acceptatiecriterium:** Applicatie laat de gebruiker niet registreren zonder alle velden in te vullen.

---

### Scenario 3 — Inloggen met geldige gegevens

**Startconditie:** Account bestaat (aangemaakt in Scenario 1 of geseed via DataSeeder).

| Stap | Actie (tester) | Verwacht resultaat |
|---|---|---|
| 1 | Navigeer naar **Login** | Loginscherm verschijnt met velden: Email, Password |
| 2 | Vul het emailadres in van het geregistreerde account | Email zichtbaar in invoerveld |
| 3 | Vul het bijbehorende wachtwoord in | Wachtwoord gemaskeerd zichtbaar |
| 4 | Klik op **Login** | Ingelogd; homescherm verschijnt met klantmenu |

**Acceptatiecriterium:** Gebruiker is ingelogd en het klantmenu is zichtbaar.

---

### Scenario 4 — Inloggen met ongeldig wachtwoord

**Startconditie:** Loginscherm is geopend.

| Stap | Actie (tester) | Verwacht resultaat |
|---|---|---|
| 1 | Vul een geldig emailadres in | — |
| 2 | Vul een **fout** wachtwoord in | — |
| 3 | Klik op **Login** | Foutmelding: `"Invalid email or password."` |
| 4 | Gebruiker blijft op loginscherm | Geen navigatie naar homescherm |

**Acceptatiecriterium:** Applicatie weigert inloggen met verkeerde credentials.

---

### Scenario 5 — Reservering plaatsen (happy path)

**Startconditie:** Gebruiker is ingelogd als klant.

| Stap | Actie (tester) | Verwacht resultaat |
|---|---|---|
| 1 | Navigeer naar **Make Reservation** | Reserveringsscherm opent met: datumveld, tijdsloten, guestsveld, tafelsoverzicht |
| 2 | Selecteer een datum in de toekomst | Datumveld toont gekozen datum; tafelbeschikbaarheid wordt herberekend |
| 3 | Selecteer een tijdslot (bijv. `17:00`) | Tijdslot gemarkeerd; tafels met ✅ AVAILABLE zichtbaar |
| 4 | Vul het aantal gasten in (bijv. `2`) | Getal verschijnt in invoerveld |
| 5 | Selecteer een tafel met status `✅ AVAILABLE` | Tafel gemarkeerd in de lijst |
| 6 | Klik op **Make Reservation** | Succesbericht: `"Reservation created. Ends at: 19:00"` |
| 7 | Navigeer naar **My Reservations** | Nieuwe reservering zichtbaar met status `Pending` |

**Acceptatiecriterium:** Reservering is aangemaakt en zichtbaar in het overzicht met de juiste eindtijd (starttijd + 2 uur).

---

### Scenario 6 — Reservering plaatsen op bezette tafel

**Startconditie:** Er bestaat al een reservering voor tafel 1 op 17:00.

| Stap | Actie (tester) | Verwacht resultaat |
|---|---|---|
| 1 | Navigeer naar **Make Reservation** | Reserveringsscherm opent |
| 2 | Selecteer dezelfde datum als de bestaande reservering | — |
| 3 | Selecteer tijdslot `17:00` | Tafel 1 toont `❌ TAKEN` in de tafelslijst |
| 4 | Selecteer tafel 1 (`❌ TAKEN`) en klik op **Make Reservation** | Foutdialoog: `"This table is already reserved for this time slot."` |
| 5 | Selecteer een ander tijdslot (bijv. `19:15`) | Tafel 1 toont nu `✅ AVAILABLE` (overlap is voorbij) |
| 6 | Klik opnieuw op **Make Reservation** | Reservering succesvol aangemaakt |

**Acceptatiecriterium:** Overlappende reserveringen worden geweigerd; niet-overlappende tijdsloten zijn wel beschikbaar voor dezelfde tafel.

---

### Scenario 7 — Reservering met ongeldig aantal gasten

**Startconditie:** Make Reservation scherm is geopend.

| Stap | Actie (tester) | Verwacht resultaat |
|---|---|---|
| 1 | Selecteer datum, tijdslot en tafel correct | — |
| 2 | Laat het veld **Guests** leeg (of vul `0` in) | — |
| 3 | Klik op **Make Reservation** | Geen reservering aangemaakt; geen foutmelding (veld wordt genegeerd) |
| 4 | Vul `2` in het Guests-veld en klik opnieuw | Reservering succesvol aangemaakt |

**Acceptatiecriterium:** Applicatie maakt geen reservering aan als het aantal gasten 0 of leeg is.

---

### Scenario 8 — Reservering bekijken en annuleren

**Startconditie:** Gebruiker heeft minstens één actieve reservering.

| Stap | Actie (tester) | Verwacht resultaat |
|---|---|---|
| 1 | Navigeer naar **My Reservations** | Lijst van eigen reserveringen zichtbaar |
| 2 | Selecteer een reservering met status `Pending` | Reservering geselecteerd |
| 3 | Kies optie **Delete / Annuleer** | Bevestigingsscherm of directe verwijdering |
| 4 | Bevestig de annulering | Reservering verdwijnt uit de actieve lijst |

**Acceptatiecriterium:** Gebruiker kan eigen reserveringen annuleren; geannuleerde reservering is niet meer zichtbaar als actief.

---

## Notities voor de tester

- Tijdsloten lopen van **17:00 t/m 22:00** in stappen van **15 minuten**
- Elke reservering duurt automatisch **2 uur**
- Tafelbeschikbaarheid wordt live herberekend bij het wisselen van datum of tijdslot
- Admin-gebruikers zien **alle** reserveringen; klanten zien alleen hun eigen reserveringen
- De applicatie draait als console-app via Terminal.GUI; navigatie gaat via het linker navigatiemenu
