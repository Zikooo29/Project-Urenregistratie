namespace Urenregistratie_Applicatie.Services;

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

    public async Task InitAsync()
    {
        await SeedTestUsers();
    }
    private async Task SeedTestUsers()
    {
        var existing = await _db.Table<Gebruiker>().ToListAsync();
        if (existing.Count > 0)
            return; // database is al gevuld → niet opnieuw seeden

        var testUsers = new List<Gebruiker>
    {
        new Gebruiker
        {
            voornaam = "Jan",
            achternaam = "Jansen",
            email = "jan@example.com",
            rol = "Admin"
        },
        new Gebruiker
        {
            voornaam = "Piet",
            achternaam = "Pieters",
            email = "piet@example.com",
            rol = "Medewerker"
        },
        new Gebruiker
        {
            voornaam = "Kees",
            achternaam = "Klaassen",
            email = "kees@example.com",
            rol = "Manager"
        }
    };

        foreach (var u in testUsers)
            await _db.InsertAsync(u);
    }
}

