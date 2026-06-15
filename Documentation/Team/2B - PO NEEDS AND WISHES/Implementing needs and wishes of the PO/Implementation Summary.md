# Implementatie van PO-wensen

## Restaurantindeling geïmplementeerd

| PO-wens | Geïmplementeerd | Waar |
|---|---|---|
| 8× 2-persoons tafels | ✅ | DataSeeder, TableEntity, TableType.TwoPerson |
| 5× 4-persoons tafels | ✅ | DataSeeder, TableType.FourPerson |
| 2× 6-persoons tafels | ✅ | DataSeeder, TableType.SixPerson |
| Bar (8 stoelen) | ✅ | DataSeeder, TableType.Bar |

## Arrangementen geïmplementeerd

| PO-wens | Geïmplementeerd | Waar |
|---|---|---|
| 2-gangen diner | ✅ | ArrangementType.TwoCourse |
| 3-gangen diner | ✅ | ArrangementType.ThreeCourse |
| 4-gangen diner | ✅ | ArrangementType.FourCourse |
| Wijnarrangement (alleen bij dinner) | ✅ | ArrangementType.Wine |

## Applicatiefunctionaliteiten

| PO-wens | Geïmplementeerd | Waar |
|---|---|---|
| Login / registratie | ✅ | LoginView, RegisterView, UserRepository |
| Reservering plaatsen (datum, tijd, personen) | ✅ | CreateReservationsView |
| Reservering inzien / aanpassen / annuleren | ✅ | ReservationsView |
| Tafeloverzicht met beschikbaarheid | ✅ | TableRepository, ReservationsView |
| Arrangement kiezen bij reservering | ✅ | CreateReservationsView |
| Medewerker beheert reserveringen | ✅ | AdminUsersView, ManageBookingsView |
| Maandelijks menu aanpassen | ✅ | DishView, CreateDishView, UpdateDishView |
| Tijdsloten (ontbijt/lunch/diner) | ✅ | Timeslots branch (PR #95, Sprint 6) |
| Kleurrijke console UI | ✅ | Terminal.GUI, LayoutView |

## Openingstijden (PO interview antwoord)
- Ontbijt: 09:00-11:00
- Lunch: 11:00-13:00
- Diner: 17:00-21:00

Tafelreservering duurt **2 uur** (antwoord PO).
