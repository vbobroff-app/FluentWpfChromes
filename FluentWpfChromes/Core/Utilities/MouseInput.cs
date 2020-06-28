using System;
using System.Windows;
using System.Windows.Input;

namespace FluentWpfChromes
{
    /// <summary>
    /// Mouse static class proxy object
    /// </summary>
    internal class MouseInput
    {
        private static readonly Type OwnerType = typeof(MouseInput);

        #region Attached propperty OverrideCursor

        public static readonly DependencyProperty OverrideCursorProperty = DependencyProperty.RegisterAttached(
            "OverrideCursor", typeof(Cursor), OwnerType,
            new FrameworkPropertyMetadata(OverrideCursorChanged));


        public static Cursor GetOverrideCursor(DependencyObject obj)
        { return (Cursor)obj.GetValue(OverrideCursorProperty); }

        public static void SetOverrideCursor(DependencyObject obj, Cursor value)
        {

            obj.SetValue(OverrideCursorProperty, value);
        }


        public static void OverrideCursorChanged(DependencyObject obj, DependencyPropertyChangedEventArgs e)
        {
            Mouse.OverrideCursor = (Cursor)e.NewValue;
        }

        #endregion

        public static bool IsLeftButtonReleased => Mouse.LeftButton == MouseButtonState.Released;

        public static bool IsLeftButtonPressed => Mouse.LeftButton == MouseButtonState.Pressed;

        public static bool IsRightButtonReleased => Mouse.RightButton == MouseButtonState.Released;

        public static bool IsRightButtonPressed => Mouse.RightButton == MouseButtonState.Pressed;
    }
}
