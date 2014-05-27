using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using MahApps.Metro.Controls;

namespace RockBot2
{
	[ValueConversion(typeof(bool), typeof(Visibility))]
	public class InvertableBooleanToVisibilityConverter : IValueConverter
	{
		enum Parameters
		{
			Normal, Inverted
		}

		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var boolValue = (bool)value;
			var direction = (Parameters)Enum.Parse(typeof(Parameters), (string)parameter);

			if (direction == Parameters.Inverted)
			{
				return !boolValue ? Visibility.Visible : Visibility.Collapsed;
			}

			return boolValue ? Visibility.Visible : Visibility.Collapsed;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}
	}

	[ValueConversion(typeof(double), typeof(double))]
	public class DimensionSubtract : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
		{
			var from = (double)value;
			var diff = double.Parse((String)parameter, CultureInfo.InvariantCulture);

			return from - diff;
		}

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
		{
			return null;
		}
	}

	[ValueConversion(typeof(bool), typeof(bool))]
	public class InverseBooleanConverter : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter,
			System.Globalization.CultureInfo culture)
		{
			if (targetType != typeof(bool))
				throw new InvalidOperationException("The target must be a boolean");

			return !(bool)value;
		}

		public object ConvertBack(object value, Type targetType, object parameter,
			System.Globalization.CultureInfo culture)
		{
			throw new NotSupportedException();
		}
	}
}
