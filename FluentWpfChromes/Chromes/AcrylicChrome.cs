using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Interop;

namespace FluentWpfChromes
{

    /// <inheritdoc cref="ChromeBase" />
    /// <summary>
    ///  Sets Blur effect with Acrylic Colors on Window.
    /// </summary>
    public class AcrylicChrome : ChromeBase, IDelayed
    {
        private static readonly Type OwnerType = typeof(AcrylicChrome);

        public int DragDelay { get; set; } = 10;
        public int ResizeDelay { get; set; } = 30;
        public bool SuppressLagging { get; set; } = true;

        #region Attached property AcrylicChrome
        [DefaultValue("Null")]
        public static readonly DependencyProperty AcrylicChromeProperty =
            DependencyProperty.RegisterAttached(
                "AcrylicChrome",
                OwnerType,
                OwnerType,
                new FrameworkPropertyMetadata(null, AcrylicChromePropertyChanged, AcrylicChromeCoerceValue));

        [AttachedPropertyBrowsableForType(typeof(Window))]
        public static AcrylicChrome GetAcrylicChrome(Window obj)
        {
            return (AcrylicChrome)obj.GetValue(AcrylicChromeProperty);
        }

        [AttachedPropertyBrowsableForType(typeof(Window))]
        public static void SetAcrylicChrome(Window obj, AcrylicChrome value)
        {
            obj.SetValue(AcrylicChromeProperty, value);
        }

        public static object AcrylicChromeCoerceValue(DependencyObject d, object baseValue)
        {

            if (DesignerProperties.GetIsInDesignMode(d))
            {
                return null;
            }

            if (baseValue == null) return null;

            if (!(d is Window)) throw new ArgumentException("d must be Window");

            if (!(baseValue is AcrylicChrome ch))
                throw new ArgumentException("baseValue must be AcrylicChrome");

            if (!ch.AllowStartUpFrozen && ch.IsFrozen)
            {
                return ch.CloneCurrentValue();
            }
            return baseValue;
        }

        [AttachedPropertyBrowsableForType(typeof(Window))]
        private static void AcrylicChromePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (DesignerProperties.GetIsInDesignMode(d))
            {
                return;
            }

            if (!(d is Window window)) throw new ArgumentException("d must be Window");
            var ch = e.NewValue as AcrylicChrome;
            if (ch == null) throw new ArgumentException("AeroGlass type expected ");

            ch.OwnerWindow = window;

            //associate initializer to window
            if (ch.SuppressLagging)
            {
                SetAcrylicInitializer(ch, window);
            }
            else
            {
                SetChromeInitializer(ch, window);
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

        private static void SetAcrylicInitializer(AcrylicChrome ch, Window window)
        {
            var initializer = AcrylicInitializer.GetAcrylicInitializer(window);
            if (initializer == null)
            {
                initializer = new AcrylicInitializer { ChromeBase = ch }; //initialize source
                AcrylicInitializer.SetAcrylicInitializer(window, initializer);
            }
            else
            {
                initializer.ChromeBase = ch;
            }
        }

        private static void SetChromeInitializer(AcrylicChrome ch, Window window)
        {
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

        /// <summary>
        /// Window Loaded handler
        /// </summary>
        private static readonly RoutedEventHandler WindowLoadedHandler = (sender, args) =>
        {
            if (!(sender is Window w)) return;

            if (!(w.GetValue(AcrylicChromeProperty) is AcrylicChrome ch)) return;

            ch.IsInitialized = true;

            ch.OwnerWindow = w;
            ch.HWndSource = (HwndSource)PresentationSource.FromVisual(w);

            var blurDesc =
                DependencyPropertyDescriptor.FromProperty(IsBlurredProperty, typeof(AcrylicChrome));
            blurDesc.RemoveValueChanged(w, IsBlurredPropertyChanged);
            blurDesc.AddValueChanged(w, IsBlurredPropertyChanged);

            var isBlur = (bool)(w.GetValue(IsBlurredProperty)) || (bool)(ch.GetValue(IsBlurredProperty));

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

            if (!(w.GetValue(AcrylicChromeProperty) is AcrylicChrome ch))
                throw new ArgumentException("sender must be Window or AcrylicChrome");

            if (ch.HWndSource == null) return;

            var isBlurred = (bool)w.GetValue(IsBlurredProperty);//(bool)e.NewValue;
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
            if (IsGlassBlurSupported())
            {
                AeroGlassBlurrier.EnableBlur(hWnd, AccentState.ACCENT_ENABLE_ACRYLICBLURBEHIND, gbrColor);
            }
            else
            {
                VistaGlassBlurrier.ExtendGlassFrame(hWnd,
                    new Thickness(-1)); //Check system Framework brfore compiling !!!
            }
        }

        /// <summary>
        /// Check if the OS support Aero Glass Blur.
        /// More version details could be checked here:
        /// https://docs.microsoft.com/en-us/windows/win32/sysinfo/operating-system-version
        /// </summary>
        private bool IsGlassBlurSupported()
        {
            var version = Environment.OSVersion.Version;
            return (version.Major == 6 && version.Minor > 1) || version.Major >= 10;
        }

        private bool atFirstTime = true;

        /// <inheritdoc />
        /// <summary>
        /// Turns off Blur effect, sets undderstratum color
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
