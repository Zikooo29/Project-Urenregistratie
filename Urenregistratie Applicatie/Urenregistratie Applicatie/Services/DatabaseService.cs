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
        _db.CreateTableAsync<Gebruiker>().Wait();
        _db.CreateTableAsync<Bedrijf>().Wait();
        _db.CreateTableAsync<Project>().Wait();
        _db.CreateTableAsync<Urenregistratie>().Wait();
    }

    public SQLiteAsyncConnection Connection => _db;

    // Get all active gebruikers
    public Task<List<Gebruiker>> GetGebruikersAsync()
    {
        return _db.Table<Gebruiker>().ToListAsync();
    }

    // Get single gebruiker by ID
    public Task<Gebruiker?> GetGebruikerByIdAsync(int gebruikerId)
    {
        return _db.Table<Gebruiker>()
                  .Where(g => g.gebruiker_id == gebruikerId)
                  .FirstOrDefaultAsync();
    }

    // Add a new gebruiker
    public Task<int> AddGebruikerAsync(Gebruiker gebruiker)
    {
        return _db.InsertAsync(gebruiker);
    }

    // Update an existing gebruiker
    public Task<int> UpdateGebruikerAsync(Gebruiker gebruiker)
    {
        return _db.UpdateAsync(gebruiker);
    }

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
        await SeedTestProjects();
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
    }

    private async Task SeedTestProjects()
    {
        var existing = await _db.Table<Project>().ToListAsync();
        if (existing.Count > 0)
        {
            return;
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
