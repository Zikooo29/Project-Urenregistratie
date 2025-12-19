using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using Urenregistratie_Applicatie.Models;
using Urenregistratie_Applicatie.Services;
using Urenregistratie_Applicatie.ViewModels;
using Urenregistratie_Applicatie.Helpers;

namespace Urenregistratie_Applicatie.Views;

public partial class Page3 : ContentView
{
    private readonly ObservableCollection<GebruikerViewModel> _visibleUsers = new();
    private readonly ObservableCollection<Gebruiker> _allDbUsers = new();
    private readonly ObservableCollection<ProjectViewModel> _projectenVM = new();
    private readonly ObservableCollection<BedrijfViewModel> _bedrijvenVM = new();


    // Edit state
    private bool _isEditing = false;
    private GebruikerViewModel? _editingUser = null;
    private bool _isEditingCompany = false;
    private BedrijfViewModel? _editingCompany = null;

    private bool _isEditingProject = false;
    private ProjectViewModel? _editingProject = null;



    // Zoeken/sorteer instellingen
    private string _searchTerm = string.Empty;
    private string _sortColumn = nameof(GebruikerViewModel.Id);
    private bool _sortAscending = true;
    private bool _isAdminUser = true;
    private readonly string _currentUserId = "2";

    private readonly DatabaseService _databaseService;

    public Page3( DatabaseService databaseService)
    {
        InitializeComponent();

        _databaseService = databaseService;

        UsersCollection.ItemsSource = _visibleUsers;
    }

    private async void OnLoaded(object? sender, EventArgs e)
    {
        try
        {
            AdminContent.IsVisible = _isAdminUser;
            AccessDeniedCard.IsVisible = !_isAdminUser;
            if (!_isAdminUser) return;

            await InitializeAsync();
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Fout bij laden pagina", ex.Message, "OK");
        }
    }

    private async Task InitializeAsync()
    {
        try
        {
            AdminContent.IsVisible = _isAdminUser;
            AccessDeniedCard.IsVisible = !_isAdminUser;

            if (!_isAdminUser)
                return;

           
            await LoadDbUsersAsync();

            await LoadProjectenAsync();
            await LoadBedrijvenAsync();
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Fout bij initialisatie", ex.Message, "OK");
        }
    }

  

    private async Task LoadDbUsersAsync()
    {
        try
        {
            _allDbUsers.Clear();
            var dbUsers = await _databaseService.GetGebruikersAsync();
            foreach (var g in dbUsers)
            {
                _allDbUsers.Add(g);
            }
            ApplyDbFiltersAndSorting();
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Fout bij laden van databasegebruikers", ex.Message, "OK");
        }
    }

    private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        try
        {
            _searchTerm = e.NewTextValue ?? string.Empty;
            ApplyDbFiltersAndSorting();
        }
        catch (Exception ex)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await Application.Current.MainPage.DisplayAlert("Fout bij zoeken", ex.Message, "OK");
            });
        }
    }




    private void ApplyDbFiltersAndSorting()
    {
        try
        {
            IEnumerable<Gebruiker> filtered = _allDbUsers;

            // Sorteert de gebruikers dynamisch op de geselecteerde kolom en richting oplopend/aflopend, met ID als fallback


            if (!string.IsNullOrWhiteSpace(_searchTerm))
            {
                var term = _searchTerm.Trim();

                filtered = filtered.Where(g =>
                    (g.voornaam?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (g.achternaam?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false) ||
                    (g.email?.Contains(term, StringComparison.OrdinalIgnoreCase) ?? false));
            }

            // Sorteren op basis van _sortColumn en _sortAscending
            filtered = (_sortColumn, _sortAscending) switch
            {
                (nameof(GebruikerViewModel.FirstName), true) => filtered.OrderBy(g => g.voornaam),
                (nameof(GebruikerViewModel.FirstName), false) => filtered.OrderByDescending(g => g.voornaam),

                (nameof(GebruikerViewModel.LastName), true) => filtered.OrderBy(g => g.achternaam),
                (nameof(GebruikerViewModel.LastName), false) => filtered.OrderByDescending(g => g.achternaam),

                (nameof(GebruikerViewModel.Email), true) => filtered.OrderBy(g => g.email),
                (nameof(GebruikerViewModel.Email), false) => filtered.OrderByDescending(g => g.email),

                (nameof(GebruikerViewModel.Role), true) => filtered.OrderBy(g => g.rol),
                (nameof(GebruikerViewModel.Role), false) => filtered.OrderByDescending(g => g.rol),

                (nameof(GebruikerViewModel.Id), true) => filtered.OrderBy(g => g.gebruiker_id),
                (nameof(GebruikerViewModel.Id), false) => filtered.OrderByDescending(g => g.gebruiker_id),

                _ => filtered.OrderBy(g => g.gebruiker_id)
            };

            //Vult de zichtbare gebruikerslijst opnieuw op basis van de gefilterde en gesorteerde resultaten en toont een lege-status indien nodig
            _visibleUsers.Clear();
            foreach (var g in filtered)
            {
                _visibleUsers.Add(ViewModelMapper.ToViewModel(g));
            }

            EmptyStateLabel.IsVisible = !_visibleUsers.Any();
        }

        // Geeft een foutmelding weer als er iets misgaat bij het filteren of sorteren van de gebruikerslijst
        catch (Exception ex)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Fout bij filteren databasegebruikers",
                    ex.Message,
                    "OK");
            });
        }
    }


    private void ApplySorting(string sortColumn)
    {
        if (_sortColumn == sortColumn)
        {
            _sortAscending = !_sortAscending;
        }
        else
        {
            _sortColumn = sortColumn;
            _sortAscending = true;
        }

        UpdateSortHeaderLabels();
        ApplyDbFiltersAndSorting();
    }

    private void OnSortByFirstName(object sender, EventArgs e) =>
        ApplySorting(nameof(GebruikerViewModel.FirstName));

    private void OnSortByLastName(object sender, EventArgs e) =>
        ApplySorting(nameof(GebruikerViewModel.LastName));

    private void OnSortByEmail(object sender, EventArgs e) =>
        ApplySorting(nameof(GebruikerViewModel.Email));

    private void OnSortByRole(object sender, EventArgs e) =>
        ApplySorting(nameof(GebruikerViewModel.Role));

    private void OnSortById(object sender, EventArgs e) =>
        ApplySorting(nameof(GebruikerViewModel.Id));

    private void UpdateSortHeaderLabels()
    {
        FirstNameHeader.Text = BuildSortLabel("Voornaam", nameof(GebruikerViewModel.FirstName));
        LastNameHeader.Text = BuildSortLabel("Achternaam", nameof(GebruikerViewModel.LastName));
        EmailHeader.Text = BuildSortLabel("E-mail", nameof(GebruikerViewModel.Email));
        RoleHeader.Text = BuildSortLabel("Rol", nameof(GebruikerViewModel.Role));
        UserIdHeader.Text = BuildSortLabel("Gebruikers-ID", nameof(GebruikerViewModel.Id));
    }

    private string BuildSortLabel(string kolomNaam, string kolomPropertyNaam)
    {
        if (_sortColumn != kolomPropertyNaam)
            return kolomNaam;

        string pijltje = _sortAscending ? "▲" : "▼";
        return kolomNaam + " " + pijltje;
    }

    // Tijdelijke knoppen voor gebruikers toevoegen, wijzigen en gebruikers verwijderen.
    private async void OnAddUsersClicked(object sender, EventArgs e) 
    {
        // Popup leegmaken en tonen
        ClearAddUserForm();
        PopupTitle.Text = "Nieuwe gebruiker toevoegen";
        ConfirmAddUserButton.Text = "Bevestigen";
        AddUserPopup.IsVisible = true;
    }
    private async void OnEditUsersClicked(object sender, EventArgs e) 
    {

        if (sender is not Button button)
            return;

        // verwacht CommandParameter="{Binding}" in XAML
        var user = button.CommandParameter as GebruikerViewModel;
        if (user is null)
            return;

        // Zet edit-state
        _isEditing = true;
        _editingUser = user;

        // Vul popup velden met huidige waarden
        FirstNameEntry.Text = user.FirstName;
        LastNameEntry.Text = user.LastName;
        EmailEntry.Text = user.Email;

        // zet role picker's index op basis van items (als match)
        var index = -1;
        for (int i = 0; i < RolePicker.Items.Count; i++)
        {
            if (string.Equals(RolePicker.Items[i], user.Role, StringComparison.OrdinalIgnoreCase))
            {
                index = i;
                break;
            }
        }
        RolePicker.SelectedIndex = index;

        // knoptekst en validatie
        ConfirmAddUserButton.Text = "Opslaan";
        ConfirmAddUserButton.IsEnabled = false; // Validatie regelt wanneer true

        // toon edit popup
        PopupTitle.Text = "Gebruiker wijzigen";
        ConfirmAddUserButton.Text = "Opslaan";
        AddUserPopup.IsVisible = true;
        
        ValidateAddUserForm();
    }

    private async void OnDeleteUsersClicked(object sender, EventArgs e)
    {
        try
        {
            if (sender is not Button button) return;

            var user = button.CommandParameter as GebruikerViewModel;
            if (user == null) return;

            if (user.Id == _currentUserId)
            {
                await Application.Current.MainPage.DisplayAlert(
                    "Niet toegestaan", "Je kunt je eigen account niet verwijderen.", "OK");
                return;
            }

            bool bevestigen = await Application.Current.MainPage.DisplayAlert(
                "Gebruiker verwijderen",
                $"Weet je zeker dat je {user.FirstName} {user.LastName} wilt verwijderen?",
                "Verwijderen", "Annuleren");

            if (!bevestigen) return;

            if (int.TryParse(user.Id, out int dbId))
            {
                await _databaseService.DeleteGebruikerAsync(dbId);
                var dbUser = _allDbUsers.FirstOrDefault(g => g.gebruiker_id == dbId);
                if (dbUser != null) _allDbUsers.Remove(dbUser);
                ApplyDbFiltersAndSorting();
            }
          

            await Application.Current.MainPage.DisplayAlert(
                "Succes", "De gebruiker is verwijderd uit het overzicht.", "OK");
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Fout bij verwijderen", ex.Message, "OK");
        }
    }
// ADJOA Formulier leegmaken en standaardwaarden instellen
private void ClearAddUserForm()
{
    FirstNameEntry.Text = string.Empty;
    LastNameEntry.Text = string.Empty;
    EmailEntry.Text = string.Empty;

    // Rol standaard op "Werknemer" zetten
    RolePicker.SelectedIndex = -1;

    FirstNameErrorLabel.IsVisible = false;
    LastNameErrorLabel.IsVisible = false;
    EmailErrorLabel.IsVisible = false;
    RoleErrorLabel.IsVisible = false;

    ConfirmAddUserButton.IsEnabled = false;
    
    // reset edit-state
    _isEditing = false;
    _editingUser = null;
    ConfirmAddUserButton.Text = "Bevestigen";
}

// ANNULEREN knop (popup sluiten en edit-state resetten)
private void OnCancelAddUserClicked(object sender, EventArgs e)
{
    AddUserPopup.IsVisible = false;
    
    // reset edit-state (geen wijziging doorgevoerd)
    _isEditing = false;
    _editingUser = null;
    ConfirmAddUserButton.Text = "Bevestigen";
}

// Wordt aangeroepen als een veld verandert (TextChanged/SelectedIndexChanged)
private void OnAddUserFieldChanged(object sender, EventArgs e)
{
    ValidateAddUserForm();
}

    private void OnShowUsersClicked(object sender, EventArgs e)
    {
        UsersTab.IsVisible = true;
        ProjectsTab.IsVisible = false;
        CompaniesTab.IsVisible = false;
    }

    private void OnShowProjectsClicked(object sender, EventArgs e)
    {
        UsersTab.IsVisible = false;
        ProjectsTab.IsVisible = true;
        CompaniesTab.IsVisible = false;
    }

    private void OnShowCompaniesClicked(object sender, EventArgs e)
    {
        UsersTab.IsVisible = false;
        ProjectsTab.IsVisible = false;
        CompaniesTab.IsVisible = true;
    }


    // Controleren of alle invoer geldig is
    private void ValidateAddUserForm()
{
    bool isValid = true;

    // Voornaam
    if (string.IsNullOrWhiteSpace(FirstNameEntry.Text))
    {
        FirstNameErrorLabel.IsVisible = true;
        isValid = false;
    }
    else
    {
        FirstNameErrorLabel.IsVisible = false;
    }

    // Achternaam
    if (string.IsNullOrWhiteSpace(LastNameEntry.Text))
    {
        LastNameErrorLabel.IsVisible = true;
        isValid = false;
    }
    else
    {
        LastNameErrorLabel.IsVisible = false;
    }

    // E-mail
    var email = EmailEntry.Text?.Trim() ?? string.Empty;

    if (string.IsNullOrWhiteSpace(email) || !IsValidEmail(email))
    {
        EmailErrorLabel.Text = "Vul een geldig e-mailadres in";
        EmailErrorLabel.IsVisible = true;
        isValid = false;
    }
        //Email mag hetzelfde blijven bij edit
        else if (_allDbUsers.Any(u =>
         u.email.Equals(email, StringComparison.OrdinalIgnoreCase) &&
         u.gebruiker_id.ToString() != (_editingUser?.Id ?? string.Empty)))
        {
        EmailErrorLabel.Text = "Dit e-mailadres bestaat al";
        EmailErrorLabel.IsVisible = true;
        isValid = false;
    }
    else
    {
        EmailErrorLabel.IsVisible = false;
    }

    // Rol
    if (RolePicker.SelectedIndex < 0)
    {
        RoleErrorLabel.IsVisible = true;
        isValid = false;
    }
    else
    {
        RoleErrorLabel.IsVisible = false;
    }

    // Bevestigen-knop alleen actief als alles goed is
    ConfirmAddUserButton.IsEnabled = isValid;
}

//e-mailcheck
private bool IsValidEmail(string email)
{
    return email.Contains("@") && email.Contains(".");
}



// Bevestigen: gebruiker aanmaken, opslaan en lijst verversen
private async void OnConfirmAddUserClicked(object sender, EventArgs e)
{
    // Validatie uitvoeren
    ValidateAddUserForm();
    if (!ConfirmAddUserButton.IsEnabled)
        return;

    if (_isEditing && _editingUser is not null)
    {
        // maak nieuw object (want properties zijn init-only)
        var updatedUser = new Gebruiker
        {
            gebruiker_id = int.Parse(_editingUser.Id.Replace("#", "")),
            voornaam = FirstNameEntry.Text.Trim(),
            achternaam = LastNameEntry.Text.Trim(),
            email = EmailEntry.Text.Trim(),
            rol = RolePicker.SelectedItem?.ToString() ?? (_editingUser.Role ?? string.Empty)
        };

            // Update in service (mock)
            await _databaseService.UpdateGebruikerAsync(updatedUser);

        // Vervang in lokale lijst (_allUsers) zodat ObservableCollection de UI updatet
       // var index = _allUsers.IndexOf(_allUsers.First(u => u.Id == _editingUser.Id));
   

            // Vervang in lokale lijst (_allUsers) zodat ObservableCollection de UI updatet
            var index = _allDbUsers.IndexOf(_allDbUsers.First(g => g.gebruiker_id == updatedUser.gebruiker_id));
            if (index >= 0)
            {
                _allDbUsers[index] = updatedUser;
            }

            // Herbouw zichtbare lijst en sluit popup
            ApplyDbFiltersAndSorting();

            AddUserPopup.IsVisible = false;

        // reset edit-state
        _isEditing = false;
        _editingUser = null;
        ConfirmAddUserButton.Text = "Bevestigen";

        await Application.Current.MainPage.DisplayAlert(
            "Succes",
            "De wijzigingen zijn opgeslagen.",
            "OK");
      


            return;


    }

    // --- anders: nieuwe gebruiker toevoegen (bestaande logica) ---
    var newUser = new Gebruiker
    {
        voornaam = FirstNameEntry.Text.Trim(),
        achternaam = LastNameEntry.Text.Trim(),
        email = EmailEntry.Text.Trim(),
        rol = RolePicker.SelectedItem?.ToString() ?? "Werknemer"
    };

    await _databaseService.AddGebruikerAsync(newUser);
    _allDbUsers.Add(newUser);
    ApplyDbFiltersAndSorting();

        AddUserPopup.IsVisible = false;

    await Application.Current.MainPage.DisplayAlert(
        "Succes",
        "De gebruiker is succesvol toegevoegd.",
        "OK");

       
    }
    private async Task LoadProjectenAsync()
    {
        try
        {
            _projectenVM.Clear();
            var projecten = await _databaseService.GetProjectenAsync();

            foreach (var p in projecten)
            {
                _projectenVM.Add(ViewModelMapper.ToViewModel(p));
            }

            ProjectsCollection.ItemsSource = _projectenVM;
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert(
                "Fout bij projecten laden",
                ex.Message,
                "OK");
        }
    }

    private async Task LoadBedrijvenAsync()
    {
        try
        {
            _bedrijvenVM.Clear();
            var bedrijven = await _databaseService.GetBedrijvenAsync();

            foreach (var b in bedrijven)
            {
                _bedrijvenVM.Add(ViewModelMapper.ToViewModel(b));
            }

            CompaniesCollection.ItemsSource = _bedrijvenVM;
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert(
                "Fout bij bedrijven laden",
                ex.Message,
                "OK");
        }
    }

    private async void OnAddProjectClicked(object sender, EventArgs e)
    {
        ValidateCompanyForm();
    }


    private void ValidateCompanyForm()
    {
        bool isValid = true;


        if (string.IsNullOrWhiteSpace(CompanyNameEntry.Text))
        {
            CompanyNameErrorLabel.IsVisible = true;
            isValid = false;
        }
        else CompanyNameErrorLabel.IsVisible = false;


        if (string.IsNullOrWhiteSpace(CompanyEmailEntry.Text) || !CompanyEmailEntry.Text.Contains("@"))
        {
            CompanyEmailErrorLabel.IsVisible = true;
            isValid = false;
        }
        else CompanyEmailErrorLabel.IsVisible = false;


        ConfirmAddCompanyButton.IsEnabled = isValid;
    }


    private void OnCancelAddCompanyClicked(object sender, EventArgs e)
    {
        AddCompanyPopup.IsVisible = false;
        _isEditingCompany = false;
        _editingCompany = null;
        ConfirmAddCompanyButton.Text = "Bevestigen";
    }


    private async void OnConfirmAddCompanyClicked(object sender, EventArgs e)
    {
        ValidateCompanyForm();
        if (!ConfirmAddCompanyButton.IsEnabled)
            return;


        if (_isEditingCompany && _editingCompany is not null)
        {
            var updated = new Bedrijf
            {
                bedrijf_id = int.Parse(_editingCompany.Id.Replace("#", "")),
                bedrijfsnaam = CompanyNameEntry.Text.Trim(),
                contactpersoon = CompanyContactEntry.Text.Trim(),
                email = CompanyEmailEntry.Text.Trim(),
                telefoonnummer = CompanyPhoneEntry.Text.Trim()
            };


            await _databaseService.UpdateBedrijfAsync(updated);


            var index = _bedrijvenVM.IndexOf(_bedrijvenVM.First(b => b.Id == _editingCompany.Id));
            if (index >= 0)
                _bedrijvenVM[index] = ViewModelMapper.ToViewModel(updated);


            AddCompanyPopup.IsVisible = false;
            _isEditingCompany = false;
            _editingCompany = null;
        }
        else
        {
            var nieuw = new Bedrijf
            {
                bedrijfsnaam = CompanyNameEntry.Text.Trim(),
                contactpersoon = CompanyContactEntry.Text.Trim(),
                email = CompanyEmailEntry.Text.Trim(),
                telefoonnummer = CompanyPhoneEntry.Text.Trim()
            };


            await _databaseService.AddBedrijfAsync(nieuw);
            _bedrijvenVM.Add(ViewModelMapper.ToViewModel(nieuw));


            AddCompanyPopup.IsVisible = false;
        }


        await Application.Current.MainPage.DisplayAlert("Succes", "Bedrijf opgeslagen", "OK");
    }




    // ✅ Sorteren Projecten & Bedrijven


    private void OnSortProjectsByName(object sender, EventArgs e)
    {
        var sorted = _projectenVM.OrderBy(p => p.Naam).ToList();
        _projectenVM.Clear();
        foreach (var item in sorted) _projectenVM.Add(item);
    }


    private void OnSortProjectsByStatus(object sender, EventArgs e)
    {
        var sorted = _projectenVM.OrderBy(p => p.Status).ToList();
        _projectenVM.Clear();
        foreach (var item in sorted) _projectenVM.Add(item);
    }


    private void OnSortCompaniesByName(object sender, EventArgs e)
    {
        var sorted = _bedrijvenVM.OrderBy(b => b.Naam).ToList();
        _bedrijvenVM.Clear();
        foreach (var item in sorted) _bedrijvenVM.Add(item);
    }


    private void OnSortCompaniesByContact(object sender, EventArgs e)
    {
        var sorted = _bedrijvenVM.OrderBy(b => b.Contactpersoon).ToList();
        _bedrijvenVM.Clear();
        foreach (var item in sorted) _bedrijvenVM.Add(item);
    }

    private void OnProjectSearchChanged(object sender, TextChangedEventArgs e)
    {
        var term = e.NewTextValue?.Trim().ToLower() ?? string.Empty;

        var gefilterd = _projectenVM
            .Where(p => (p.Naam?.ToLower().Contains(term) ?? false) ||
                        (p.Status?.ToLower().Contains(term) ?? false))
            .ToList();

        ProjectsCollection.ItemsSource = gefilterd;
    }
    private void OnSortProjectsByStart(object sender, EventArgs e)
    {
        var sorted = _projectenVM.OrderBy(p => p.Startdatum).ToList();
        _projectenVM.Clear();
        foreach (var item in sorted)
            _projectenVM.Add(item);
    }
    private void OnSortProjectsByEnd(object sender, EventArgs e)
    {
        var sorted = _projectenVM.OrderBy(p => p.Einddatum).ToList();
        _projectenVM.Clear();
        foreach (var item in sorted)
            _projectenVM.Add(item);
    }
    private async void OnDeleteProjectClicked(object sender, EventArgs e)
    {
        var button = sender as Button;
        var projectVM = button?.CommandParameter as ProjectViewModel;
        if (projectVM == null)
            return;

        bool bevestigen = await Application.Current.MainPage.DisplayAlert("Bevestig", "Weet je zeker dat je dit project wilt verwijderen?", "Ja", "Nee");
        if (!bevestigen)
            return;

        if (int.TryParse(projectVM.Id.Replace("#", ""), out int id))
        {
            var project = await _databaseService.GetProjectByIdAsync(id);
            if (project != null)
            {
                await _databaseService.DeleteProjectAsync(project);

                var item = _projectenVM.FirstOrDefault(p => p.Id == projectVM.Id);
                if (item != null)
                    _projectenVM.Remove(item);

                await Application.Current.MainPage.DisplayAlert("Succes", "Project verwijderd", "OK");
            }
        }
    }
    private void OnCancelAddProjectClicked(object sender, EventArgs e)
    {
        AddProjectPopup.IsVisible = false;
        _isEditingProject = false;
        _editingProject = null;
        ConfirmAddProjectButton.Text = "Bevestigen";
    }
    private void OnSortCompaniesByEmail(object sender, EventArgs e)
    {
        var sorted = _bedrijvenVM.OrderBy(b => b.Email).ToList();
        _bedrijvenVM.Clear();
        foreach (var item in sorted)
            _bedrijvenVM.Add(item);
    }

    private void OnSortCompaniesByPhone(object sender, EventArgs e)
    {
        var sorted = _bedrijvenVM.OrderBy(b => b.Telefoonnummer).ToList();
        _bedrijvenVM.Clear();
        foreach (var item in sorted)
            _bedrijvenVM.Add(item);
    }

    private async void OnConfirmAddProjectClicked(object sender, EventArgs e)
    {
        // Voor nu simpele validatie
        if (string.IsNullOrWhiteSpace(ProjectNameEntry.Text) ||
            string.IsNullOrWhiteSpace(ProjectStatusEntry.Text) ||
            string.IsNullOrWhiteSpace(ProjectStartEntry.Text) ||
            string.IsNullOrWhiteSpace(ProjectEndEntry.Text))
        {
            await Application.Current.MainPage.DisplayAlert("Fout", "Vul alle velden in", "OK");
            return;
        }

        var nieuwProject = new Project
        {
            projectnaam = ProjectNameEntry.Text.Trim(),
            status = ProjectStatusEntry.Text.Trim(),
            startdatum = ProjectStartEntry.Text.Trim(),
            einddatum = ProjectEndEntry.Text.Trim()
        };

        await _databaseService.AddProjectAsync(nieuwProject);
        _projectenVM.Add(ViewModelMapper.ToViewModel(nieuwProject));
        AddProjectPopup.IsVisible = false;

        await Application.Current.MainPage.DisplayAlert("Succes", "Project toegevoegd", "OK");
    }
    private void OnAddCompanyClicked(object sender, EventArgs e)
    {
        // Reset form
        CompanyNameEntry.Text = string.Empty;
        CompanyContactEntry.Text = string.Empty;
        CompanyEmailEntry.Text = string.Empty;
        CompanyPhoneEntry.Text = string.Empty;

        CompanyNameErrorLabel.IsVisible = false;
        CompanyEmailErrorLabel.IsVisible = false;

        _isEditingCompany = false;
        _editingCompany = null;

        CompanyPopupTitle.Text = "Nieuw bedrijf toevoegen";
        ConfirmAddCompanyButton.Text = "Bevestigen";
        ConfirmAddCompanyButton.IsEnabled = true;

        AddCompanyPopup.IsVisible = true;
    }
    private void OnCompanySearchChanged(object sender, TextChangedEventArgs e)
    {
        var term = e.NewTextValue?.Trim().ToLower() ?? string.Empty;

        var gefilterd = _bedrijvenVM
            .Where(b => (b.Naam?.ToLower().Contains(term) ?? false) ||
                        (b.Contactpersoon?.ToLower().Contains(term) ?? false))
            .ToList();

        CompaniesCollection.ItemsSource = gefilterd;
    }
    private async void OnDeleteCompanyClicked(object sender, EventArgs e)
    {
        if (sender is not Button button)
            return;

        var company = button.CommandParameter as BedrijfViewModel;
        if (company == null)
            return;

        bool bevestigen = await Application.Current.MainPage.DisplayAlert(
            "Bedrijf verwijderen",
            $"Weet je zeker dat je het bedrijf '{company.Naam}' wilt verwijderen?",
            "Verwijderen", "Annuleren");

        if (!bevestigen)
            return;

        if (int.TryParse(company.Id.Replace("#", ""), out int bedrijfId))
        {
            var allCompanies = await _databaseService.GetBedrijvenAsync();
            var bedrijf = allCompanies.FirstOrDefault(b => b.bedrijf_id == bedrijfId);

            if (bedrijf != null)
            {
                await _databaseService.DeleteBedrijfAsync(bedrijf);
                var item = _bedrijvenVM.FirstOrDefault(b => b.Id == company.Id);
                if (item != null)
                    _bedrijvenVM.Remove(item);

                await Application.Current.MainPage.DisplayAlert("Succes", "Bedrijf verwijderd", "OK");
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Fout", "Bedrijf niet gevonden", "OK");
            }
        }
    }
    private void OnEditProjectClicked(object sender, EventArgs e)
    {
        if (sender is not Button button) return;
        var project = button.CommandParameter as ProjectViewModel;
        if (project == null) return;

        _isEditingProject = true;
        _editingProject = project;

        ProjectPopupTitle.Text = "Project wijzigen";
        ConfirmAddProjectButton.Text = "Opslaan";

        ProjectNameEntry.Text = project.Naam;
        ProjectStatusEntry.Text = project.Status;
        ProjectStartEntry.Text = project.Startdatum;
        ProjectEndEntry.Text = project.Einddatum;

        AddProjectPopup.IsVisible = true;
    }
    private void OnEditCompanyClicked(object sender, EventArgs e)
    {
        if (sender is not Button button) return;
        var bedrijf = button.CommandParameter as BedrijfViewModel;
        if (bedrijf == null) return;

        _isEditingCompany = true;
        _editingCompany = bedrijf;

        CompanyPopupTitle.Text = "Bedrijf wijzigen";
        ConfirmAddCompanyButton.Text = "Opslaan";

        CompanyNameEntry.Text = bedrijf.Naam;
        CompanyContactEntry.Text = bedrijf.Contactpersoon;
        CompanyEmailEntry.Text = bedrijf.Email;
        CompanyPhoneEntry.Text = bedrijf.Telefoonnummer;

        AddCompanyPopup.IsVisible = true;
    }

}





