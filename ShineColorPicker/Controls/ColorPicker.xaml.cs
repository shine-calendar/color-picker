using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using ShineColorPicker;
using System.Globalization;

namespace ShineColorPicker.Controls
{
    /// <summary>
    /// Interaction logic for ColorPicker.xaml
    /// </summary>
    public partial class ColorPicker : UserControl
    {
        #region Constructor, Events, Properties

        public ColorPicker()
        {
            InitializeComponent();
        }

        public delegate void ColorEventHandler(object sender, ColorEventArgs e);

        public event ColorEventHandler ColorChanged;

        public Window OwnerWindow { get; set; }

        public Color Color
        {
            get
            {
                return (colorPreview.Background as SolidColorBrush).Color;
            }
            set
            {
                colorPreview.Background = new SolidColorBrush(value);
                if (!updating)
                {
                    textBox.Text = ToHexString(value);
                }
                errorText.Visibility = Visibility.Collapsed;
                ColorChanged?.Invoke(this, new ColorEventArgs(value));
            }
        }

        bool updating = false;

        #endregion

        private void button_Click(object sender, RoutedEventArgs e)
        {
            ColorPickerDialog cpd = new ColorPickerDialog(Color);
            cpd.Owner = OwnerWindow;
            cpd.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            cpd.ShowDialog();

            if (cpd.DialogResult)
            {
                Color = cpd.SelectedColor;
            }
        }

        #region Hex color conversion

        /// <summary>
        /// Creates a color, based upon a hex triplet string.
        /// </summary>
        /// <param name="hex">The hex triplet string. Should be either of the format "AARRGGBB", "RRGGBB", or "RGB" (including or excluding the "#").</param>
        /// <returns></returns>
        Color CreateFromHex(string hex)
        {
            if (hex.StartsWith("#", StringComparison.Ordinal))
            {
                hex = hex.Substring(1);
            }

            switch (hex.Length)
            {
                case 6:
                    try
                    {
                        return Color.FromRgb(
                            byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber),
                            byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber),
                            byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber));
                    }
                    catch (FormatException ex)
                    {
                        throw new FormatException("Hex string is not in a correct format.", ex);
                    }

                case 8:
                    try
                    {
                        return Color.FromArgb(
                            byte.Parse(hex.Substring(0, 2), NumberStyles.HexNumber),
                            byte.Parse(hex.Substring(2, 2), NumberStyles.HexNumber),
                            byte.Parse(hex.Substring(4, 2), NumberStyles.HexNumber),
                            byte.Parse(hex.Substring(6, 2), NumberStyles.HexNumber));
                    }
                    catch (FormatException ex)
                    {
                        throw new FormatException("Hex string is not in a correct format.", ex);
                    }

                case 3:
                    try
                    {
                        string r = hex.Substring(0, 1) + hex.Substring(0, 1);
                        string g = hex.Substring(1, 1) + hex.Substring(1, 1);
                        string b = hex.Substring(2, 1) + hex.Substring(2, 1);

                        return Color.FromRgb(
                            byte.Parse(r, NumberStyles.HexNumber),
                            byte.Parse(g, NumberStyles.HexNumber),
                            byte.Parse(b, NumberStyles.HexNumber));
                    }
                    catch (FormatException ex)
                    {
                        throw new FormatException("Hex string is not in a correct format.", ex);
                    }

                default:
                    throw new ArgumentOutOfRangeException(nameof(hex), "The hex value must have a length of 3, 6, or 8, not including the '#' symbol.");
            }
        }

        // color to hex conversion code: http://www.cambiaresearch.com/articles/1/convert-dotnet-color-to-hex-string
        //written by Steve Lautenschlager

        static char[] hexDigits = {
         '0', '1', '2', '3', '4', '5', '6', '7',
         '8', '9', 'A', 'B', 'C', 'D', 'E', 'F'};

        string ToHexString(Color color)
        {
            byte[] bytes = new byte[3];
            bytes[0] = color.R;
            bytes[1] = color.G;
            bytes[2] = color.B;
            char[] chars = new char[bytes.Length * 2];
            for (int i = 0; i < bytes.Length; i++)
            {
                int b = bytes[i];
                chars[i * 2] = hexDigits[b >> 4];
                chars[i * 2 + 1] = hexDigits[b & 0xF];
            }
            return new string(chars);
        }

        #endregion

        /// <summary>
        /// Sets the selected color for this picker to the hex string you provide.
        /// </summary>
        /// <param name="s">The hex string. Use the formats "AARRGGBB", "RRGGBB", or "RGB" (with or without the "#").</param>
        /// <exception cref="ArgumentException">Thrown if the hex string could not be interpreted. Color is not changed as a result.</exception>
        public void SetColorFromString(string s)
        {
            try
            {
                updating = true;
                Color = CreateFromHex(s);
                textBox.Text = s;
                updating = false;
            }
            catch (Exception ex) when (ex is ArgumentOutOfRangeException || ex is FormatException)
            {
                updating = false;
                throw new ArgumentException("Color could not be set. Hex code could not be interpreted.", ex);
            }
        }

        private void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                updating = true;
                Color = CreateFromHex(textBox.Text);
                updating = false;
            }
            catch (Exception ex) when (ex is ArgumentOutOfRangeException || ex is FormatException)
            {
                //colorPreview.Background = new SolidColorBrush(Colors.Transparent);
                errorText.Visibility = Visibility.Visible;
            }
        }
    }
}
