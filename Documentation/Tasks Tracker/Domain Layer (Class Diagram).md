# Domain Layer (Class Diagram)

Status: Done
Assignee: Quindows, QuinDows
Due date: 03/23/2026
Priority: 10
Task type: Requirement to start
Effort level: Large
Projects: Debug Diner

## Task description

Provide an overview of the task and related details.

![diagram](Domain Layer (Class Diagram)/diagram_8.png)

Ik zie vooral `Entities.`  Waar zijn mijn Repository Interfaces?
Je hebt helemaal gelijk dat er geen logica zit in de Domain Layer, maar ik mis wel de interfaces die in de Infrastructure Layer implemented moeten worden. Dingen zoals `GetUser` `UpdateUser` of `GetReservationsForUser` dit soort dingen ga je in een Repository vinden, dus de interface moet dit definieren.
Nu ben ik zelf heel erg van `GetAsync<T>(params object[] args)` om het makkelijker te maken, maar dit gaat niet meer op met zo veel unions als we hier gaan maken.

## Sub-tasks

- [ ]
- [ ]
- [ ]

## Supporting files



