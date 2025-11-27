using System.Collections.ObjectModel;
using Microsoft.Extensions.Logging;
using Urenregistratie_Applicatie.Models;
using Urenregistratie_Applicatie.Services;

namespace Urenregistratie_Applicatie.Views;

public partial class Page3 : ContentView
{
    private readonly ObservableCollection<UserAccount> _allUsers = new();
    private readonly ObservableCollection<UserAccount> _visibleUsers = new();
    private readonly UserService _userService = new();
    private readonly ILogger<Page3> _logger;

    private string _searchTerm = string.Empty;
    private string _sortColumn = nameof(UserAccount.LastName);
    private bool _sortAscending = true;
    private bool _isAdminUser = true;

    public Page3()
    {
        InitializeComponent();

        UsersCollection.ItemsSource = _visibleUsers;
        _logger = LoggerFactory.Create(builder => builder.AddDebug()).CreateLogger<Page3>();
    }

    private async void OnLoaded(object? sender, EventArgs e)
    {
        await InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        AdminContent.IsVisible = _isAdminUser;
        AccessDeniedCard.IsVisible = !_isAdminUser;

        if (!_isAdminUser)
        {
            return;
        }

        await LoadUsersAsync();
    }

    private async Task LoadUsersAsync(bool simulateFailure = false)
    {
        LoadingIndicator.IsVisible = true;
        LoadingIndicator.IsRunning = true;
        ErrorLabel.IsVisible = false;

        try
        {
            var users = await _userService.GetUsersAsync(simulateFailure);
            _allUsers.Clear();

            foreach (var user in users)
            {
                _allUsers.Add(user);
            }

            ApplyFiltersAndSorting();
            StatusLabel.Text = $"Bijgewerkt op {DateTime.Now:dd-MM-yyyy HH:mm}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Fout bij ophalen van gebruikers.");
            ErrorLabel.IsVisible = true;
            ErrorLabel.Text = "Er is iets misgegaan bij het laden van de gebruikers. Probeer het later opnieuw.";
            _visibleUsers.Clear();
            EmptyStateLabel.IsVisible = false;
        }
        finally
        {
            LoadingIndicator.IsRunning = false;
            LoadingIndicator.IsVisible = false;
        }
    }

    private void OnSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        _searchTerm = e.NewTextValue ?? string.Empty;
        ApplyFiltersAndSorting();
    }

    private void OnSortByFirstName(object sender, EventArgs e) => ApplySorting(nameof(UserAccount.FirstName));

    private void OnSortByLastName(object sender, EventArgs e) => ApplySorting(nameof(UserAccount.LastName));

    private void OnSortByEmail(object sender, EventArgs e) => ApplySorting(nameof(UserAccount.Email));

    private void OnSortByRole(object sender, EventArgs e) => ApplySorting(nameof(UserAccount.Role));

    private void OnSortByLastLogin(object sender, EventArgs e) => ApplySorting(nameof(UserAccount.LastLogin));

    private void OnSortById(object sender, EventArgs e) => ApplySorting(nameof(UserAccount.Id));

    private async void OnRefreshClicked(object sender, EventArgs e)
    {
        await LoadUsersAsync();
    }

    private void OnResetFiltersClicked(object sender, EventArgs e)
    {
        _searchTerm = string.Empty;
        UserSearchBar.Text = string.Empty;
        ApplySorting(_sortColumn, resetDirection: true);
    }

    private void ApplySorting(string sortColumn, bool resetDirection = false)
    {
        if (_sortColumn == sortColumn && !resetDirection)
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

    private void ApplyFiltersAndSorting()
    {
        if (!_isAdminUser)
        {
            return;
        }

        IEnumerable<UserAccount> filtered = _allUsers;

        if (!string.IsNullOrWhiteSpace(_searchTerm))
        {
            filtered = filtered.Where(MatchesSearchTerm);
        }

        filtered = SortUsers(filtered);
        UpdateVisibleUsers(filtered.ToList());

        EmptyStateLabel.IsVisible = !_visibleUsers.Any() && !ErrorLabel.IsVisible;
    }

    private bool MatchesSearchTerm(UserAccount user)
    {
        var term = _searchTerm.Trim();

        if (string.IsNullOrWhiteSpace(term))
        {
            return true;
        }

        return user.FirstName.Contains(term, StringComparison.OrdinalIgnoreCase)
               || user.LastName.Contains(term, StringComparison.OrdinalIgnoreCase)
               || user.FullName.Contains(term, StringComparison.OrdinalIgnoreCase)
               || user.Email.Contains(term, StringComparison.OrdinalIgnoreCase);
    }

    private IEnumerable<UserAccount> SortUsers(IEnumerable<UserAccount> users)
    {
        Func<UserAccount, object?> keySelector = _sortColumn switch
        {
            nameof(UserAccount.FirstName) => user => user.FirstName,
            nameof(UserAccount.LastName) => user => user.LastName,
            nameof(UserAccount.Email) => user => user.Email,
            nameof(UserAccount.Role) => user => user.Role,
            nameof(UserAccount.LastLogin) => user => user.LastLogin,
            nameof(UserAccount.Id) => user => user.Id,
            _ => user => user.LastName
        };

        return _sortAscending
            ? users.OrderBy(keySelector).ToList()
            : users.OrderByDescending(keySelector).ToList();
    }

    private void UpdateVisibleUsers(IReadOnlyCollection<UserAccount> users)
    {
        _visibleUsers.Clear();

        foreach (var user in users)
        {
            _visibleUsers.Add(user);
        }
    }

    private void UpdateSortHeaderLabels()
    {
        FirstNameHeader.Text = BuildSortLabel("Voornaam", nameof(UserAccount.FirstName));
        LastNameHeader.Text = BuildSortLabel("Achternaam", nameof(UserAccount.LastName));
        EmailHeader.Text = BuildSortLabel("E-mail", nameof(UserAccount.Email));
        RoleHeader.Text = BuildSortLabel("Rol", nameof(UserAccount.Role));
        LastLoginHeader.Text = BuildSortLabel("Laatste login", nameof(UserAccount.LastLogin));
        UserIdHeader.Text = BuildSortLabel("Gebruikers-ID", nameof(UserAccount.Id));
    }

    private string BuildSortLabel(string label, string column)
    {
        if (_sortColumn != column)
        {
            return label;
        }

        var arrow = _sortAscending ? "▲" : "▼";
        return $"{label} {arrow}";
    }
}
