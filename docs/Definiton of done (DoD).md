# Definition of Done (DoD)

Deze Definition of Done geldt voor zowel Product Backlog (PB) als Sprint Backlog taken (SB).  
De Definition of Done kan in latere sprints worden uitgebreid of aangepast indien nodig.

---

## 1. Sprint Backlog Taken  
Een Sprint Backlog taak wordt als **Done** beschouwd wanneer:

### Development & Testing
- De functionaliteit voldoet aan alle acceptatiecriteria van Sprint 1 (US1, US2, US3):
  - Het menu toont alle beschikbare hoofdpagina’s.
  - Menu-opties hebben duidelijke, beschrijvende namen.
  - Het menu is visueel herkenbaar als navigatie-element.
  - Het menu is vanaf elke pagina bereikbaar.
  - De gebruiker kan binnen maximaal 2 acties naar een andere pagina navigeren.
  - Het menu toont op welke pagina de gebruiker zich bevindt.
  - Een klik op een menu-optie opent automatisch de gekoppelde pagina.
  - Elke menu-optie is gekoppeld aan één unieke pagina.
  - De geopende pagina toont de juiste bijbehorende inhoud.
  - Indien een pagina niet kan worden geladen, wordt een foutmelding getoond.
- De applicatie start zonder fouten of warnings.
- De navigatie en paginaweergave zijn handmatig getest en functioneren zoals bedoeld.
- Eventuele gevonden fouten zijn opgelost.

### Documentatie
- De user stories van Sprint 1 (US1, US2, US3) zijn bijgewerkt en als afgerond gemarkeerd.
- De testcases voor Sprint 1 zijn volledig ingevuld.
- Het ontwerp/mock-up van dashboard en menu is bijgewerkt naar de gerealiseerde functionaliteit.
- Documentatie is consistent vormgegeven en bevat versiebeheer, distributie en een inhoudsopgave.

### Repository & Deployment
- De code is gecommit en gepusht naar de juiste branch in GitHub.
- De feature-branch is succesvol gemerged in de sprint-branch zonder conflicten.
- De applicatie draait correct op de actuele sprintbranch.
- De taakstatus is bijgewerkt in de Sprint Backlog.

---

## 2. Product Backlog Items (PB)
Een Product Backlog item (user story) wordt als **Done** beschouwd wanneer:

- Alle gekoppelde Sprint Backlog taken (SB’s) de status **Done** hebben.
- Alle acceptatiecriteria van de user story volledig zijn gerealiseerd.
- De functionaliteit werkt end-to-end zoals beschreven.
- De Product Owner de functionaliteit heeft beoordeeld en goedgekeurd.
- Het team akkoord is dat de user story volledig voldoet aan de Definition of Done.
- Documentatie, ontwerp en backlogs zijn bijgewerkt.

---

Wanneer zowel de Sprint Backlog taken als het Product Backlog item voldoen aan bovenstaande criteria, wordt het item officieel beschouwd als **Done**.
