# Gebruikersoverzicht bestanden (kopieer-en-plak)

Hieronder staan de relevante bestanden uit het gebruikersoverzicht uitgeschreven zodat je ze direct kunt kopiëren en plakken.

## Models/UserAccount.cs
```csharp
namespace Urenregistratie_Applicatie.Models;

public class UserAccount
{
    public string Id { get; init; } = string.Empty;

    public string FirstName { get; init; } = string.Empty;

    public string LastName { get; init; } = string.Empty;

    public string Email { get; init; } = string.Empty;

    public string Role { get; init; } = string.Empty;

    public DateTime LastLogin { get; init; }
        = DateTime.SpecifyKind(DateTime.MinValue, DateTimeKind.Utc);

    public string FullName => $"{FirstName} {LastName}".Trim();
}
```

## Services/UserService.cs
```csharp
using Urenregistratie_Applicatie.Models;

namespace Urenregistratie_Applicatie.Services;

public class UserService
{
    private readonly List<UserAccount> _seedUsers = new()
    {
        new UserAccount
        {
            Id = "USR-001",
            FirstName = "Lotte",
            LastName = "Jansen",
            Email = "lotte.jansen@company.nl",
            Role = "Administrator",
            LastLogin = DateTime.SpecifyKind(DateTime.UtcNow.AddDays(-1), DateTimeKind.Utc)
        },
        new UserAccount
        {
            Id = "USR-002",
            FirstName = "Ruben",
            LastName = "Vermeulen",
            Email = "ruben.vermeulen@company.nl",
            Role = "Manager",
            LastLogin = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(-6), DateTimeKind.Utc)
        },
        new UserAccount
        {
            Id = "USR-003",
            FirstName = "Sara",
            LastName = "de Vries",
            Email = "sara.devries@company.nl",
            Role = "Medewerker",
            LastLogin = DateTime.SpecifyKind(DateTime.UtcNow.AddDays(-3), DateTimeKind.Utc)
        },
        new UserAccount
        {
            Id = "USR-004",
            FirstName = "Finn",
            LastName = "Visser",
            Email = "finn.visser@company.nl",
            Role = "Medewerker",
            LastLogin = DateTime.SpecifyKind(DateTime.UtcNow.AddDays(-10), DateTimeKind.Utc)
        },
        new UserAccount
        {
            Id = "USR-005",
            FirstName = "Nora",
            LastName = "Smit",
            Email = "nora.smit@company.nl",
            Role = "Administrator",
            LastLogin = DateTime.SpecifyKind(DateTime.UtcNow.AddHours(-4), DateTimeKind.Utc)
        },
        new UserAccount
        {
            Id = "USR-006",
            FirstName = "Milan",
            LastName = "Vos",
            Email = "milan.vos@company.nl",
            Role = "Manager",
            LastLogin = DateTime.SpecifyKind(DateTime.UtcNow.AddDays(-30), DateTimeKind.Utc)
        }
    };

    public async Task<IReadOnlyList<UserAccount>> GetUsersAsync(bool simulateFailure = false)
    {
        await Task.Delay(250);

        if (simulateFailure)
        {
            throw new HttpRequestException("Gebruikersservice is niet beschikbaar.");
        }

        // Simuleer database query door een nieuwe lijst te retourneren
        return _seedUsers
            .Select(user => new UserAccount
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = user.Role,
                LastLogin = user.LastLogin
            })
            .ToList();
    }
}
```

## Views/Page3.xaml
```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentView xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="Urenregistratie_Applicatie.Views.Page3"
             Loaded="OnLoaded">
    <VerticalStackLayout Padding="20" Spacing="12">
        <Label Text="Gebruikersoverzicht"
               FontSize="22"
               FontAttributes="Bold" />

        <Label Text="Als administratiemedewerker kun je hier alle accounts beheren en controleren."
               TextColor="Gray" />

        <Frame x:Name="AccessDeniedCard"
               IsVisible="False"
               BorderColor="DarkRed"
               BackgroundColor="MistyRose"
               Padding="16">
            <Label Text="Alleen admins hebben toegang tot deze pagina."
                   TextColor="DarkRed"
                   FontAttributes="Bold" />
        </Frame>

        <VerticalStackLayout x:Name="AdminContent" Spacing="12" IsVisible="False">
            <Grid ColumnDefinitions="*,Auto" VerticalOptions="Center">
                <Label Text="Overzicht van alle gebruikers"
                       FontAttributes="Semibold" />
                <HorizontalStackLayout Spacing="8">
                    <Button Text="Ververs lijst"
                            Clicked="OnRefreshClicked"
                            SemanticProperties.Description="Ververs de gebruikerslijst" />
                    <Button Text="Reset filters"
                            Clicked="OnResetFiltersClicked" />
                </HorizontalStackLayout>
            </Grid>

            <SearchBar x:Name="UserSearchBar"
                       Placeholder="Zoek op naam of e-mail"
                       TextChanged="OnSearchTextChanged" />

            <Label x:Name="StatusLabel"
                   FontSize="12"
                   TextColor="Gray" />

            <Label x:Name="ErrorLabel"
                   Text="Er is iets misgegaan bij het laden van de gebruikers."
                   TextColor="DarkRed"
                   FontAttributes="Bold"
                   IsVisible="False" />

            <ActivityIndicator x:Name="LoadingIndicator"
                                IsVisible="False"
                                IsRunning="False"
                                Color="{StaticResource Primary}" />

            <Frame Padding="0">
                <Grid RowDefinitions="Auto,*">
                    <Grid ColumnDefinitions="2*,2*,3*,1.5*,2*,1.5*"
                          Padding="12"
                          BackgroundColor="{AppThemeBinding Light={StaticResource Gray100}, Dark={StaticResource Gray950}}">
                        <Button x:Name="FirstNameHeader"
                                Grid.Column="0"
                                Text="Voornaam"
                                Clicked="OnSortByFirstName"
                                Padding="0"
                                HorizontalOptions="Start"
                                BackgroundColor="Transparent"
                                TextColor="{StaticResource Primary}" />
                        <Button x:Name="LastNameHeader"
                                Grid.Column="1"
                                Text="Achternaam"
                                Clicked="OnSortByLastName"
                                Padding="0"
                                HorizontalOptions="Start"
                                BackgroundColor="Transparent"
                                TextColor="{StaticResource Primary}" />
                        <Button x:Name="EmailHeader"
                                Grid.Column="2"
                                Text="E-mail"
                                Clicked="OnSortByEmail"
                                Padding="0"
                                HorizontalOptions="Start"
                                BackgroundColor="Transparent"
                                TextColor="{StaticResource Primary}" />
                        <Button x:Name="RoleHeader"
                                Grid.Column="3"
                                Text="Rol"
                                Clicked="OnSortByRole"
                                Padding="0"
                                HorizontalOptions="Start"
                                BackgroundColor="Transparent"
                                TextColor="{StaticResource Primary}" />
                        <Button x:Name="LastLoginHeader"
                                Grid.Column="4"
                                Text="Laatste login"
                                Clicked="OnSortByLastLogin"
                                Padding="0"
                                HorizontalOptions="Start"
                                BackgroundColor="Transparent"
                                TextColor="{StaticResource Primary}" />
                        <Button x:Name="UserIdHeader"
                                Grid.Column="5"
                                Text="Gebruikers-ID"
                                Clicked="OnSortById"
                                Padding="0"
                                HorizontalOptions="Start"
                                BackgroundColor="Transparent"
                                TextColor="{StaticResource Primary}" />
                    </Grid>

                    <CollectionView x:Name="UsersCollection"
                                    Grid.Row="1"
                                    ItemsLayout="VerticalList"
                                    SelectionMode="None"
                                    ItemsSource="{Binding}">
                        <CollectionView.EmptyView>
                            <Label x:Name="EmptyStateLabel"
                                   Text="Geen gebruikers gevonden"
                                   HorizontalOptions="Center"
                                   VerticalOptions="Center"
                                   TextColor="Gray" />
                        </CollectionView.EmptyView>
                        <CollectionView.ItemTemplate>
                            <DataTemplate>
                                <Grid ColumnDefinitions="2*,2*,3*,1.5*,2*,1.5*"
                                      Padding="12"
                                      ColumnSpacing="8"
                                      BackgroundColor="{AppThemeBinding Light={StaticResource White}, Dark={StaticResource Black}}">
                                    <Label Grid.Column="0"
                                           Text="{Binding FirstName}" />
                                    <Label Grid.Column="1"
                                           Text="{Binding LastName}" />
                                    <Label Grid.Column="2"
                                           Text="{Binding Email}" />
                                    <Label Grid.Column="3"
                                           Text="{Binding Role}" />
                                    <Label Grid.Column="4"
                                           Text="{Binding LastLogin, StringFormat='{}{0:dd-MM-yyyy HH:mm}'}" />
                                    <Label Grid.Column="5"
                                           Text="{Binding Id}" />
                                </Grid>
                            </DataTemplate>
                        </CollectionView.ItemTemplate>
                    </CollectionView>
                </Grid>
            </Frame>
        </VerticalStackLayout>
    </VerticalStackLayout>
</ContentView>
```

## Views/Page3.xaml.cs
```csharp
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
```

## Views/MainPage.xaml
```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:views="clr-namespace:Urenregistratie_Applicatie.Views"
             x:Class="Urenregistratie_Applicatie.Views.MainPage"
             Title="MijnUren - Hoofdscherm">

    <Grid RowDefinitions="Auto,*">

        <!-- HEADER -->
        <Grid Grid.Row="0"
              Style="{StaticResource HeaderGridStyle}">


            <!-- home button -->
            <Button Grid.Column="0"
                    Text="⌂"
                    Style="{StaticResource HeaderIconButtonStyle}"
                    Clicked="OnMainMenuClicked" />

            <!-- Title Label -->
            <Label x:Name="HeaderTitle"
                   Grid.Column="2"
                   Text="Urenregistratie"
                   Style="{StaticResource HeaderTitleLabelStyle}" />
        </Grid>

        <!-- SIDEBAR + CONTENT -->
        <Grid Grid.Row="1"
              ColumnDefinitions="Auto,*">

            <!-- Alleen Page1, Page2, Page3 -->
            <VerticalStackLayout x:Name="SideMenu"
                                 Style="{StaticResource SideMenuStyle}"
                                 IsVisible="True">

                <Label Text="Menu"
                       Style="{StaticResource SideMenuTitleLabelStyle}" />

                <Button Text="Page 1"
                        Clicked="OnPage1Clicked" />

                <Button Text="Page 2"
                        Clicked="OnPage2Clicked" />

                <Button Text="Gebruikers"
                        Clicked="OnPage3Clicked" />

            </VerticalStackLayout>

            <!-- SubPage  -->
            <ContentView x:Name="SubPage"
                         Grid.Column="1">
                <VerticalStackLayout Padding="20">
                    <Label Text="Welkom Bij Mijn uren"
                           Style="{StaticResource SubPageWelcomeLabelStyle}" />
                </VerticalStackLayout>
            </ContentView>

        </Grid>
    </Grid>

</ContentPage>
```

## Views/MainPage.xaml.cs
```csharp
using Urenregistratie_Applicatie.Views;

namespace Urenregistratie_Applicatie.Views;

public partial class MainPage : ContentPage
{

    private View _defaultContent;
    public MainPage()
    {
        InitializeComponent();

        _defaultContent = SubPage.Content;


        // SubPage.Content = new Page1(); komt hier (is optioneel)
    }



    // Home knop
    private void OnMainMenuClicked(object sender, EventArgs e)
    {

        SubPage.Content = _defaultContent;
        HeaderTitle.Text = "Urenregistratie";
        this.Title = "MijnUren - Hoofdscherm";

    }

    //Pagina 1 knop
    private void OnPage1Clicked(object sender, EventArgs e)
    {

        SubPage.Content = new Page1();
        HeaderTitle.Text = "Page 1";
        this.Title = "MijnUren - Page 1";
    }

    //Pagina 2 knop
    private void OnPage2Clicked(object sender, EventArgs e)
    {

        SubPage.Content = new Page2();
        HeaderTitle.Text = "Page 2";
        this.Title = "MijnUren - Page 2";
    }
    //Pagina 3 knop
    private void OnPage3Clicked(object sender, EventArgs e)
    {

        SubPage.Content = new Page3();
        HeaderTitle.Text = "Gebruikersoverzicht";
        this.Title = "MijnUren - Gebruikers";
    }
}
```
