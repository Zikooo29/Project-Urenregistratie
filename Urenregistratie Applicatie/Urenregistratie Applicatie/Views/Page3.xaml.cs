using System.Collections.ObjectModel;
using Microsoft.Extensions.Logging;
using Urenregistratie_Applicatie.Models;
using Urenregistratie_Applicatie.Services;

namespace Urenregistratie_Applicatie.Views;


public partial class Page3 : ContentView
{
    // Ophalen uit opslag voor alle gebruikers  en wat zichtbaar is op het scherm
    private readonly ObservableCollection<UserAccount> _allUsers = new();
    private readonly ObservableCollection<UserAccount> _visibleUsers = new();

    // Uit service mock-data
    private readonly UserService _userService = new();

    // Zoeken/sorteer instellingen
    private string _searchTerm = string.Empty;
    private string _sortColumn = nameof(UserAccount.Id);
    private bool _sortAscending = true;

    // Admin toegangscontrole, true = toegang tot de scherm, false = geen toegang
    private bool _isAdminUser = true;

    private readonly string _currentUserId = "#2"; // simulatie van ingelogde gebruiker

    public Page3()
    {
        InitializeComponent();

        // UI lijst koppelen aan zichtbare lijst
        UsersCollection.ItemsSource = _visibleUsers;
    }

    // Bij laden van de pagina: gebruikersscherm tonen en users laden
    private async void OnLoaded(object? sender, EventArgs e)
    {
        await InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        AdminContent.IsVisible = _isAdminUser;
        AccessDeniedCard.IsVisible = !_isAdminUser;

        if (!_isAdminUser)
            return;

        await LoadUsersAsync();
    }

    // Gebruikers uit service ophalen en in UI zetten
    private async Task LoadUsersAsync(bool simulateFailure = false)
    {
        var users = await _userService.GetUsersAsync(simulateFailure);

        _allUsers.Clear();
        foreach (var user in users)
            _allUsers.Add(user);

        ApplyFiltersAndSorting();
    }

    // Word aangeroepen zodra de tekst in de zoekbalk veranderd/ingevuld word en past filter meteen toe.
    private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        _searchTerm = e.NewTextValue ?? string.Empty;
        ApplyFiltersAndSorting();
    }


    // functie Zoeken en sorteren, de lijst met gebruikers filteren op basis van de zoekterm
    private void ApplyFiltersAndSorting()
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

    // Zoeken op voornaam, achternaam of e-mail
    private bool MatchesSearchTerm(UserAccount user)
    {
        var term = _searchTerm.Trim();
        if (string.IsNullOrWhiteSpace(term))
            return true;

        return user.FirstName.Contains(term, StringComparison.OrdinalIgnoreCase)
            || user.LastName.Contains(term, StringComparison.OrdinalIgnoreCase)
            || user.Email.Contains(term, StringComparison.OrdinalIgnoreCase);
    }

    // De sorteer functie 
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


    // Event-handlers die de sortering binnen de columns regelen
    private void OnSortByFirstName(object sender, EventArgs e) => ApplySorting(nameof(UserAccount.FirstName));
    private void OnSortByLastName(object sender, EventArgs e) => ApplySorting(nameof(UserAccount.LastName));
    private void OnSortByEmail(object sender, EventArgs e) => ApplySorting(nameof(UserAccount.Email));
    private void OnSortByRole(object sender, EventArgs e) => ApplySorting(nameof(UserAccount.Role));
    private void OnSortById(object sender, EventArgs e) => ApplySorting(nameof(UserAccount.Id));


    // Sorteer op basis van gekozen kolom en richting
    private IEnumerable<UserAccount> SortUsers(IEnumerable<UserAccount> users)
    {
        if (_sortColumn == nameof(UserAccount.FirstName))
        {
            return _sortAscending
                ? users.OrderBy(u => u.FirstName)
                : users.OrderByDescending(u => u.FirstName);
        }

        if (_sortColumn == nameof(UserAccount.LastName))
        {
            return _sortAscending
                ? users.OrderBy(u => u.LastName)
                : users.OrderByDescending(u => u.LastName);
        }

        if (_sortColumn == nameof(UserAccount.Email))
        {
            return _sortAscending
                ? users.OrderBy(u => u.Email)
                : users.OrderByDescending(u => u.Email);
        }

        if (_sortColumn == nameof(UserAccount.Role))
        {
            return _sortAscending
                ? users.OrderBy(u => u.Role)
                : users.OrderByDescending(u => u.Role);
        }

        if (_sortColumn == nameof(UserAccount.Id))
        {
            return _sortAscending
                ? users.OrderBy(u => u.Id)
                : users.OrderByDescending(u => u.Id);
        }

        return _sortAscending
            ? users.OrderBy(u => u.Id)
            : users.OrderByDescending(u => u.Id);
    }


    // Zorgt ervoor dat de zichtbare lijst ook visueel wordt bijgewerkt (dus niet alleen in geheugen)
    private void UpdateVisibleUsers(IReadOnlyCollection<UserAccount> users)
    {
        _visibleUsers.Clear();
        foreach (var user in users)
            _visibleUsers.Add(user);
    }

    // Deze functie zet bij elke kolomheader de juiste tekst en plaatst een sorteer pijltje (▲ of ▼) bij de kolom waarop op dat moment gesorteerd wordt.
    private void UpdateSortHeaderLabels()
    {
        FirstNameHeader.Text = BuildSortLabel("Voornaam", nameof(UserAccount.FirstName));
        LastNameHeader.Text = BuildSortLabel("Achternaam", nameof(UserAccount.LastName));
        EmailHeader.Text = BuildSortLabel("E-mail", nameof(UserAccount.Email));
        RoleHeader.Text = BuildSortLabel("Rol", nameof(UserAccount.Role));
        UserIdHeader.Text = BuildSortLabel("Gebruikers-ID", nameof(UserAccount.Id));
    }

    //Deze functie voegt een sorteer-pijltje (▲ of ▼) toe aan de kolomnaam als die kolom op dat moment gebruikt wordt om te sorteren.
    private string BuildSortLabel(string kolomNaam, string kolomPropertyNaam)
    {
        if (_sortColumn != kolomPropertyNaam)
            return kolomNaam;

        string pijltje = _sortAscending ? "▲" : "▼";

        return kolomNaam + " " + pijltje;
    }

    // Tijdelijke knoppen voor gebruikers toevoegen, wijzigen en gebruikers verwijderen.
    private async void OnAddUsersClicked(object sender, EventArgs e) =>
        await Application.Current.MainPage.DisplayAlert("Melding", "Tijdelijke actie toevoegen gebruiker", "Kaas");
    private async void OnEditUsersClicked(object sender, EventArgs e) =>
        await Application.Current.MainPage.DisplayAlert("Melding", "Tijdelijke actie gebruiker wijzingen", "Pepernoten");
    private async void OnDeleteUsersClicked(object sender, EventArgs e)
    {
        if (sender is not Button button)
            return;

        // Haal de bijbehorende user uit de CommandParameter
        var user = button.CommandParameter as UserAccount;
        if (user is null)
            return;

        // Geen self-delete door admin
        if (user.Id == _currentUserId)
        {
            await Application.Current.MainPage.DisplayAlert(
                "Niet toegestaan",
                "Je kunt je eigen account niet verwijderen.",
                "OK");
            return;
        }

        // 1. Bevestigingsmodal tonen
        bool bevestigen = await Application.Current.MainPage.DisplayAlert(
            "Gebruiker verwijderen",
            $"Weet je zeker dat je {user.FirstName} {user.LastName} wilt verwijderen?",
            "Verwijderen",
            "Annuleren");

        // 2. Bij annuleren: niets doen
        if (!bevestigen)
            return;

        // 3. Verwijder gebruiker via service
        await _userService.DeleteUserAsync(user.Id);

        // 4. Verwijder uit de lokale lijsten en UI direct verversen
        var userInAll = _allUsers.FirstOrDefault(u => u.Id == user.Id);
        if (userInAll is not null)
        {
            _allUsers.Remove(userInAll);
        }

        // Dit herbouwt _visibleUsers o.b.v. filters & sortering
        ApplyFiltersAndSorting();

        // 5. Succesmelding
        await Application.Current.MainPage.DisplayAlert(
            "Succes",
            "De gebruiker is verwijderd uit het overzicht.",
            "OK");
    }

}
