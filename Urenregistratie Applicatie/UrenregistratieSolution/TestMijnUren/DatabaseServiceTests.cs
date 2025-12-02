using NUnit.Framework;
using System.Threading.Tasks;
using Urenregistratie_Applicatie.Models;

namespace Urenregistratie.Tests
{
    [TestFixture]
    public class DatabaseServiceTests
    {
        private DatabaseService _db;

        [SetUp]
        public async Task Setup()
        {
            // Gebruik in-memory SQLite database
            _db = new DatabaseService(":memory:");

            // Zorg dat alle tabellen bestaan
            await _db.Connection.CreateTableAsync<Gebruiker>();
            await _db.Connection.CreateTableAsync<Bedrijf>();
            await _db.Connection.CreateTableAsync<Project>();
            await _db.Connection.CreateTableAsync<Urenregistratie>();
        }

        [Test]
        public async Task Insert_And_Read_Gebruiker_Works()
        {
            var g = new Gebruiker
            {
                Naam = "Jan",
                Email = "jan@test.com"
            };

            await _db.Connection.InsertAsync(g);

            var dbItem = await _db.Connection.Table<Gebruiker>()
                .Where(x => x.Id == g.Id)
                .FirstOrDefaultAsync();

            Assert.IsNotNull(dbItem);
            Assert.AreEqual(g.Naam, dbItem.Naam);
            Assert.AreEqual(g.Email, dbItem.Email);
        }

        [Test]
        public async Task Insert_And_Retrieve_Bedrijf_Works()
        {
            var b = new Bedrijf { Naam = "Acme Corp" };
            await _db.Connection.InsertAsync(b);

            var dbBedrijf = await _db.Connection.Table<Bedrijf>()
                .Where(x => x.Id == b.Id)
                .FirstOrDefaultAsync();

            Assert.IsNotNull(dbBedrijf);
            Assert.AreEqual(b.Naam, dbBedrijf.Naam);
        }

        [Test]
        public async Task Insert_And_Retrieve_Project_With_Bedrijf()
        {
            var b = new Bedrijf { Naam = "Acme Corp" };
            await _db.Connection.InsertAsync(b);

            var p = new Project { Naam = "Project X", BedrijfId = b.Id };
            await _db.Connection.InsertAsync(p);

            var dbProject = await _db.Connection.Table<Project>()
                .Where(x => x.Id == p.Id)
                .FirstOrDefaultAsync();

            Assert.IsNotNull(dbProject);
            Assert.AreEqual("Project X", dbProject.Naam);
            Assert.AreEqual(b.Id, dbProject.BedrijfId);
        }

        [Test]
        public async Task Insert_And_Read_Urenregistratie_Works()
        {
            var gebruiker = new Gebruiker { Naam = "Jan", Email = "jan@test.com" };
            await _db.Connection.InsertAsync(gebruiker);

            var bedrijf = new Bedrijf { Naam = "Acme Corp" };
            await _db.Connection.InsertAsync(bedrijf);

            var project = new Project { Naam = "Project X", BedrijfId = bedrijf.Id };
            await _db.Connection.InsertAsync(project);

            var u = new Urenregistratie
            {
                GebruikerId = gebruiker.Id,
                ProjectId = project.Id,
                Uren = 8
            };

            await _db.Connection.InsertAsync(u);

            var dbU = await _db.Connection.Table<Urenregistratie>()
                .Where(x => x.Id == u.Id)
                .FirstOrDefaultAsync();

            Assert.IsNotNull(dbU);
            Assert.AreEqual(8, dbU.Uren);
            Assert.AreEqual(gebruiker.Id, dbU.GebruikerId);
            Assert.AreEqual(project.Id, dbU.ProjectId);
        }
    }
}
