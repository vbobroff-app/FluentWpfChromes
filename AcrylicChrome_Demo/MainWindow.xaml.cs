using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using FluentWpfChromes;

namespace AcrylicChrome_Demo
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

            var chrome = w.GetValue(AcrylicChrome.AcrylicChromeProperty);
            if (chrome == null) return;

            var blur = (bool)(w.GetValue(ChromeBase.IsBlurredProperty));
            w.SetValue(ChromeBase.IsBlurredProperty, !blur); ;
        }
    }





}
