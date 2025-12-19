using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Maui.Controls;
using Urenregistratie_Applicatie.Models;
using Urenregistratie_Applicatie.Services;

namespace Urenregistratie_Applicatie.Views;

public partial class Page1 : ContentView
{
    // Database service om data op te slaan en op te halen
    private readonly DatabaseService _databaseService;

    // De huidige registratie waar de gebruiker mee bezig is
    private Urenregistratie? _huidigeRegistratie;

    // Lijst met alle actieve projecten
    private List<Project> _projectLijst = new List<Project>();

    // Boolean om bij te houden of de pagina al geladen is
    private bool _isGeladen;

    public Page1(DatabaseService databaseService)
    {
        InitializeComponent();
        _databaseService = databaseService;
    }

    // Deze functie wordt aangeroepen zodra de pagina wordt getoond
    private async void OnPageLoaded(object? sender, EventArgs e)
    {
        // Zorg dat we maar 1 keer laden
        if (_isGeladen)
            return;

        _isGeladen = true;
        await LaadAlleData();
    }

    // Laad alle benodigde data voor de pagina
    private async Task LaadAlleData()
    {
        await LaadBestaandeRegistratie();
        await LaadProjecten();
        await SlaAutomatischOp();
    }

    // Laad een bestaande registratie of maak een nieuwe aan
    private async Task LaadBestaandeRegistratie()
    {
        try
        {
            // Vraag aan database: is er al een registratie die nog niet af is?
            _huidigeRegistratie = await _databaseService.GetActiveRegistrationAsync();

            if (_huidigeRegistratie == null)
            {
                // Er is nog geen registratie, maak een nieuwe aan
                await MaakNieuweRegistratie();
                return;
            }

            // Er bestaat al een registratie, vul de velden in
            VulVeldenIn(_huidigeRegistratie);
            ZetVeldenReadOnly(_huidigeRegistratie.status);
        }
        catch (Exception ex)
        {
            ToonFoutmelding($"Kon registratie niet laden: {ex.Message}");
        }
    }

    // Maak een nieuwe registratie aan met standaard waardes
    private async Task MaakNieuweRegistratie()
    {
        // Maak een nieuwe registratie met standaard waardes
        _huidigeRegistratie = new Urenregistratie
        {
            datum = DateTime.Today.ToString("dd-MM-yyyy"),
            starttijd = new TimeSpan(8, 30, 0).ToString(),
            eindtijd = new TimeSpan(17, 0, 0).ToString(),
            status = "Draft",
            omschrijving = string.Empty,
        };

        try
        {
            // Sla de nieuwe registratie op in de database
            await _databaseService.SaveRegistrationAsync(_huidigeRegistratie);

            // Vul de velden in met de standaard waardes
            VulVeldenIn(_huidigeRegistratie);
            projectPicker.SelectedItem = null;
            ZetVeldenReadOnly(_huidigeRegistratie.status);

            ToonStatus("Er staat één actieve registratie open. Vul de gegevens aan.");
        }
        catch (Exception ex)
        {
            ToonFoutmelding($"Kon registratie niet aanmaken: {ex.Message}");
        }
    }

    // Vul de formuliervelden in met data uit de registratie
    private void VulVeldenIn(Urenregistratie registratie)
    {
        // Zet de datum
        if (DateTime.TryParse(registratie.datum, out DateTime datum))
        {
            datePicker.Date = datum;
        }
        else
        {
            datePicker.Date = DateTime.Today;
        }

        // Zet de starttijd
        if (TimeSpan.TryParse(registratie.starttijd, out TimeSpan start))
        {
            startTimePicker.Time = start;
        }

        // Zet de eindtijd
        if (TimeSpan.TryParse(registratie.eindtijd, out TimeSpan eind))
        {
            endTimePicker.Time = eind;
        }

        // Zet de opmerking
        commentEditor.Text = registratie.omschrijving;
    }

    // Laad alle actieve projecten uit de database
    private async Task LaadProjecten()
    {
        try
        {
            // Haal alle projecten op die actief zijn
            _projectLijst = await _databaseService.GetActiveProjectsAsync();

            // Zet de projecten in de dropdown
            projectPicker.ItemsSource = _projectLijst;
            projectPicker.ItemDisplayBinding = new Binding("display_naam");

            // Als er al een project gekozen was, selecteer dat
            if (_huidigeRegistratie != null && _huidigeRegistratie.project_id > 0)
            {
                var gekozenProject = _projectLijst.FirstOrDefault(p => p.project_id == _huidigeRegistratie.project_id);

                if (gekozenProject == null)
                {
                    // Het eerder gekozen project is niet meer actief
                    ToonFoutmelding("Het gekozen project is niet meer actief. Kies een ander project.");
                    projectPicker.SelectedItem = null;
                }
                else
                {
                    projectPicker.SelectedItem = gekozenProject;
                }
            }
        }
        catch (Exception ex)
        {
            ToonFoutmelding($"Kon projecten niet laden: {ex.Message}");
        }
    }

    // Sla de huidige data automatisch op
    private async Task SlaAutomatischOp()
    {
        if (_huidigeRegistratie == null)
            return;

        // Haal alle waardes uit de velden
        _huidigeRegistratie.datum = datePicker.Date.ToString("dd-MM-yyyy");
        _huidigeRegistratie.starttijd = startTimePicker.Time.ToString();
        _huidigeRegistratie.eindtijd = endTimePicker.Time.ToString();
        _huidigeRegistratie.omschrijving = commentEditor.Text ?? string.Empty;
        _huidigeRegistratie.status = "Draft";
        _huidigeRegistratie.laatst_gewijzigd = DateTime.Now.ToString("dd-MM-yyyy");

        // Haal het geselecteerde project op
        if (projectPicker.SelectedItem is Project gekozenProject)
        {
            _huidigeRegistratie.project_id = gekozenProject.project_id;
        }
        else
        {
            _huidigeRegistratie.project_id = 0;
        }

        try
        {
            // Maak foutmeldingen leeg
            ToonFoutmelding(string.Empty);
            ToonSuccesmelding(string.Empty);

            // Controleer of alles goed is ingevuld
            ControleerInvoer();

            // Sla op in de database
            await _databaseService.SaveRegistrationAsync(_huidigeRegistratie);
            ToonStatus("Concept automatisch opgeslagen. Werk de registratie af om te voltooien.");
        }
        catch (Exception ex)
        {
            ToonFoutmelding($"Kon niet automatisch opslaan: {ex.Message}");
        }
    }

    // Controleer of alle verplichte velden correct zijn ingevuld
    private void ControleerInvoer()
    {
        var fouten = new List<string>();

        // Controleer of er een project is gekozen
        if (projectPicker.SelectedItem == null)
        {
            fouten.Add("Kies eerst een project voordat je afrondt.");
        }

        // Controleer of eindtijd later is dan starttijd
        if (endTimePicker.Time <= startTimePicker.Time)
        {
            fouten.Add("Eindtijd moet later zijn dan starttijd.");
        }

        // Toon foutmeldingen als die er zijn
        if (fouten.Any())
        {
            ToonFoutmelding(string.Join(" ", fouten));
        }
        else
        {
            ToonFoutmelding(string.Empty);
        }
    }

    // Zet velden op read-only als de registratie af is
    private void ZetVeldenReadOnly(string status)
    {
        // Check of de registratie af is
        bool isAfgerond = string.Equals(status, "Completed", StringComparison.OrdinalIgnoreCase);

        // Zet alle velden uit als de registratie af is
        datePicker.IsEnabled = !isAfgerond;
        projectPicker.IsEnabled = !isAfgerond;
        startTimePicker.IsEnabled = !isAfgerond;
        endTimePicker.IsEnabled = !isAfgerond;
        commentEditor.IsEnabled = !isAfgerond;
        saveButton.IsEnabled = !isAfgerond;

        // Zet de "nieuwe registratie" knop aan als deze af is
        newButton.IsEnabled = isAfgerond;

        if (isAfgerond)
        {
            ToonStatus("De registratie is afgerond en staat op alleen-lezen.");
        }
        else
        {
            ToonStatus("Er staat één actieve registratie open. Er kan geen tweede worden gestart.");
        }
    }

    // Deze functies worden aangeroepen als de gebruiker iets wijzigt
    private async void OnDateChanged(object sender, DateChangedEventArgs e)
    {
        await SlaAutomatischOp();
    }

    private async void OnProjectChanged(object? sender, EventArgs e)
    {
        await SlaAutomatischOp();
    }

    private async void OnStartTimeChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(TimePicker.Time))
        {
            await SlaAutomatischOp();
        }
    }

    private async void OnEndTimeChanged(object? sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(TimePicker.Time))
        {
            await SlaAutomatischOp();
        }
    }

    private async void OnCommentChanged(object? sender, TextChangedEventArgs e)
    {
        await SlaAutomatischOp();
    }

    // Deze functie wordt aangeroepen als je op "Afronden" klikt
    private async void OnSaveButtonClicked(object sender, EventArgs e)
    {
        if (_huidigeRegistratie == null)
            return;

        // Laad de projecten opnieuw om te checken of ze nog actief zijn
        await LaadProjecten();

        // Controleer of er een project is gekozen
        if (projectPicker.SelectedItem is not Project gekozenProject)
        {
            ToonFoutmelding("Selecteer een project voordat je afrondt.");
            return;
        }

        // Check of het project nog steeds actief is
        if (!_projectLijst.Any(p => p.project_id == gekozenProject.project_id))
        {
            ToonFoutmelding("Het gekozen project is inactief geworden. Kies een ander project.");
            projectPicker.SelectedItem = null;
            return;
        }

        // Controleer of eindtijd later is dan starttijd
        if (endTimePicker.Time <= startTimePicker.Time)
        {
            ToonFoutmelding("Eindtijd moet later zijn dan starttijd.");
            return;
        }

        // Bereken het aantal uren
        var aantalUren = (endTimePicker.Time - startTimePicker.Time).TotalHours;

        // Check of het aantal uren positief is
        if (aantalUren <= 0)
        {
            ToonFoutmelding("Het aantal uren kan niet negatief zijn.");
            return;
        }

        try
        {
            // Vul alle velden in
            _huidigeRegistratie.datum = datePicker.Date.ToString("dd-MM-yyyy");
            _huidigeRegistratie.project_id = gekozenProject.project_id;
            _huidigeRegistratie.starttijd = startTimePicker.Time.ToString();
            _huidigeRegistratie.eindtijd = endTimePicker.Time.ToString();
            _huidigeRegistratie.omschrijving = commentEditor.Text ?? string.Empty;
            _huidigeRegistratie.aantal_uren = aantalUren;
            _huidigeRegistratie.status = "Completed";
            _huidigeRegistratie.laatst_gewijzigd = DateTime.Now.ToString("dd-MM-yyyy");

            // Sla op in de database
            await _databaseService.SaveRegistrationAsync(_huidigeRegistratie);

            // Toon succesmelding
            ToonFoutmelding(string.Empty);
            ToonSuccesmelding("Registratie opgeslagen en afgerond. Je kunt nu een nieuwe starten.");
            ZetVeldenReadOnly(_huidigeRegistratie.status);
        }
        catch (Exception ex)
        {
            ToonFoutmelding($"Kon niet opslaan: {ex.Message}");
        }
    }

    // Deze functie wordt aangeroepen als je op "Nieuwe registratie" klikt
    private async void OnNewButtonClicked(object sender, EventArgs e)
    {
        // Check of de huidige registratie wel af is
        if (_huidigeRegistratie != null && _huidigeRegistratie.status == "Draft")
        {
            ToonFoutmelding("Rond eerst de huidige registratie af voordat je een nieuwe begint.");
            return;
        }

        // Laad de projecten opnieuw
        await LaadProjecten();

        // Maak een nieuwe registratie aan
        await MaakNieuweRegistratie();
    }

    // Hulpfuncties om berichten te tonen
    private void ToonFoutmelding(string bericht)
    {
        errorLabel.Text = bericht;
    }

    private void ToonSuccesmelding(string bericht)
    {
        successLabel.Text = bericht;
    }

    private void ToonStatus(string bericht)
    {
        statusLabel.Text = bericht;
    }
}