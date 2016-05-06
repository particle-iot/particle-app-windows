using System;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Media;

namespace Particle.Tinker.Controls
{
    #region Enums

    public enum PinAlignment
    {
        Left,
        Right
    }

    #endregion

    #region Converters

    public class DigitalValueToBackgroundConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var stringValue = (string)value;

            switch ((string)parameter)
            {
                case "background":
                    if (stringValue == "HIGH")
                        return new SolidColorBrush(Colors.White);
                    else
                        return new SolidColorBrush(Colors.Transparent);
                default:
                    if (stringValue == "HIGH")
                        return new SolidColorBrush(Colors.Black);
                    else
                        return new SolidColorBrush(Colors.White);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class KnownValueToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if ((bool)value)
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class PinActionDigitalToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var pinAction = (PinAction)value;

            if (pinAction == PinAction.DigitalRead || pinAction == PinAction.DigitalWrite || pinAction == PinAction.None)
                return Visibility.Collapsed;
            else
                return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class PinActionToFillConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            PinAction pinAction = PinAction.None;
            if (value != null)
                pinAction = (PinAction)value;

            switch (pinAction)
            {
                case PinAction.AnalogRead:
                case PinAction.AnalogWrite:
                case PinAction.AnalogWriteDac:
                case PinAction.DigitalRead:
                case PinAction.DigitalWrite:
                    return Application.Current.Resources["TinkerPinBackgroundSelected"];
                case PinAction.None:
                default:
                    return Application.Current.Resources["TinkerPinBackground"];
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class PinActionToFontSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var pinAction = (PinAction)value;

            if (pinAction == PinAction.DigitalRead || pinAction == PinAction.DigitalWrite)
                return 8;
            else
                return 14;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class PinActionToStrokeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            PinAction pinAction = PinAction.None;
            if (value != null)
                pinAction = (PinAction)value;

            switch (pinAction)
            {
                case PinAction.AnalogRead:
                    return Application.Current.Resources["Emerald"];
                case PinAction.AnalogWrite:
                    return Application.Current.Resources["Sunflower"];
                case PinAction.AnalogWriteDac:
                    return Application.Current.Resources["PinOrange"];
                case PinAction.DigitalRead:
                    return Application.Current.Resources["Cyan"];
                case PinAction.DigitalWrite:
                    return Application.Current.Resources["Alizarin"];
                case PinAction.None:
                default:
                    return new SolidColorBrush(Colors.Transparent);
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class PinActionToTextMarginConverter : IValueConverter
    {
        private Thickness digital = new Thickness(3, 0, 20, 0);
        private Thickness analog = new Thickness(10, 0, 5, 0);

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var pinAction = (PinAction)value;

            if (pinAction == PinAction.DigitalRead || pinAction == PinAction.DigitalWrite)
                return digital;
            else
                return analog;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class PinAlignmentToFlowDirectionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var pinAlignment = (PinAlignment)value;

            if (pinAlignment == PinAlignment.Left)
                return FlowDirection.LeftToRight;
            else
                return FlowDirection.RightToLeft;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class PinAlignmentToGridColumnConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var pinAlignment = (PinAlignment)value;

            switch ((string)parameter)
            {
                case "pin":
                    if (pinAlignment == PinAlignment.Left)
                        return 0;
                    else
                        return 1;
                case "value":
                    if (pinAlignment == PinAlignment.Left)
                        return 1;
                    else
                        return 0;
                default:
                    return 0;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class PinAlignmentToGridWidthConverter : IValueConverter
    {
        private GridLength PinColumn = new GridLength(.3, GridUnitType.Star);
        private GridLength ValueColumn = new GridLength(.7, GridUnitType.Star);

        public object Convert(object value, Type targetType, object parameter, string language)
        {
            var pinAlignment = (PinAlignment)value;

            switch ((string)parameter)
            {
                case "0":
                    if (pinAlignment == PinAlignment.Left)
                        return PinColumn;
                    else
                        return ValueColumn;
                default:
                    if (pinAlignment == PinAlignment.Left)
                        return ValueColumn;
                    else
                        return PinColumn;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    
    public class RequestErrorToFillConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if ((bool)value)
                return Application.Current.Resources["TinkerPinErrorBackground"];
            else
                return new SolidColorBrush(Colors.Transparent);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    public class SyncingToFillConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if ((bool)value)
                return Application.Current.Resources["TinkerPinBackground"];
            else
                return new SolidColorBrush(Colors.Transparent);
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }

    #endregion

    public sealed partial class PinButton : UserControl
    {
        #region Properties

        public PinAlignment Alignment
        {
            get { return (PinAlignment)GetValue(AlignmentProperty); }
            set { SetValue(AlignmentProperty, value); }
        }

        public static readonly DependencyProperty AlignmentProperty =
                    DependencyProperty.Register("Alignment", typeof(PinAlignment), typeof(PinButton), new PropertyMetadata(PinAlignment.Left));

        #endregion

        #region Constructors

        public PinButton()
        {
            InitializeComponent();
        }

        #endregion
    }
}
