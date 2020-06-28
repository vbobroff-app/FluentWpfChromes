using System;
using System.ComponentModel;
using System.Windows;

namespace FluentWpfChromes
{
    /// <inheritdoc />
    /// <summary>
    /// Initialize Window HWndSource by Hook WndProc method
    /// </summary>
    internal sealed class ChromeInitializer : InitializerBase
    {
        private static readonly Type OwnerType = typeof(ChromeInitializer);


        #region Attached property ChromeInitializer

        [DefaultValue("Null")]
        public static readonly DependencyProperty ChromeInitializerProperty =
            DependencyProperty.RegisterAttached(
                "ChromeInitializer",
                OwnerType,
                OwnerType,
                new FrameworkPropertyMetadata(null, ChromeInitializerPropertyChanged));

        [AttachedPropertyBrowsableForType(typeof(Window))]
        public static ChromeInitializer GetChromeInitializer(Window obj)
        {
            return (ChromeInitializer)obj.GetValue(ChromeInitializerProperty);
        }

        [AttachedPropertyBrowsableForType(typeof(Window))]
        public static void SetChromeInitializer(Window obj, ChromeInitializer value)
        {
            obj.SetValue(ChromeInitializerProperty, value);
        }

        [AttachedPropertyBrowsableForType(typeof(Window))]
        private static void ChromeInitializerPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

            if (!(d is Window window)) throw new ArgumentException("d must be Window");
            if(!(e.NewValue is ChromeInitializer ch)) throw new ArgumentNullException();

            ch.Initialize(window);

        }


        #endregion

        // constructor
        public ChromeInitializer()
        {
            HandlersCreate();
        }

    }
}