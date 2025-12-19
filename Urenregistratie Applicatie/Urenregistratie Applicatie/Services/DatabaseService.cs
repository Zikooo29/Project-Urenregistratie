namespace Urenregistratie_Applicatie.Services;

using System.Collections.Generic;
using SQLite;
using Urenregistratie_Applicatie.Models;

public class DatabaseService
{
    private readonly SQLiteAsyncConnection _db;

    public DatabaseService(string dbPath)
    {
        _db = new SQLiteAsyncConnection(dbPath);

        // Tabellen aanmaken
        _db.CreateTableAsync<Gebruiker>().Wait();
        _db.CreateTableAsync<Bedrijf>().Wait();
        _db.CreateTableAsync<Project>().Wait();
        _db.CreateTableAsync<Urenregistratie>().Wait();
        _db.CreateTableAsync<GebruikerBedrijf>().Wait();
        _db.CreateTableAsync<ProjectMedewerker>().Wait();
    }

    public SQLiteAsyncConnection Connection => _db;

    // GEBRUIKERS
    public Task<List<Gebruiker>> GetGebruikersAsync() => _db.Table<Gebruiker>().ToListAsync();
    public Task<Gebruiker?> GetGebruikerByIdAsync(int gebruikerId) =>
        _db.Table<Gebruiker>().Where(g => g.gebruiker_id == gebruikerId).FirstOrDefaultAsync();
    public Task<int> AddGebruikerAsync(Gebruiker gebruiker) => _db.InsertAsync(gebruiker);
    public Task<int> UpdateGebruikerAsync(Gebruiker gebruiker) => _db.UpdateAsync(gebruiker);
    public async Task<int> DeleteGebruikerAsync(int gebruikerId)
    {
        var gebruiker = await GetGebruikerByIdAsync(gebruikerId);
        return gebruiker != null ? await _db.DeleteAsync(gebruiker) : 0;
    }

    // PROJECTEN
    public Task<List<Project>> GetProjectenAsync() => _db.Table<Project>().ToListAsync();
    public Task<int> AddProjectAsync(Project project) => _db.InsertAsync(project);
    public Task<int> UpdateProjectAsync(Project project) => _db.UpdateAsync(project);
    public Task<int> DeleteProjectAsync(Project project) => _db.DeleteAsync(project);
    public Task<Project?> GetProjectByIdAsync(int id)
    {
        return _db.Table<Project>().Where(p => p.project_id == id).FirstOrDefaultAsync();
    }

    // BEDRIJVEN
    public Task<List<Bedrijf>> GetBedrijvenAsync() => _db.Table<Bedrijf>().ToListAsync();
    public Task<int> AddBedrijfAsync(Bedrijf bedrijf) => _db.InsertAsync(bedrijf);
    public Task<int> UpdateBedrijfAsync(Bedrijf bedrijf) => _db.UpdateAsync(bedrijf);
    public Task<int> DeleteBedrijfAsync(Bedrijf bedrijf) => _db.DeleteAsync(bedrijf);

    // Delete gebruiker permanently from the database
    public async Task<int> DeleteGebruikerAsync(int gebruikerId)
    {
        var gebruiker = await _db.Table<Gebruiker>()
                                 .Where(g => g.gebruiker_id == gebruikerId)
                                 .FirstOrDefaultAsync();
        if (gebruiker != null)
        {
            return await _db.DeleteAsync(gebruiker);
        }

        return 0;
    }

    public async Task<List<Project>> GetActiveProjectsAsync()
    {
        return await _db.Table<Project>()
                        .Where(p => p.actief)
                        .ToListAsync();
    }

    public Task<Project?> GetProjectByIdAsync(int projectId)
    {
        return _db.Table<Project>()
                  .Where(p => p.project_id == projectId)
                  .FirstOrDefaultAsync();
    }

    public async Task<Urenregistratie?> GetActiveRegistrationAsync()
    {
        return await _db.Table<Urenregistratie>()
                        .Where(r => r.status == "Draft")
                        .OrderByDescending(r => r.registratie_id)
                        .FirstOrDefaultAsync();
    }

    public Task<int> SaveRegistrationAsync(Urenregistratie registratie)
    {
        if (registratie.registratie_id == 0)
        {
            return _db.InsertAsync(registratie);
        }

        return _db.UpdateAsync(registratie);
    }

    public async Task InitAsync()
    {
        await SeedTestUsers();
        await SeedTestCompanies();
        await SeedTestProjects();
        await SeedGebruikerBedrijven();
        await SeedProjectMedewerkers();
    }

    private async Task SeedTestUsers()
    {
        var existing = await _db.Table<Gebruiker>().ToListAsync();
        if (existing.Count > 2)
        {
            return;
        }

        var testUsers = new List<Gebruiker>
        {
            new Gebruiker
            {
                voornaam = "Jan",
                achternaam = "Jansen",
                email = "jan@example.com",
                rol = "Medewerker",
            },
        };

        foreach (var u in testUsers)
        {
            await _db.InsertAsync(u);
    }
    private async Task SeedTestCompanies()
    {
        var existing = await _db.Table<Bedrijf>().ToListAsync();
        if (existing.Count > 0)
            return;

        var bedrijven = new List<Bedrijf>
    {
        new Bedrijf
        {
            bedrijfsnaam = "TechSolutions B.V.",
            contactpersoon = "Emma de Boer",
            email = "emma@techsolutions.nl",
            telefoonnummer = "010-1234567",
            adres = "Innovatiepad 1, Rotterdam",
            type = "ICT"
        },
        new Bedrijf
        {
            bedrijfsnaam = "GreenEnergy Nederland",
            contactpersoon = "Mark van Dijk",
            email = "mark@greenenergy.nl",
            telefoonnummer = "020-7654321",
            adres = "Duurzaamplein 4, Amsterdam",
            type = "Energie"
        }
    };

        foreach (var b in bedrijven)
            await _db.InsertAsync(b);
    }
    private async Task SeedTestProjects()
    {
        var existing = await _db.Table<Project>().ToListAsync();
        if (existing.Count > 0)
            return;

        var bedrijven = await _db.Table<Bedrijf>().ToListAsync();
        if (!bedrijven.Any())
            return; // geen bedrijven aanwezig om aan projecten te koppelen

        int bedrijfId = bedrijven.First().bedrijf_id;

        var projecten = new List<Project>
    {
        new Project
        {
            projectnaam = "Website Redesign",
            beschrijving = "Nieuwe UI/UX voor klantportaal",
            startdatum = "2025-01-10",
            einddatum = "2025-03-30",
            status = "Actief",
            bedrijf_id = bedrijfId
        },
        new Project
        {
            projectnaam = "Energie Dashboard",
            beschrijving = "Realtime verbruik monitoring",
            startdatum = "2025-02-01",
            einddatum = "2025-06-01",
            status = "Gepland",
            bedrijf_id = bedrijfId
        }
    };

        foreach (var p in projecten)
            await _db.InsertAsync(p);
    }
    private async Task SeedGebruikerBedrijven()
    {
        var bestaande = await _db.Table<GebruikerBedrijf>().ToListAsync();
        if (bestaande.Any())
            return;

        var gebruikers = await _db.Table<Gebruiker>().ToListAsync();
        var bedrijven = await _db.Table<Bedrijf>().ToListAsync();

        if (!gebruikers.Any() || !bedrijven.Any())
            return;

        var koppelingen = new List<GebruikerBedrijf>
    {
        new GebruikerBedrijf
        {
            gebruiker_id = gebruikers.First().gebruiker_id,
            bedrijf_id = bedrijven[0].bedrijf_id,
            rol = "Werknemer"
        },
        new GebruikerBedrijf
        {
            gebruiker_id = gebruikers.First().gebruiker_id,
            bedrijf_id = bedrijven[1].bedrijf_id,
            rol = "Projectleider"
        }
    };

        foreach (var k in koppelingen)
            await _db.InsertAsync(k);
    }
    private async Task SeedProjectMedewerkers()
    {
        var bestaande = await _db.Table<ProjectMedewerker>().ToListAsync();
        if (bestaande.Any())
            return;

        var gebruikers = await _db.Table<Gebruiker>().ToListAsync();
        var projecten = await _db.Table<Project>().ToListAsync();

        if (!gebruikers.Any() || !projecten.Any())
            return;

        var koppelingen = new List<ProjectMedewerker>
    {
        new ProjectMedewerker
        {
            gebruiker_id = gebruikers.First().gebruiker_id,
            project_id = projecten[0].project_id,
            rol = "Medewerker"
        },
        new ProjectMedewerker
        {
            gebruiker_id = gebruikers.First().gebruiker_id,
            project_id = projecten[1].project_id,
            rol = "Projectleider"
        }
    };

        foreach (var k in koppelingen)
            await _db.InsertAsync(k);
    }

}


        var testProjects = new List<Project>
        {
            new Project
            {
                projectcode = "PRJ-001",
                projectnaam = "Website vernieuwing",
                beschrijving = "Nieuwe homepage voor klant",
                status = "actief",
                actief = true,
            },
            new Project
            {
                projectcode = "PRJ-002",
                projectnaam = "Interne tools",
                beschrijving = "Verbetering urenportaal",
                status = "actief",
                actief = true,
            },
            new Project
            {
                projectcode = "PRJ-003",
                projectnaam = "Oud project",
                beschrijving = "Niet meer actief",
                status = "inactief",
                actief = false,
            },
        };

        foreach (var project in testProjects)
        {
            await _db.InsertAsync(project);
        }
    }
}
