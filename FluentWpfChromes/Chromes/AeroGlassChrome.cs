using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Interop;

namespace FluentWpfChromes
{
    /// <summary>
    ///  Sets Blur effect on Window.
    /// </summary>
    public class AeroGlassChrome : ChromeBase
    {
        private static readonly Type OwnerType = typeof(AeroGlassChrome);

        private bool atFirstTime = true;

        #region Attached property AeroGlassChrome

        [DefaultValue("Null")] public static readonly DependencyProperty AeroGlassChromeProperty =
            DependencyProperty.RegisterAttached(
                "AeroGlassChrome",
                OwnerType,
                OwnerType,
                new FrameworkPropertyMetadata(null, AeroGlassChromePropertyChanged, AeroGlassChromeCoerceValue));

        [AttachedPropertyBrowsableForType(typeof(Window))]
        public static AeroGlassChrome GetAeroGlassChrome(Window obj)
        {
            return (AeroGlassChrome) obj.GetValue(AeroGlassChromeProperty);
        }

        [AttachedPropertyBrowsableForType(typeof(Window))]
        public static void SetAeroGlassChrome(Window obj, AeroGlassChrome value)
        {
            obj.SetValue(AeroGlassChromeProperty, value);
        }

        public static object AeroGlassChromeCoerceValue(DependencyObject d, object baseValue)
        {

            if (DesignerProperties.GetIsInDesignMode(d))
            {
                return null;
            }

            if (baseValue == null) return null;

            if (!(d is Window)) throw new ArgumentException("d must be Window");

            if (!(baseValue is AeroGlassChrome ch))
                throw new ArgumentException("baseValue must be AeroGlassChrome");

            if (!ch.AllowStartUpFrozen && ch.IsFrozen)
            {
                return ch.CloneCurrentValue();
            }

            return baseValue;
        }

        [AttachedPropertyBrowsableForType(typeof(Window))]
        private static void AeroGlassChromePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(d))
            {
                return;
            }

            if (!(d is Window window)) throw new ArgumentException("d must be Window");
            var ch = e.NewValue as AeroGlassChrome;
            if (ch == null) throw new ArgumentException("AeroGlass type expected ");

            ch.OwnerWindow = window;

            //associate initializer to window
            var initializer = ChromeInitializer.GetChromeInitializer(window);
            if (initializer == null)
            {
                initializer = new ChromeInitializer() {ChromeBase = ch}; //initialize source
                ChromeInitializer.SetChromeInitializer(window, initializer);
            }
            else
            {
                initializer.ChromeBase = ch;
            }


            if (!window.IsLoaded)
            {
                window.Loaded += WindowLoadedHandler;
            }
            else
            {
                if (!ch.IsInitialized)
                    WindowLoadedHandler.Invoke(window, null);

            }


        }

        #endregion

        /// <summary>
        /// Window Loaded handler
        /// </summary>
        private static readonly RoutedEventHandler WindowLoadedHandler = (sender, args) =>
        {
            if (!(sender is Window w)) return;

            if (!(w.GetValue(AeroGlassChromeProperty) is AeroGlassChrome ch)) return;

            ch.IsInitialized = true;

            ch.OwnerWindow = w;
            ch.HWndSource = (HwndSource) PresentationSource.FromVisual(w);

            var blurDesc =
                DependencyPropertyDescriptor.FromProperty(IsBlurredProperty, typeof(AeroGlassChrome));
            blurDesc.RemoveValueChanged(w, IsBlurredPropertyChanged);
            blurDesc.AddValueChanged(w, IsBlurredPropertyChanged);


            var isBlur = (bool) (w.GetValue(IsBlurredProperty)) || (bool) (ch.GetValue(IsBlurredProperty));

            if (!isBlur) ch.DisableBlur();
            else ch.EnableBlur();

            w.Loaded -= WindowLoadedHandler;
        };

        /// <summary>
        /// PropertyChanged Callback for Windows object
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void IsBlurredPropertyChanged(object sender, EventArgs e)
        {
            if (!(sender is Window w)) return;

            if (!(w.GetValue(AeroGlassChromeProperty) is AeroGlassChrome ch))
                throw new ArgumentException("d must be Window or AeroGlassChrome");

            if (ch.HWndSource == null) return;

            var isBlurred = (bool) w.GetValue(IsBlurredProperty); //(bool)e.NewValue;
            if (isBlurred)
            {
                ch.EnableBlur();
            }
            else
            {
                ch.DisableBlur();
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Sets blur effect to current window
        /// </summary>
        public override void EnableBlur()
        {
            var hWnd = HWndSource.Handle;

            if (Environment.OSVersion.Version.Major >= 6)
            {
                if (Environment.OSVersion.Version.Minor > 1)
                    AeroGlassBlurrier.EnableBlur(hWnd);
                else
                    VistaGlassBlurrier.ExtendGlassFrame(hWnd,
                        new Thickness(-1));
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Turns off Blur effect, sets undderstatum color
        /// </summary>
        public override void DisableBlur()
        {
            #region windows BUG fix
            if (atFirstTime) //windows BUG fix
            {
                atFirstTime = false;
                DisableBlur();
                EnableBlur();
            }
            #endregion

            var hWnd = HWndSource.Handle;

            if (Environment.OSVersion.Version.Major >= 6)
            {
                if (Environment.OSVersion.Version.Minor > 1)
                    AeroGlassBlurrier.DisableBlur(hWnd, gbrColor);
                else
                {
                    VistaGlassBlurrier.DisableBlurFrame(hWnd);
                }

            }

        }
    }


}
