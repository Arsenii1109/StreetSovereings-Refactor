using System.Windows;
using StreetSovereings.Game;

namespace StreetSovereings.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    private void StartGame(object sender, RoutedEventArgs e)
    {
        Close();
        Program.Main();
    }
        
    private void QuitGame(object sender, RoutedEventArgs e)
    {
        Close();
    }

    private void OpenSettings(object sender, RoutedEventArgs e)
    {
        SettingsWindow settingsWindow = new SettingsWindow();
        settingsWindow.Show();
    }
}