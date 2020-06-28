using System;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

namespace FluentWpfChromes
{
    /// <inheritdoc />
    /// <summary>
    ///  abstract Chrome class with virtual methods EnableBlur(), DisableBlur()
    /// </summary>
    public abstract class ChromeBase : Freezable
    {
        private static readonly Type OwnerType = typeof(ChromeBase);

        /// <summary>
        /// Current wWindow object
        /// </summary>
        public Window OwnerWindow { set; get; }

        /// <summary>
        /// Handle Source of current Window
        /// </summary>
        public HwndSource HWndSource { set; get; }

        public bool IsInitialized { set; get; }

        protected uint gbrColor = (uint)UsingColors.Transparent;


        #region DependencyProperty ResizeBorderThickness
        public static readonly DependencyProperty ResizeBorderThicknessProperty = DependencyProperty.Register(
            "ResizeBorderThickness",
            typeof(Thickness),
            OwnerType,
            new PropertyMetadata(new Thickness (4,4,4,4)),
            (value) => Utility.IsThicknessNonNegative((Thickness)value));

        public Thickness ResizeBorderThickness
        {
            get { return (Thickness)GetValue(ResizeBorderThicknessProperty); }
            set { SetValue(ResizeBorderThicknessProperty, value); }
        }
        #endregion

        #region DependencyProperty CaptionHeight
        public static readonly DependencyProperty CaptionHeightProperty = DependencyProperty.Register(
            "CaptionHeight",
            typeof(double),
            OwnerType,
            new PropertyMetadata(22.0),
            value => (double)value >= 0.0);

        /// <summary>The extent of the top of the window to treat as the caption.</summary>
        public double CaptionHeight
        {
            get => (double)GetValue(CaptionHeightProperty);
            set => SetValue(CaptionHeightProperty, value);
        }
        #endregion

        #region DependencyProperty AllowStartUpFrozen
        public static DependencyProperty AllowStartUpFrozenProperty =
            DependencyProperty.Register("AllowStartUpFrozen", typeof(bool), OwnerType,
                new FrameworkPropertyMetadata(false));


        public bool AllowStartUpFrozen
        {
            get => (bool)GetValue(AllowStartUpFrozenProperty);
            set => SetValue(AllowStartUpFrozenProperty, value);
        }
        #endregion

        #region DependencyProperty UnderStratumColor
        public static DependencyProperty UnderStratumColorProperty =
            DependencyProperty.Register("UnderStratumColor", typeof(object), OwnerType,
                new PropertyMetadata(ColorFrom.BlackOpacity, UnderStratumColorPropertyChangedCallback, UnderStratumColorPropertyCoerceValueCallback));

        /// <summary>
        /// Validate Color fpr 0x00000000 value
        /// </summary>
        /// <param name="d"></param>
        /// <param name="baseValue"></param>
        /// <returns></returns>
        private static object UnderStratumColorPropertyCoerceValueCallback(DependencyObject d, object baseValue)
        {
            //Color #0000 in user32 is SystemColors.WindowColor 
            return (baseValue == null || baseValue.IsEqual(ColorFrom.Zero)) 
                ? ColorFrom.BlackOpacity 
                : ColorFrom.Object(baseValue);
        }

        public object UnderStratumColor
        {
            get => GetValue(UnderStratumColorProperty);
            set => SetValue(UnderStratumColorProperty, value);
        }

        public static void UnderStratumColorPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is ChromeBase ch))
                throw new ArgumentException("CompositeChrome type expected");

            if (ch.OwnerWindow == null) return;
            if(ch.HWndSource == null) return;

            var colorValue = ColorFrom.Object(e.NewValue);
            ch.gbrColor = colorValue.ToABGRhex();


            if (ch.DependencyOpacity != colorValue.A)
                ch.DependencyOpacity = colorValue.A;


            var isBlurred = (bool)ch.OwnerWindow.GetValue(IsBlurredProperty);

                if (!isBlurred) ch.DisableBlur();
                else ch.EnableBlur();
            
        }
        #endregion

        #region DependencyProperty RgbChannelProperty
        public static DependencyProperty RgbChannelProperty =
            DependencyProperty.Register("RgbChannel", typeof(object), OwnerType,
                new PropertyMetadata("Black", RgbChannelPropertyChangedCallback, RgbChannelPropertyCoerceValueCallback));

        private static object RgbChannelPropertyCoerceValueCallback(DependencyObject d, object baseValue)
        {
            return (baseValue == null)
                ? Color.FromArgb(1, 0, 0, 0)
                : ColorFrom.Object(baseValue);
        }

        public object RgbChannel
        {
            get => GetValue(RgbChannelProperty);
            set => SetValue(RgbChannelProperty, value);
        }

        public static void RgbChannelPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is ChromeBase ch)) throw new ArgumentException();

            var colorValue = ColorFrom.Object(e.NewValue);
            colorValue.A = ch.DependencyOpacity;

            if (!ch.UnderStratumColor.IsEqual(colorValue))
                ch.UnderStratumColor = colorValue;
        }
        #endregion

        #region DependencyProperty DependencyOpacity
        public static DependencyProperty DependencyOpacityProperty =
            DependencyProperty.Register("DependencyOpacity", typeof(byte), OwnerType,
                new PropertyMetadata((byte)0, UnderStratumOpacityPropertyChangedCallback));

        public byte DependencyOpacity
        {
            get => (byte)GetValue(DependencyOpacityProperty);
            set => SetValue(DependencyOpacityProperty, value);
        }

        public static void UnderStratumOpacityPropertyChangedCallback(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is ChromeBase ch)) throw new ArgumentException();

            var opacity = (byte)e.NewValue;
            var color = ColorFrom.Object(ch.UnderStratumColor);

            if (color.A == opacity) return;

            color.A = opacity;
            ch.UnderStratumColor = color;

        }
        #endregion

        #region Attached property IsHitTestVisibleInChrome
        public static readonly DependencyProperty IsHitTestVisibleInChromeProperty = DependencyProperty.RegisterAttached(
            "IsHitTestVisibleInChrome",
            typeof(bool),
            OwnerType,
            new FrameworkPropertyMetadata(false, FrameworkPropertyMetadataOptions.Inherits));

        public static bool GetIsHitTestVisibleInChrome(IInputElement inputElement)
        {
            if (inputElement == null) throw new ArgumentNullException(nameof(inputElement));
            var obj = inputElement as DependencyObject;
            if (obj == null)
            {
                throw new ArgumentException("The element must be a DependencyObject, in AeroGlassChrome.GetIsHitTestVisibleInChrome", nameof(inputElement));
            }
            return (bool)obj.GetValue(IsHitTestVisibleInChromeProperty);
        }

        public static void SetIsHitTestVisibleInChrome(IInputElement inputElement, bool hitTestVisible)
        {
            if (inputElement == null) throw new ArgumentNullException(nameof(inputElement));
            var obj = inputElement as DependencyObject;
            if (obj == null)
            {
                throw new ArgumentException("The element must be a DependencyObject, in AeroGlassChrome.SetIsHitTestVisibleInChrome", nameof(inputElement));
            }
            obj.SetValue(IsHitTestVisibleInChromeProperty, hitTestVisible);
        }
        #endregion

        #region Attached property IsBlurred
        public static readonly DependencyProperty IsBlurredProperty =
            DependencyProperty.RegisterAttached(
                "IsBlurred",
                typeof(bool),
                OwnerType,
                new FrameworkPropertyMetadata(true, FrameworkPropertyMetadataOptions.Inherits, IsBlurredPropertyChanged));

        [AttachedPropertyBrowsableForType(typeof(Window))]
        [AttachedPropertyBrowsableForType(typeof(ChromeBase))]
        public static bool GetIsBlurred(Window obj)
        {
            return (bool)obj.GetValue(IsBlurredProperty);
        }

        [AttachedPropertyBrowsableForType(typeof(Window))]
        [AttachedPropertyBrowsableForType(typeof(ChromeBase))]
        public static void SetIsBlurred(Window obj, bool value)
        {
            obj.SetValue(IsBlurredProperty, value);
        }

        private static void IsBlurredPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is ChromeBase ch)) return;

            if (ch.HWndSource == null) return;

            var isBlurred = (bool)e.NewValue;
            if (isBlurred)
            {
                ch.EnableBlur();
            }
            else
            {
                ch.DisableBlur();
            }
        }
        #endregion

        #region Attached event DragStart
        public static readonly RoutedEvent DragStartEvent =
            EventManager.RegisterRoutedEvent("DragStart",
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                OwnerType);

        public static void AddHandler_DragStart(DependencyObject o, RoutedEventHandler handler)
        {
            if (!(o is UIElement uiElement)) return;
            uiElement.AddHandler(DragStartEvent, handler);
        }
        public static void RemoveAHandler_DragStart(DependencyObject o, RoutedEventHandler handler)
        {
            if (!(o is UIElement uiElement)) return;
            uiElement.RemoveHandler(DragStartEvent, handler);
        }
        #endregion

        #region Attached event DragMove
        public static readonly RoutedEvent DragMoveEvent =
            EventManager.RegisterRoutedEvent("DragMove",
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                OwnerType);

        public static void AddHandler_DragMove(DependencyObject o, RoutedEventHandler handler)
        {
            if (!(o is UIElement uiElement)) return;
            uiElement.AddHandler(DragMoveEvent, handler);
        }
        public static void RemoveAHandler_DragMove(DependencyObject o, RoutedEventHandler handler)
        {
            if (!(o is UIElement uiElement)) return;
            uiElement.RemoveHandler(DragMoveEvent, handler);
        }
        #endregion

        #region Attached event DragEnd
        public static readonly RoutedEvent DragEndEvent =
            EventManager.RegisterRoutedEvent("DragEnd",
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                OwnerType);

        public static void AddHandler_DragEnd(DependencyObject o, RoutedEventHandler handler)
        {
            if (!(o is UIElement uiElement)) return;
            uiElement.AddHandler(DragEndEvent, handler);
        }
        public static void RemoveAHandler_DragEnd(DependencyObject o, RoutedEventHandler handler)
        {
            if (!(o is UIElement uiElement)) return;
            uiElement.RemoveHandler(DragEndEvent, handler);
        }
        #endregion


        /// <inheritdoc />
        /// <summary>
        /// Freezable base method implementation
        /// </summary>
        /// <returns></returns>
        protected override Freezable CreateInstanceCore()
        {
            return (Freezable) Activator.CreateInstance(GetType());
        }

        /// <summary>
        /// Sets blur effect to current window
        /// </summary>
        public virtual void EnableBlur() { }

        /// <summary>
        /// Turns off Blur effect, sets undderstratum color
        /// </summary>
        public virtual void DisableBlur() {}

    }
}
