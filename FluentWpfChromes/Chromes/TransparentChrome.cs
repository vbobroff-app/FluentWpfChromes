using System;
using System.ComponentModel;
using System.Windows;

namespace FluentWpfChromes
{
    public class TransparentChrome : ChromeBase
    {
        private static readonly Type OwnerType = typeof(TransparentChrome);

        #region Attached property TransparentChrome
        [DefaultValue("Null")]
        public static readonly DependencyProperty TransparentChromeProperty =
            DependencyProperty.RegisterAttached(
                "TransparentChrome",
                OwnerType,
                OwnerType,
                new FrameworkPropertyMetadata(null, TransparentChromePropertyChanged, TransparentChromeCoerceValue));

        [AttachedPropertyBrowsableForType(typeof(Window))]
        public static TransparentChrome GetTransparentChrome(Window obj)
        {
            return (TransparentChrome)obj.GetValue(TransparentChromeProperty);
        }

        [AttachedPropertyBrowsableForType(typeof(Window))]
        public static void SetTransparentChrome(Window obj, TransparentChrome value)
        {
            obj.SetValue(TransparentChromeProperty, value);
        }

        public static object TransparentChromeCoerceValue(DependencyObject d, object baseValue)
        {

            if (DesignerProperties.GetIsInDesignMode(d))
            {
                return null;
            }

            if (baseValue == null) return null;

            if (!(d is Window)) throw new ArgumentException("d must be Window");

            if (!(baseValue is TransparentChrome ch))
                throw new ArgumentException("baseValue must be TransparentChrome");

            if (!ch.AllowStartUpFrozen && ch.IsFrozen)
            {
                return ch.CloneCurrentValue();
            }
            return baseValue;
        }

        [AttachedPropertyBrowsableForType(typeof(Window))]
        private static void TransparentChromePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(d))
            {
                return;
            }

            if (!(d is Window window)) throw new ArgumentException("d must be Window");
            var ch = e.NewValue as TransparentChrome;
            if (ch == null) throw new ArgumentException("Transparent type expected ");

            ch.OwnerWindow = window;

            //associate initializer to window
            var initializer = ChromeInitializer.GetChromeInitializer(window);
            if (initializer == null)
            {
                initializer = new ChromeInitializer() { ChromeBase = ch }; //initialize source
                ChromeInitializer.SetChromeInitializer(window, initializer);
            }
            else
            {
                initializer.ChromeBase = ch;
            }

        }
        #endregion

    }
}
