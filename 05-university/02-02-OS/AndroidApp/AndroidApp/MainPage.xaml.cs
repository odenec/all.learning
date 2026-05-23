namespace AndroidApp;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
    }


    private void OnCalculate(object sender, EventArgs e)
    {

        double.TryParse(FirstInput.Text, out double num1);


        double.TryParse(SecondInput.Text, out double num2);


        double sum = num1 + num2;


        ResultLabel.Text = "Результат: " + sum.ToString();
    }
}