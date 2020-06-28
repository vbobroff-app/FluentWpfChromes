using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace FluentWpfChromes
{
    /// <inheritdoc />
    /// <summary>
    /// Initialize Window HWndSource by Hook WndProc method,
    /// special for Acrylic window
    /// </summary>
    internal sealed class AcrylicInitializer : InitializerBase
    {
        private static readonly Type OwnerType = typeof(AcrylicInitializer);

        /// <summary>
        /// Hit test value
        /// </summary>
        private HT _ht;


        #region Drag, reisize variables
        private bool _isDrag;
        private bool _isResize;
        private RECT _windowPosition;
        private Point _startPoint;
        private DateTime _hitTime = DateTime.Now;
        int _dragDelay = 10;
        private int _resizeDelay = 30;
        private ResizeDirection _rDirection;
        #endregion

        #region Attached property AcrylicInitializer

        [DefaultValue("Null")]
        public static readonly DependencyProperty AcrylicInitializerProperty =
            DependencyProperty.RegisterAttached(
                "AcrylicInitializer",
                OwnerType,
                OwnerType,
                new FrameworkPropertyMetadata(null, AcrylicInitializerPropertyChanged));

        [AttachedPropertyBrowsableForType(typeof(Window))]
        public static AcrylicInitializer GetAcrylicInitializer(Window obj)
        {
            return (AcrylicInitializer)obj.GetValue(AcrylicInitializerProperty);
        }

        [AttachedPropertyBrowsableForType(typeof(Window))]
        public static void SetAcrylicInitializer(Window obj, AcrylicInitializer value)
        {
            obj.SetValue(AcrylicInitializerProperty, value);
        }

        [AttachedPropertyBrowsableForType(typeof(Window))]
        private static void AcrylicInitializerPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {

            if (!(d is Window window)) throw new ArgumentException("d must be Window");
            if (!(e.NewValue is AcrylicInitializer ch)) throw new ArgumentNullException();

            ch.Initialize(window);

        }


        #endregion

        protected override void HandlersCreate()
        {
            _messageDictionary.Add(WM.NCACTIVATE, Handle_NCActivate);
            _messageDictionary.Add(WM.NCCALCSIZE, Handle_NCCalcSize);
            _messageDictionary.Add(WM.NCHITTEST, Handle_NCHitTest);
            _messageDictionary.Add(WM.NCRBUTTONUP, Handle_NCRButtonUp);

            _messageDictionary.Add(WM.WINDOWPOSCHANGED, Handle_WindowPosChanged);
            _messageDictionary.Add(WM.DWMCOMPOSITIONCHANGED, Handle_DwmCompositionChanged);

            _messageDictionary.Add(WM.LBUTTONDOWN, _HandleLButtonDown);
            _messageDictionary.Add(WM.MOUSEMOVE, _HandleMouseMove);
            _messageDictionary.Add(WM.LBUTTONUP, _HandleLButtonUp);
        }

        // constructor
        public AcrylicInitializer()
        {
            HandlersCreate();
        }
        protected override IntPtr WndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            var message = (WM)msg;

            foreach (var wm in _messageDictionary.Keys)
            {
                if (wm == message)
                {
                    _ht = (HT)_messageDictionary[wm](hWnd, message, wParam, lParam, out handled);

                }
            }

            return NativeMethods.DefWindowProc(hWnd, message, wParam, lParam);

        }

        #region Handlers

        public override IntPtr Handle_NCHitTest(IntPtr hWnd, WM uMsg, IntPtr wParam, IntPtr lParam, out bool handled)
        {
            if (_source.CompositionTarget == null)
            {
                handled = false;
                return IntPtr.Zero;
            }

            var DpiScaleX = _source.CompositionTarget.TransformFromDevice.M11;
            var DpiScaleY = _source.CompositionTarget.TransformFromDevice.M22;

            // Let the system know if we consider the mouse to be in our effective non-client area.
            var mousePosScreen = new Point(Utility.GetXlParam(lParam), Utility.GetYlParam(lParam));
            var windowPosition = GetWindowRect(hWnd);

            var mousePosWindow = mousePosScreen;
            mousePosWindow.Offset(-windowPosition.X, -windowPosition.Y);
            mousePosWindow = DpiHelper.DevicePixelsToLogical(mousePosWindow, DpiScaleX, DpiScaleY);

            var inputElement = _window.InputHitTest(mousePosWindow);
            if (inputElement != null)
            {
                if (ChromeBase.GetIsHitTestVisibleInChrome(inputElement))
                {

                    handled = true;
                    return new IntPtr((int)HT.CLIENT);
                }

                if (mousePosWindow.X < ChromeBase.CaptionHeight)
                {
                    var parent = ((DependencyObject)inputElement).TryFindParent<Control>();
                    if (ChromeBase.GetIsHitTestVisibleInChrome(parent))
                    {
                        handled = true;
                        return new IntPtr((int)HT.CLIENT);
                    }
                }

            }

            handled = true;

            var realWindowPosition = DpiHelper.DeviceRectToLogical(windowPosition, DpiScaleX, DpiScaleY);
            var realMousePosition = DpiHelper.DevicePixelsToLogical(mousePosScreen, DpiScaleX, DpiScaleY);
            var ht = _HitTestNca(realWindowPosition, realMousePosition);

            return new IntPtr((int)ht);
        }


        private HT _HitTestNca(Rect windowPosition, Point mousePosition)
        {
            //in the middle, HT.CLIENT
            var uRow = 1;
            var uCol = 1;
            var inResizeBorder = false;

            var resizeThickness = ChromeBase?.ResizeBorderThickness ?? new Thickness(4);
            var captionHeight =  ChromeBase?.CaptionHeight ?? 0.0;

            Mouse.OverrideCursor = null;

            // top or bottom 
            if (mousePosition.Y >= windowPosition.Top && mousePosition.Y < windowPosition.Top + resizeThickness.Top + captionHeight)
            {
                inResizeBorder = (mousePosition.Y < (windowPosition.Top + resizeThickness.Top));
                uRow = 0; // top (caption or resize border)
            }
            else if (mousePosition.Y < windowPosition.Bottom && mousePosition.Y >= windowPosition.Bottom - (int)resizeThickness.Bottom)
            {
                Mouse.OverrideCursor = Cursors.SizeNS;
                uRow = 2; // bottom
            }

            // left or right
            if (mousePosition.X >= windowPosition.Left && mousePosition.X < windowPosition.Left + (int)resizeThickness.Left)
            {
                Mouse.OverrideCursor = Cursors.SizeWE;
                uCol = 0; // left
            }
            else if (mousePosition.X < windowPosition.Right && mousePosition.X >= windowPosition.Right - resizeThickness.Right)
            {
                Mouse.OverrideCursor = Cursors.SizeWE;
                uCol = 2; // right
            }

           
            // If the cursor is in one of the top edges by the caption bar, but below the top resize border,
            // then resize left-right rather than diagonally.
            if (uRow == 0 && uCol != 1 && !inResizeBorder)
            {
                uRow = 1;
              
            }

            HT ht = _HitTestBorders[uRow, uCol];

            if (Mouse.OverrideCursor != null)
            {
                switch (ht)
                {
                    case HT.TOPLEFT:
                        Mouse.OverrideCursor = Cursors.SizeNWSE;
                        break;
                    case HT.TOPRIGHT:
                        Mouse.OverrideCursor = Cursors.SizeNESW;
                        break;
                    case HT.BOTTOMLEFT:
                        Mouse.OverrideCursor = Cursors.SizeNESW;
                        break;
                    case HT.BOTTOMRIGHT:
                        Mouse.OverrideCursor = Cursors.SizeNWSE;
                        break;
                }
            }

            if (ht == HT.TOP)
            {
                if (!inResizeBorder)
                {
                    ht = HT.CAPTION;
                }
                else
                {
                    Mouse.OverrideCursor = Cursors.SizeNS;
                }
            }

            return ht;
        }


        private IntPtr _HandleLButtonDown(IntPtr hWnd, WM uMsg, IntPtr wParam, IntPtr lParam, out bool handled)
        {

            switch (_ht)
            {
                case HT.CAPTION: //header area 
                    _isDrag = true;
                    _windowPosition = NativeMethods.GetWindowRect(hWnd);
                    _startPoint = CursorPosition.GetCursorPosition();
                    NativeMethods.SetCapture(hWnd);
                    var args = new RoutedEventArgs(ChromeBase.DragStartEvent, this);
                    _window?.RaiseEvent(args);
                    break;
                case HT.LEFT:
                    _rDirection = ResizeDirection.Left;
                    break;
                case HT.RIGHT:
                    _rDirection = ResizeDirection.Right;
                    break;
                case HT.TOP:
                    _rDirection = ResizeDirection.Top;
                    break;
                case HT.BOTTOM:
                    _rDirection = ResizeDirection.Bottom;
                    break;
                case HT.TOPRIGHT:
                    _rDirection = ResizeDirection.TopRight;
                    break;
                case HT.TOPLEFT:
                    _rDirection = ResizeDirection.TopLeft;
                    break;
                case HT.BOTTOMLEFT:
                    _rDirection = ResizeDirection.BottomLeft;
                    break;
                case HT.BOTTOMRIGHT:
                    _rDirection = ResizeDirection.BottomRight;
                    break;
            }

            if ((int)_ht > 9 && (int)_ht < 18)
            {
                _isResize = true;
                _windowPosition = NativeMethods.GetWindowRect(hWnd);
                _startPoint = CursorPosition.GetCursorPosition();
                NativeMethods.SetCapture(hWnd);
            }

            handled = false;
            return IntPtr.Zero;
        }

        private IntPtr _HandleMouseMove(IntPtr hWnd, WM uMsg, IntPtr wParam, IntPtr lParam, out bool handled)
        {
            //Mouse move in header area 
            if (_isDrag)
            {
                var deltaTine = DateTime.Now - _hitTime;
                if (deltaTine.TotalMilliseconds < 10)
                {
                    handled = false;
                    return IntPtr.Zero;
                }

                _hitTime = DateTime.Now;
                var currentPoint = CursorPosition.GetCursorPosition();
                var delta = currentPoint - _startPoint;

                NativeMethods.MoveWindow(hWnd, _windowPosition.Left + (int)delta.X, _windowPosition.Top + (int)delta.Y, _windowPosition.Width, _windowPosition.Height, true);

                var args = new RoutedEventArgs(ChromeBase.DragMoveEvent, this);
                _window?.RaiseEvent(args);

            }
            else
            if (_isResize)
            {
                var deltaTine = DateTime.Now - _hitTime;
                if (deltaTine.TotalMilliseconds < 30)
                {
                    handled = false;
                    return IntPtr.Zero;
                }
                _hitTime = DateTime.Now;
                var currentPoint = CursorPosition.GetCursorPosition();
                var delta = currentPoint - _startPoint;
                switch (_rDirection)
                {
                    case ResizeDirection.Right:
                        NativeMethods.MoveWindow(hWnd, _windowPosition.Left, _windowPosition.Top, _windowPosition.Width + (int)delta.X, _windowPosition.Height, true);
                        break;
                    case ResizeDirection.Left:
                        NativeMethods.MoveWindow(hWnd, _windowPosition.Left + (int)delta.X, _windowPosition.Top, _windowPosition.Width - (int)delta.X, _windowPosition.Height, true);
                        break;
                    case ResizeDirection.Bottom:
                        NativeMethods.MoveWindow(hWnd, _windowPosition.Left, _windowPosition.Top, _windowPosition.Width, _windowPosition.Height + (int)delta.Y, true);
                        break;
                    case ResizeDirection.Top:
                        NativeMethods.MoveWindow(hWnd, _windowPosition.Left, _windowPosition.Top + (int)delta.Y, _windowPosition.Width, _windowPosition.Height - (int)delta.Y, true);
                        break;
                    case ResizeDirection.TopRight:
                        NativeMethods.MoveWindow(hWnd, _windowPosition.Left, _windowPosition.Top + (int)delta.Y, _windowPosition.Width + (int)delta.X, _windowPosition.Height - (int)delta.Y, true);
                        break;
                    case ResizeDirection.BottomRight:
                        NativeMethods.MoveWindow(hWnd, _windowPosition.Left, _windowPosition.Top, _windowPosition.Width + (int)delta.X, _windowPosition.Height + (int)delta.Y, true);
                        break;
                    case ResizeDirection.TopLeft:
                        NativeMethods.MoveWindow(hWnd, _windowPosition.Left + (int)delta.X, _windowPosition.Top + (int)delta.Y, _windowPosition.Width - (int)delta.X, _windowPosition.Height - (int)delta.Y, true);
                        break;
                    case ResizeDirection.BottomLeft:
                        NativeMethods.MoveWindow(hWnd, _windowPosition.Left + (int)delta.X, _windowPosition.Top, _windowPosition.Width - (int)delta.X, _windowPosition.Height + (int)delta.Y, true);
                        break;
                }
            }
            handled = false;
            return IntPtr.Zero;
        }

        private IntPtr _HandleLButtonUp(IntPtr hWnd, WM uMsg, IntPtr wParam, IntPtr lParam, out bool handled)
        {
            if (_isDrag || _isResize)
            {
                NativeMethods.ReleaseCapture();
            }
            _isDrag = false;
            _isResize = false;
            
            //Left button click in header area 
            if (HT.CAPTION == _ht)
            {
                var args = new RoutedEventArgs(ChromeBase.DragEndEvent, this);
                _window?.RaiseEvent(args);
            }
            handled = false;
            return IntPtr.Zero;
        }

        #endregion


    }
}