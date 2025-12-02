using NUnit.Framework;
using SQLite;
using System.Threading.Tasks;
using Urenregistratie_Applicatie.Models;

namespace Test
{
    [TestFixture]
    public class DatabaseServiceTests
    {
        private DatabaseService _dbService;

        [SetUp]
        public void Setup()
        {
            _dbService = new DatabaseService(":memory:");

            // Synchronously wait for table creation
            _dbService.Connection.CreateTableAsync<Gebruiker>().Wait();
            _dbService.Connection.CreateTableAsync<Bedrijf>().Wait();
            _dbService.Connection.CreateTableAsync<Project>().Wait();
            _dbService.Connection.CreateTableAsync<Urenregistratie>().Wait();
        }

        [Test]
        public async Task CanAddAndRetrieveGebruiker()
        {
            var gebruiker = new Gebruiker { Naam = "Jan", Email = "jan@test.com" };
            await _dbService.Connection.InsertAsync(gebruiker);

            var retrieved = await _dbService.Connection.Table<Gebruiker>()
                .Where(g => g.Id == gebruiker.Id)
                .FirstOrDefaultAsync();

            Assert.AreEqual(gebruiker.Naam, retrieved.Naam);
            Assert.AreEqual(gebruiker.Email, retrieved.Email);
        }

        [Test]
        public async Task CanAddAndRetrieveBedrijf()
        {
            var bedrijf = new Bedrijf { Naam = "Acme Corp" };
            await _dbService.Connection.InsertAsync(bedrijf);

            var retrieved = await _dbService.Connection.Table<Bedrijf>()
                .Where(b => b.Id == bedrijf.Id)
                .FirstOrDefaultAsync();

            Assert.AreEqual(bedrijf.Naam, retrieved.Naam);
        }

        [Test]
        public async Task CanAddAndRetrieveProjectWithRelation()
        {
            var bedrijf = new Bedrijf { Naam = "Acme Corp" };
            await _dbService.Connection.InsertAsync(bedrijf);

            var project = new Project { Naam = "Project X", BedrijfId = bedrijf.Id };
            await _dbService.Connection.InsertAsync(project);

            var retrieved = await _dbService.Connection.Table<Project>()
                .Where(p => p.Id == project.Id)
                .FirstOrDefaultAsync();

            Assert.AreEqual(project.Naam, retrieved.Naam);
            Assert.AreEqual(project.BedrijfId, retrieved.BedrijfId);

            // Verify relationship: project.BedrijfId points to an existing Bedrijf  
            var relatedBedrijf = await _dbService.Connection.Table<Bedrijf>()
                .Where(b => b.Id == retrieved.BedrijfId)
                .FirstOrDefaultAsync();
            Assert.IsNotNull(relatedBedrijf);
            Assert.AreEqual(bedrijf.Naam, relatedBedrijf.Naam);
        }

        [Test]
        public async Task CanAddAndRetrieveUrenregistratieWithRelations()
        {
            var gebruiker = new Gebruiker { Naam = "Jan", Email = "jan@test.com" };
            await _dbService.Connection.InsertAsync(gebruiker);

            var bedrijf = new Bedrijf { Naam = "Acme Corp" };
            await _dbService.Connection.InsertAsync(bedrijf);

            var project = new Project { Naam = "Project X", BedrijfId = bedrijf.Id };
            await _dbService.Connection.InsertAsync(project);

            var uren = new Urenregistratie { GebruikerId = gebruiker.Id, ProjectId = project.Id, Uren = 8 };
            await _dbService.Connection.InsertAsync(uren);

            var retrieved = await _dbService.Connection.Table<Urenregistratie>()
                .Where(u => u.Id == uren.Id)
                .FirstOrDefaultAsync();

            Assert.AreEqual(uren.GebruikerId, retrieved.GebruikerId);
            Assert.AreEqual(uren.ProjectId, retrieved.ProjectId);
            Assert.AreEqual(uren.Uren, retrieved.Uren);

            // Verify relationships  
            var relatedGebruiker = await _dbService.Connection.Table<Gebruiker>()
                .Where(g => g.Id == retrieved.GebruikerId)
                .FirstOrDefaultAsync();
            Assert.IsNotNull(relatedGebruiker);
            Assert.AreEqual(gebruiker.Naam, relatedGebruiker.Naam);

            var relatedProject = await _dbService.Connection.Table<Project>()
                .Where(p => p.Id == retrieved.ProjectId)
                .FirstOrDefaultAsync();
            Assert.IsNotNull(relatedProject);
            Assert.AreEqual(project.Naam, relatedProject.Naam);
        }
    }  

}
