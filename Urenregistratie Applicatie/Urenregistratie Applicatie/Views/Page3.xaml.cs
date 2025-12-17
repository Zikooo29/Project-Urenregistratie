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

    // Edit state
    private bool _isEditing = false;
    private GebruikerViewModel? _editingUser = null;



    // Zoeken/sorteer instellingen
    private string _searchTerm = string.Empty;
    private string _sortColumn = nameof(GebruikerViewModel.Id);
    private bool _sortAscending = true;
    private bool _isAdminUser = true;
    private readonly string _currentUserId = "2";

    private readonly DatabaseService _databaseService;

    public Page3(DatabaseService databaseService)
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

            if (!string.IsNullOrWhiteSpace(_searchTerm))
            {
                var term = _searchTerm.Trim();
                filtered = filtered.Where(g =>
                    g.voornaam.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    g.achternaam.Contains(term, StringComparison.OrdinalIgnoreCase) ||
                    g.email.Contains(term, StringComparison.OrdinalIgnoreCase));
            }

            filtered = _sortAscending
                ? filtered.OrderBy(g => g.gebruiker_id)
                : filtered.OrderByDescending(g => g.gebruiker_id);

            _visibleUsers.Clear();
            foreach (var g in filtered)
            {
                _visibleUsers.Add(ViewModelMapper.ToViewModel(g));
            }

            EmptyStateLabel.IsVisible = !_visibleUsers.Any();
        }
        catch (Exception ex)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await Application.Current.MainPage.DisplayAlert("Fout bij filteren databasegebruikers", ex.Message, "OK");
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
        else if (EmailValidator.EmailExists(
            email,
            _allDbUsers.Select(u => (u.email, u.gebruiker_id.ToString())),
            _editingUser?.Id))
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
        return EmailValidator.IsValidEmail(email);
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


}
