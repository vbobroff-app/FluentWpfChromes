using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;

namespace FluentWpfChromes
{
    /// <inheritdoc />
    /// <summary>
    /// Base class to initialize Window HWndSource by Hook WndProc method
    /// </summary>
    internal abstract class InitializerBase : DependencyObject
    {
        internal Dictionary<WM, WindowMessageHandler> _messageDictionary = new Dictionary<WM, WindowMessageHandler>();

        /// <summary>
        /// current Window instance
        /// </summary>
        protected Window _window;

        /// <summary>Underlying HWndSource for the _window.</summary>
        protected HwndSource _source;

        /// <summary>Underlying HWnd for the _window.</summary>
        protected IntPtr _hWnd;

        /// <summary>
        ///  Attache WndProc flag
        /// </summary>
        private bool _isHooked;

        private ChromeBase _chromeBase;

        /// <summary>
        /// Matrix of the HT values for NC window messages.
        /// </summary>
        [SuppressMessage("Microsoft.Performance", "CA1814:PreferJaggedArraysOverMultidimensional", MessageId = "Member")]
        protected static readonly HT[,] _HitTestBorders = new[,]
        {
            { HT.TOPLEFT,    HT.TOP,     HT.TOPRIGHT    },
            { HT.LEFT,       HT.CLIENT,  HT.RIGHT       },
            { HT.BOTTOMLEFT, HT.BOTTOM,  HT.BOTTOMRIGHT },
        };

        protected virtual void HandlersCreate()
        {
            _messageDictionary.Add(WM.NCACTIVATE, Handle_NCActivate);
            _messageDictionary.Add(WM.NCCALCSIZE, Handle_NCCalcSize);
            _messageDictionary.Add(WM.NCHITTEST, Handle_NCHitTest);
            _messageDictionary.Add(WM.NCRBUTTONUP, Handle_NCRButtonUp);
            _messageDictionary.Add(WM.NCLBUTTONDOWN, Handle_NCLButtonDown);
            _messageDictionary.Add(WM.EXITSIZEMOVE, Handle_NCLButtonUp);
            _messageDictionary.Add(WM.WINDOWPOSCHANGED, Handle_WindowPosChanged);
            _messageDictionary.Add(WM.DWMCOMPOSITIONCHANGED, Handle_DwmCompositionChanged);
        }

        /// <summary>
        ///Current Window Chrome
        /// </summary>
        public ChromeBase ChromeBase
        {
            set
            {
                _chromeBase = value;
                if (_hWnd != IntPtr.Zero)
                {
                    _chromeBase.HWndSource = _source;
                }

            }
            get => _chromeBase;
        }


        protected void Initialize(Window window)
        {
            _window = window;
            _source = PresentationSource.FromVisual(_window) as HwndSource;

            if (_source == null)
            {
                _window.SourceInitialized += Window_SourceInitialized;
                return;
            }

            _hWnd = _source.Handle;

            if (_chromeBase != null) ChromeBase.HWndSource = _source;

            if (_isHooked) return;

            NativeMethods.SetInvisibleNonClientArea(_hWnd);

            if (_source.CompositionTarget != null)
                _source.CompositionTarget.BackgroundColor = Colors.Transparent;


            _source.AddHook(WndProc);
            _isHooked = true;
        }

        /// <summary>
        /// Overriden window messages listener method
        /// </summary>
        /// <param name="hWnd"></param>
        /// <param name="msg"></param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <param name="handled"></param>
        /// <returns></returns>
        protected virtual IntPtr WndProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            var message = (WM)msg;

            foreach (var wm in _messageDictionary.Keys)
            {
                if (wm == message)
                {

                    return _messageDictionary[wm](hWnd, message, wParam, lParam, out handled);
                }
            }
            return IntPtr.Zero;
        }


        #region Handlers

        protected virtual IntPtr Handle_NCActivate(IntPtr hWnd, WM uMsg, IntPtr wParam, IntPtr lParam, out bool handled)
        {
            // Directly call DefWindowProc with a custom parameter
            var lRet = NativeMethods.DefWindowProc(hWnd, WM.NCACTIVATE, wParam, new IntPtr(-1));
            handled = true;
            return lRet;
        }

        protected virtual IntPtr Handle_NCCalcSize(IntPtr hWnd, WM uMsg, IntPtr wParam, IntPtr lParam, out bool handled)
        {
            handled = true;

            // Per MSDN for NCCALCSIZE, always return 0, when wParam == FALSE
            var retVal = IntPtr.Zero;
            if (wParam.ToInt32() != 0) // wParam == TRUE
            {
                retVal = new IntPtr((int)(WVR.REDRAW));
            }

            return retVal;
        }

        public virtual IntPtr Handle_NCHitTest(IntPtr hWnd, WM uMsg, IntPtr wParam, IntPtr lParam, out bool handled)
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

                if (mousePosWindow.X < _chromeBase.CaptionHeight)
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
            var ht = HitTestNca(realWindowPosition, realMousePosition);

            return new IntPtr((int)ht);
        }


        private HT HitTestNca(Rect windowPosition, Point mousePosition)
        {
            //in the middle, HT.CLIENT
            var uRow = 1;
            var uCol = 1;
            var inResizeBorder = false;

            var resizeThickness = _chromeBase?.ResizeBorderThickness ?? new Thickness(4);
            var captionHeight = _chromeBase?.CaptionHeight ?? 0.0;

            // top or bottom 
            if (mousePosition.Y >= windowPosition.Top && mousePosition.Y < windowPosition.Top + resizeThickness.Top + captionHeight)
            {
                inResizeBorder = (mousePosition.Y < (windowPosition.Top + resizeThickness.Top));
                uRow = 0; // top (caption or resize border)
            }
            else if (mousePosition.Y < windowPosition.Bottom && mousePosition.Y >= windowPosition.Bottom - (int)resizeThickness.Bottom)
            {
                uRow = 2; // bottom
            }

            // left or right
            if (mousePosition.X >= windowPosition.Left && mousePosition.X < windowPosition.Left + (int)resizeThickness.Left)
            {
                uCol = 0; // left
            }
            else if (mousePosition.X < windowPosition.Right && mousePosition.X >= windowPosition.Right - resizeThickness.Right)
            {
                uCol = 2; // right
            }

            // If the cursor is in one of the top edges by the caption bar, but below the top resize border,
            // then resize left-right rather than diagonally.
            if (uRow == 0 && uCol != 1 && !inResizeBorder)
            {
                uRow = 1;
            }

            HT ht = _HitTestBorders[uRow, uCol];

            if (ht == HT.TOP && !inResizeBorder)
            {
                ht = HT.CAPTION;
            }

            return ht;
        }

        /// <summary>
        /// Get the bounding rectangle for the window in physical coordinates.
        /// </summary>
        /// <returns>The bounding rectangle for the window.</returns>
        protected Rect GetWindowRect(IntPtr hWnd)
        {
            // Get the window rectangle.
            RECT windowPosition = NativeMethods.GetWindowRect(hWnd);
            return new Rect(windowPosition.Left, windowPosition.Top, windowPosition.Width, windowPosition.Height);
        }


        protected virtual IntPtr Handle_NCLButtonDown(IntPtr hWnd, WM uMsg, IntPtr wParam, IntPtr lParam, out bool handled)
        {
            //Left button click in header area 
            if (HT.CAPTION == (HT)wParam.ToInt32())
            {
                var args = new RoutedEventArgs(ChromeBase.DragStartEvent, this);
                _window?.RaiseEvent(args);
            }
            handled = false;
            return IntPtr.Zero;
        }

        protected virtual IntPtr Handle_NCLButtonUp(IntPtr hWnd, WM uMsg, IntPtr wParam, IntPtr lParam, out bool handled)
        {

            //Left button click in header area 
            if (HT.CAPTION == (HT)wParam.ToInt32())
            {
                var args = new RoutedEventArgs(ChromeBase.DragEndEvent, this);
                _window?.RaiseEvent(args);
            }
            handled = false;
            return IntPtr.Zero;
        }

        protected virtual IntPtr Handle_NCRButtonUp(IntPtr hWnd, WM uMsg, IntPtr wParam, IntPtr lParam, out bool handled)
        {
            // Emulate the system behavior of clicking the right mouse button over the caption area
            // to bring up the system menu.
            if (HT.CAPTION == (HT)wParam.ToInt32())
            {
                NativeMethods.ShowSystemMenuPhysicalCoordinates(hWnd, new Point(Utility.GetXlParam(lParam), Utility.GetYlParam(lParam)));
            }
            handled = false;
            return IntPtr.Zero;
        }

        protected virtual IntPtr Handle_WindowPosChanged(IntPtr hWnd, WM uMsg, IntPtr wParam, IntPtr lParam, out bool handled)
        {
            // Still want to pass this to DefWndProc
            handled = false;
            return IntPtr.Zero;
        }

        protected virtual IntPtr Handle_DwmCompositionChanged(IntPtr hWnd, WM uMsg, IntPtr wParam, IntPtr lParam, out bool handled)
        {
            handled = false;
            return IntPtr.Zero;
        }
        #endregion

        private void Window_SourceInitialized(object sender, EventArgs e)
        {
            if (!(sender is Window w)) return;
            _window = (Window)sender;

            _source = PresentationSource.FromVisual(_window) as HwndSource;
            if (_source == null) throw new NullReferenceException("source");

            _hWnd = _source.Handle;

            if (_chromeBase != null) ChromeBase.HWndSource = _source;

            w.SourceInitialized -= Window_SourceInitialized;

            if (_isHooked) return;

            NativeMethods.SetInvisibleNonClientArea(_hWnd);

            if (_source.CompositionTarget != null)
                _source.CompositionTarget.BackgroundColor = Colors.Transparent;

            _source.AddHook(WndProc);
            _isHooked = true;

        }

    }
}