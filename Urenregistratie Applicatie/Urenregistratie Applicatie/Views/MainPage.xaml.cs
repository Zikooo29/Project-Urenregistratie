using Urenregistratie_Applicatie.Views;

namespace Urenregistratie_Applicatie.Views;

public partial class MainPage : ContentPage
{
    private readonly Page3 _page3;
    private readonly Page1 _page1;
    private readonly Page2 _page2;
    private readonly View _defaultContent;

    public MainPage(Page1 page1, Page2 page2, Page3 page3)
    {
        InitializeComponent();

        _page1 = page1;
        _page2 = page2;
        _page3 = page3;

        _defaultContent = SubPage.Content;
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

        SubPage.Content = _page1;
        HeaderTitle.Text = "Page 1";
        this.Title = "MijnUren - Page 1";
    }

    //Pagina 2 knop
    private void OnPage2Clicked(object sender, EventArgs e)
    {
        
        SubPage.Content = _page2;
        HeaderTitle.Text = "Page 2";
        this.Title = "Urenoverzicht - Page 2";
    }
    //Pagina 3 knop
    private void OnPage3Clicked(object sender, EventArgs e)
    {

        SubPage.Content = _page3;
        HeaderTitle.Text = "Gebruikersoverzict";
        this.Title = "Gebruikers";
    }
}