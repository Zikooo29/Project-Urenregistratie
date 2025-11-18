# Project-Urenregistratie 

##  Projectbeschrijving

**Project-Urenregistratie** is een desktop-applicatie, ontwikkeld in opdracht van detacheringsbureau **“Ons Werk”**, met als doel het **digitaliseren en stroomlijnen** van hun urenregistratieproces.

Momenteel vullen werknemers urenbonnen handmatig in, wat leidt tot een tijdrovende en foutgevoelige administratieve verwerking. Dit project vervangt dat handmatige proces door een gebruiksvriendelijke, digitale oplossing gebouwd met de cross-platformkracht van C# en .NET MAUI.

### Kernfunctionaliteit:
* **Werknemersportaal:** Eenvoudige registratie van gewerkte uren per opdrachtgever en project.
* **Digitale Goedkeuring:** Mogelijkheid voor opdrachtgevers om uren digitaal te accorderen (bijvoorbeeld via een in-app handtekening of goedkeuringsflow).
* **Administratieve Integratie:** Naadloze export of directe integratie van goedgekeurde uren naar het **boekhoudsysteem** (voor facturering) en het **verloningssysteem** (voor salarisverwerking).
* **Toekomstvisie:** De applicatie wordt modulair opgezet om toekomstige verkoop aan andere detacheringsbureaus (**"MijnUren"** product) te faciliteren.

---

## Gebruikte technologieën

Dit project wordt ontwikkeld als een moderne, cross-platform applicatie. De volgende technologieën en frameworks worden gebruikt:

| Categorie | Technologie | Versie | Toelichting |
| :--- | :--- | :--- | :--- |
| **Frontend/Cross-Platform** | **C#** en **.NET MAUI** | .NET 8 | Single codebase voor iOS, Android, Windows en macOS. |
| **Programmeertaal** | **C#** | Nieuwste standaard | De primaire taal voor zowel de applicatie-logica als de UI. |
| **Architectuur** | **MVVM** | | **M**odel-**V**iew-**V**iew**M**odel-patroon voor schone scheiding van concerns en betere testbaarheid. |
| **Gegevensopslag (Lokaal)** | **SQLite** | | Lichte, lokale database voor offline functionaliteit en caching. |
| **Gegevensopslag (Centraal)** | **Entity Framework Core** | | Object-Relational Mapper (ORM) voor interactie met de centrale database (bijv. SQL Server). |
| **Backend/API (Optioneel)** | **ASP.NET Core Web API** | .NET 8 | Voor het verwerken van centrale data, authenticatie en communicatie met externe systemen. |

---

## Installatie en Opstarten

### 1. Vereisten
Zorg ervoor dat de volgende tools zijn geïnstalleerd op je ontwikkelmachine:

* **Visual Studio 2022** (Aanbevolen: Community/Professional/Enterprise editie).
* **Visual Studio Workload:** `.NET Multi-platform App UI development` (MAUI).
* **.NET 8 SDK** (of de gespecificeerde versie in het projectbestand).

### 2. De Repository Klonen

* Klonen van de repository via HTTPS
* git clone Project-Urenregistratie
* cd Project-Urenregistratie

### Naamgeving (Naming Conventions)

We volgen de Microsoft .NET Naming Guidelines:

We volgen de **Microsoft .NET Naming Guidelines**:

| Element | Conventie | Voorbeeld | Toelichting |
| :--- | :--- | :--- | :--- |
| **Klassen, Methoden, Properties** | **PascalCase** | `public class UrenService { ... }` | Hoofdletter aan het begin van elk woord. |
| **Lokale Variabelen, Parameters** | **camelCase** | `string werknemerNaam;` | Kleine letter aan het begin. |
| **Fields (Private Instance)** | **camelCase** (met een underscore prefix) | `private readonly ILogger _logger;` | Duidelijk onderscheid van lokale variabelen. |
| **Interfaces** | **PascalCase** (met een 'I' prefix) | `public interface IUrenRepository { ... }` | Standaard .NET conventie. |
| **XAML Controls (Name)** | **camelCase** (met type prefix) | `<Button x:Name="btnOpslaan"/>` | Gebruik korte prefixen zoals btn, lbl, txt. |

### 2. Formattering en Whitespace
* **Indenting:** Gebruik 4 spaties voor inspringen. Gebruik geen tabs.

* **Braces** ({ }): Plaats de opening curly brace ({) op een nieuwe regel voor klassen, methoden en control flows (if, for, while).

### 3. MAUI/XAML Conventies
* Styling: Maak optimaal gebruik van Styles en Resource Dictionaries om code-duplicatie te voorkomen.

* Volgorde van Attributen: Houd een logische volgorde aan in XAML-controls: x:Name/x:DataType > Layout Properties > Visual Properties > Event Handlers.

### 4. Opruimen
Verwijder altijd ongebruikte using statements.

Gebruik XML-documentatie (///) voor publieke methoden en klassen.


```bash
