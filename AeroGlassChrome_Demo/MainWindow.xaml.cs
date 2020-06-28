using System.Windows;
using System.Windows.Input;
using FluentWpfChromes;

namespace AeroGlassChrome_Demo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MainWindow_OnMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (!(sender is Window w)) return;
            if (e.ChangedButton != MouseButton.Left) return;

            var chrome = w.GetValue(AeroGlassChrome.AeroGlassChromeProperty);
            if (chrome == null) return;

            var blur = (bool)(w.GetValue(ChromeBase.IsBlurredProperty));
            w.SetValue(ChromeBase.IsBlurredProperty, !blur);

        }
    }
}
