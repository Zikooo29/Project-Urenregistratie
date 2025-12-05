using Microsoft.Extensions.Logging;
using System.Collections.ObjectModel;
using Urenregistratie_Applicatie.Models;
using Urenregistratie_Applicatie.Services;

namespace Urenregistratie_Applicatie.Views;

public partial class Page3 : ContentView
{
    private readonly ObservableCollection<UserAccount> _allUsers = new();
    private readonly ObservableCollection<UserAccount> _visibleUsers = new();
    private readonly ObservableCollection<Gebruiker> _allDbUsers = new();

    // Edit state
    private bool _isEditing = false;
    private UserAccount? _editingUser = null;

    // Uit service mock-data
   // private readonly UserService _userService = new();

    // Zoeken/sorteer instellingen
    private string _searchTerm = string.Empty;
    private string _sortColumn = nameof(UserAccount.Id);
    private bool _sortAscending = true;
    private bool _isAdminUser = true;
    private readonly string _currentUserId = "2";

    private readonly UserService _userService;
    private readonly DatabaseService _databaseService;

    public Page3(UserService userService, DatabaseService databaseService)
    {
        InitializeComponent();

        _userService = userService;
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

            await LoadUsersAsync();
            await LoadDbUsersAsync();
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Fout bij initialisatie", ex.Message, "OK");
        }
    }

    private async Task LoadUsersAsync(bool simulateFailure = false)
    {
        try
        {
            var users = await _userService.GetUsersAsync(simulateFailure);

            _allUsers.Clear();
            foreach (var user in users)
                _allUsers.Add(user);

            ApplyFiltersAndSorting();
        }
        catch (Exception ex)
        {
            await Application.Current.MainPage.DisplayAlert("Fout bij laden van gebruikers", ex.Message, "OK");
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
            ApplyFiltersAndSorting();
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

    private void ApplyFiltersAndSorting()
    {
        try
        {
            if (!_isAdminUser)
                return;

            IEnumerable<UserAccount> filtered = _allUsers;

            if (!string.IsNullOrWhiteSpace(_searchTerm))
                filtered = filtered.Where(MatchesSearchTerm);

            filtered = SortUsers(filtered);

            UpdateVisibleUsers(filtered.ToList());
            EmptyStateLabel.IsVisible = !_visibleUsers.Any();
        }
        catch (Exception ex)
        {
            MainThread.BeginInvokeOnMainThread(async () =>
            {
                await Application.Current.MainPage.DisplayAlert("Fout bij filteren", ex.Message, "OK");
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
                _visibleUsers.Add(new UserAccount
                {
                    Id = g.gebruiker_id.ToString(),
                    FirstName = g.voornaam,
                    LastName = g.achternaam,
                    Email = g.email,
                    Role = g.rol
                });
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

    private bool MatchesSearchTerm(UserAccount user)
    {
        var term = _searchTerm.Trim();
        if (string.IsNullOrWhiteSpace(term))
            return true;

        return user.FirstName.Contains(term, StringComparison.OrdinalIgnoreCase)
            || user.LastName.Contains(term, StringComparison.OrdinalIgnoreCase)
            || user.Email.Contains(term, StringComparison.OrdinalIgnoreCase);
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
        ApplyFiltersAndSorting();
    }

    private void OnSortByFirstName(object sender, EventArgs e) =>
        ApplySorting(nameof(UserAccount.FirstName));

    private void OnSortByLastName(object sender, EventArgs e) =>
        ApplySorting(nameof(UserAccount.LastName));

    private void OnSortByEmail(object sender, EventArgs e) =>
        ApplySorting(nameof(UserAccount.Email));

    private void OnSortByRole(object sender, EventArgs e) =>
        ApplySorting(nameof(UserAccount.Role));

    private void OnSortById(object sender, EventArgs e) =>
        ApplySorting(nameof(UserAccount.Id));

    private IEnumerable<UserAccount> SortUsers(IEnumerable<UserAccount> users)
    {
        return _sortColumn switch
        {
            nameof(UserAccount.FirstName) => _sortAscending ? users.OrderBy(u => u.FirstName) : users.OrderByDescending(u => u.FirstName),
            nameof(UserAccount.LastName) => _sortAscending ? users.OrderBy(u => u.LastName) : users.OrderByDescending(u => u.LastName),
            nameof(UserAccount.Email) => _sortAscending ? users.OrderBy(u => u.Email) : users.OrderByDescending(u => u.Email),
            nameof(UserAccount.Role) => _sortAscending ? users.OrderBy(u => u.Role) : users.OrderByDescending(u => u.Role),
            nameof(UserAccount.Id) => _sortAscending ? users.OrderBy(u => u.Id) : users.OrderByDescending(u => u.Id),
            _ => _sortAscending ? users.OrderBy(u => u.Id) : users.OrderByDescending(u => u.Id),
        };
    }

    private void UpdateVisibleUsers(IReadOnlyCollection<UserAccount> users)
    {
        _visibleUsers.Clear();
        foreach (var user in users)
            _visibleUsers.Add(user);
    }

    private void UpdateSortHeaderLabels()
    {
        FirstNameHeader.Text = BuildSortLabel("Voornaam", nameof(UserAccount.FirstName));
        LastNameHeader.Text = BuildSortLabel("Achternaam", nameof(UserAccount.LastName));
        EmailHeader.Text = BuildSortLabel("E-mail", nameof(UserAccount.Email));
        RoleHeader.Text = BuildSortLabel("Rol", nameof(UserAccount.Role));
        UserIdHeader.Text = BuildSortLabel("Gebruikers-ID", nameof(UserAccount.Id));
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
        AddUserPopup.IsVisible = true;
    }
    private async void OnEditUsersClicked(object sender, EventArgs e) 
    {

        if (sender is not Button button)
            return;

        // verwacht CommandParameter="{Binding}" in XAML
        var user = button.CommandParameter as UserAccount;
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
        var index = RolePicker.Items?.IndexOf(user.Role) ?? -1;
        RolePicker.SelectedIndex = index >= 0 ? index : -1;

        // knoptekst en validatie
        ConfirmAddUserButton.Text = "Opslaan";
        ConfirmAddUserButton.IsEnabled = false; // Validatie regelt wanneer true

        // toon popup
        AddUserPopup.IsVisible = true;
        
        ValidateAddUserForm();
    }

    private async void OnDeleteUsersClicked(object sender, EventArgs e)
    {
        try
        {
            if (sender is not Button button) return;

            var user = button.CommandParameter as UserAccount;
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
            else
            {
                await _userService.DeleteUserAsync(user.Id);
                var u = _allUsers.FirstOrDefault(x => x.Id == user.Id);
                if (u != null) _allUsers.Remove(u);
                ApplyFiltersAndSorting();
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
    else if (_allUsers.Any(u => u.Email.Equals(email, StringComparison.OrdinalIgnoreCase)
                                            && u.Id != (_editingUser?.Id ?? string.Empty)))
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
        var updatedUser = new UserAccount
        {
            Id = _editingUser.Id,
            FirstName = FirstNameEntry.Text.Trim(),
            LastName = LastNameEntry.Text.Trim(),
            Email = EmailEntry.Text.Trim(),
            Role = RolePicker.SelectedItem?.ToString() ?? (_editingUser.Role ?? string.Empty)
        };

        // Update in service (mock)
        await _userService.UpdateUserAsync(updatedUser);

        // Vervang in lokale lijst (_allUsers) zodat ObservableCollection de UI updatet
        var index = _allUsers.IndexOf(_allUsers.First(u => u.Id == _editingUser.Id));
        if (index >= 0)
        {
            _allUsers[index] = updatedUser;
        }

        // Herbouw zichtbare lijst en sluit popup
        ApplyFiltersAndSorting();

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
    var newUser = new UserAccount
    {
        Id = GenerateNewUserId(),
        FirstName = FirstNameEntry.Text.Trim(),
        LastName = LastNameEntry.Text.Trim(),
        Email = EmailEntry.Text.Trim(),
        Role = RolePicker.SelectedItem?.ToString() ?? "Werknemer"
    };

    await _userService.AddUserAsync(newUser);
    _allUsers.Add(newUser);
    ApplyFiltersAndSorting();

    AddUserPopup.IsVisible = false;

    await Application.Current.MainPage.DisplayAlert(
        "Succes",
        "De gebruiker is succesvol toegevoegd.",
        "OK");
}

// Nieuwe ID maken: pakt hoogste nummer en telt er 1 bij op (#5, #6, etc.)
private string GenerateNewUserId()
{
    int maxId = 0;

    foreach (var user in _allUsers)
    {
        if (!string.IsNullOrWhiteSpace(user.Id) &&
            user.Id.StartsWith("#") &&
            int.TryParse(user.Id.Substring(1), out int num))
        {
            if (num > maxId)
                maxId = num;
        }
    }

    return "#" + (maxId + 1);
}

}

