using SQLite;
using Urenregistratie_Applicatie.Models;

public class DatabaseService
{
    private readonly SQLiteAsyncConnection _db;

    public DatabaseService(string dbPath)
    {
        _db = new SQLiteAsyncConnection(dbPath);
        _db.CreateTableAsync<Gebruiker>();
        _db.CreateTableAsync<Bedrijf>();
        _db.CreateTableAsync<Project>();
        _db.CreateTableAsync<Urenregistratie>();
    }

    public SQLiteAsyncConnection Connection => _db;

    // Example CRUD:
    //public Task<int> AddGebruiker(Gebruiker g)
    //{
   //     return _db.InsertAsync(g);
   // }
}

