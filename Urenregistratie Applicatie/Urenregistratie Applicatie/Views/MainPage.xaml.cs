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

    // Menu knop
    private void OnMenuButtonClicked(object sender, EventArgs e)
    {
        SideMenu.IsVisible = !SideMenu.IsVisible;
    }

    // Home knop
    private void OnMainMenuClicked(object sender, EventArgs e)
    {
        SideMenu.IsVisible = false;
        SubPage.Content = _defaultContent;

    }

    //Pagina 1 knop
    private void OnPage1Clicked(object sender, EventArgs e)
    {
        SideMenu.IsVisible = false;
        SubPage.Content = new Page1();
    }

    //Pagina 2 knop
    private void OnPage2Clicked(object sender, EventArgs e)
    {
        SideMenu.IsVisible = false;
        SubPage.Content = new Page2();
    }
}