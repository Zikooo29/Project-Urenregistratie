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