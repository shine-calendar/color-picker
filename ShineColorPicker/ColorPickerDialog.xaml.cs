using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Windows.Media;
using System.IO;
using System.Text;
using System.Windows.Controls;
using System.Globalization;

namespace ShineColorPicker
{
    /// <summary>
    /// Interaction logic for ColorPickerDialog.xaml
    /// </summary>
    public partial class ColorPickerDialog : Window
    {

        #region Constructors

        public ColorPickerDialog()
        {
            InitializeComponent();
        }

        public ColorPickerDialog(Color color)
        {
            InitializeComponent();
            UpdateSelectedColor(color);
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the color selected in this dialog.
        /// </summary>
        public Color SelectedColor { get; private set; } = Colors.Black;
        /// <summary>
        /// Gets the dialog result of this dialog, based upon whether the user accepted the changes.
        /// </summary>
        public new bool DialogResult { get; private set; } = false;

        #endregion

        void UpdateSelectedColor(Color col, bool includeSliders = true)
        {
            SelectedColor = col;
            brdrSelColor.Background = new SolidColorBrush(col);

            if (includeSliders)
            {
                UpdateValues(col, "");
            }
        }

        #region Color Conversion Functions

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

        Color CreateFromHSV(double hue, double saturation, double value)
        {
            // math formula source: https://en.wikipedia.org/wiki/HSL_and_HSV#Converting_to_RGB

            double c = value * saturation; // chroma

            double h = hue / 60;

            double x = c * (1 - Math.Abs((h % 2) - 1));
            double m = value - c;

            double r1 = 0;
            double g1 = 0;
            double b1 = 0;

            if (0 <= h && h < 1)
            {
                r1 = c;
                g1 = x;
                b1 = 0;
            }
            else if (1 <= h && h < 2)
            {
                r1 = x;
                g1 = c;
                b1 = 0;
            }
            else if (2 <= h && h < 3)
            {
                r1 = 0;
                g1 = c;
                b1 = x;
            }
            else if (3 <= h && h < 4)
            {
                r1 = 0;
                g1 = x;
                b1 = c;
            }
            else if (4 <= h && h < 5)
            {
                r1 = x;
                g1 = 0;
                b1 = c;
            }
            else if (5 <= h && h < 6)
            {
                r1 = c;
                g1 = 0;
                b1 = x;
            }
            else
            {
                r1 = 0;
                g1 = 0;
                b1 = 0;
            }

            return Color.FromRgb(Convert.ToByte((r1 + m) * 255), Convert.ToByte((g1 + m) * 255), Convert.ToByte((b1 + m) * 255));
        }

        void ToHSV(Color color, out double hue, out double saturation, out double value)
        {
            // taken from http://www.rapidtables.com/convert/color/rgb-to-hsv.htm
            // backed up by https://en.wikipedia.org/wiki/HSL_and_HSV#Formal_derivation

            double r = Convert.ToDouble(color.R) / 255;
            double g = Convert.ToDouble(color.G) / 255;
            double b = Convert.ToDouble(color.B) / 255;

            double max = Math.Max(r, Math.Max(g, b));
            double min = Math.Min(r, Math.Min(g, b));
            double delta = max - min;

#pragma warning disable RECS0018 // Comparison of floating point numbers with equality operator
            if (delta == 0)
            {
                hue = 0;
            }
            else if (max == r)
            {
                if ((g - b) < 0)
                {
                    hue = 360 + (60 * (((g - b) / delta) % 6));
                }
                else
                {
                    hue = 60 * (((g - b) / delta) % 6);
                }
            }
            else if (max == g)
            {
                hue = 60 * (((b - r) / delta) + 2);
            }
            else // (max == b)
            {
                hue = 60 * (((r - g) / delta) + 4);
            }
#pragma warning restore RECS0018 // Comparison of floating point numbers with equality operator

            if (hue < 0)
            {
                Console.WriteLine("HUE UNDER 0: " + hue);
            }

            saturation = (Math.Abs(max) < Double.Epsilon) ? 0 : delta / max;
            value = max;

        }

        #endregion

        #region Swatches
        private void ColorButton_Click(object sender, RoutedEventArgs e)
        {
            UpdateSelectedColor(((sender as Button).Background as System.Windows.Media.SolidColorBrush).Color);
        }

        #endregion

        #region Sliders

        bool updating = false;

        private void sldR_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!updating)
            {
                nudR.Value = (int) e.NewValue;
            }
        }

        private void sldG_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!updating)
            {
                nudG.Value = (int) e.NewValue;
            }
        }

        private void sldB_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!updating)
            {
                nudB.Value = (int) e.NewValue;
            }
        }

        private void sldH_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!updating)
            {
                nudH.Value = (int) e.NewValue;
            }
        }

        private void sldS_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!updating)
            {
                nudS.Value = e.NewValue / 1000;
            }
        }

        private void sldV_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!updating)
            {
                nudV.Value = e.NewValue / 1000;
            }
        }

        private void txtHex_TextChanged(object sender, TextChangedEventArgs e)
        {
            try
            {
                Color col = CreateFromHex(txtHex.Text);
                lblQ.Visibility = Visibility.Collapsed;

                UpdateValues(col, "Hex");
            }
            catch (FormatException)
            {
                lblQ.Visibility = Visibility.Visible;
            }
            catch (ArgumentOutOfRangeException)
            {
                lblQ.Visibility = Visibility.Visible;
            }
            catch (NullReferenceException)
            {

            }
        }

        private void nudR_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (!updating)
            {
                try
                {
                    Color col = Color.FromRgb((byte) nudR.Value.Value, (byte) nudG.Value.Value, (byte) nudB.Value.Value);

                    UpdateValues(col, "RGB");
                }
                catch (NullReferenceException)
                {

                }
            }
        }

        private void nudG_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (!updating)
            {
                try
                {
                    Color col = Color.FromRgb((byte) nudR.Value.Value, (byte) nudG.Value.Value, (byte) nudB.Value.Value);

                    UpdateValues(col, "RGB");
                }
                catch (NullReferenceException)
                {

                }
            }
        }

        private void nudB_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (!updating)
            {
                try
                {
                    Color col = Color.FromRgb((byte) nudR.Value.Value, (byte) nudG.Value.Value, (byte) nudB.Value.Value);

                    UpdateValues(col, "RGB");
                }
                catch (NullReferenceException)
                {

                }
            }
        }

        private void nudH_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (!updating)
            {
                try
                {
                    Color col = CreateFromHSV(nudH.Value.Value, nudS.Value.Value, nudV.Value.Value);

                    UpdateValues(col, "HSV");
                }
                catch (NullReferenceException)
                {

                }
            }
        }

        private void nudS_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (!updating)
            {
                try
                {
                    Color col = CreateFromHSV(nudH.Value.Value, nudS.Value.Value, nudV.Value.Value);

                    UpdateValues(col, "HSV");
                }
                catch (NullReferenceException)
                {

                }
            }

        }

        private void nudV_ValueChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (!updating)
            {
                try
                {
                    Color col = CreateFromHSV(nudH.Value.Value, nudS.Value.Value, nudV.Value.Value);

                    UpdateValues(col, "HSV");
                }
                catch (NullReferenceException)
                {

                }
            }
        }

        void UpdateValues(Color col, string except)
        {
            if (except == "all")
            {
                return;
            }

            updating = true;

            if (except != "RGB")
            {
                nudR.Value = col.R;
                nudG.Value = col.G;
                nudB.Value = col.B;

                sldR.Value = col.R;
                sldG.Value = col.G;
                sldB.Value = col.B;
            }

            if (except != "HSV")
            {
                double h;
                double s;
                double v;

                ToHSV(col, out h, out s, out v);

                nudH.Value = (int) h;
                nudS.Value = s;
                nudV.Value = v;

                sldH.Value = h;
                sldS.Value = s * 1000;
                sldV.Value = v * 1000;
            }

            if (except != "Hex")
            {
                txtHex.Text = ToHexString(col);
            }

            UpdateSelectedColor(col, false);
            brdrSlColor.Background = new SolidColorBrush(col);

            updating = false;

        }

        #endregion

        #region From Image

        private void btnOpenImage_Click(object sender, RoutedEventArgs e)
        {
            string fPath = Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);

            Microsoft.Win32.OpenFileDialog ofd = new Microsoft.Win32.OpenFileDialog();
            ofd.Filter = "Images (*.png,*.jpg,*.jpeg,*.bmp,*.gif,*.wmf)|*.png;*.jpg;*.jpeg;*.bmp;*.gif;*.wmf|All Files (*.*)|*.*";

            ofd.InitialDirectory = fPath;

            bool? res = ofd.ShowDialog();

            if (res.HasValue && res.Value)
            {
                imgPicker.Source = new System.Windows.Media.Imaging.BitmapImage(new Uri(ofd.FileName, UriKind.Absolute));
            }
        }

        private void imgPicker_SelectedColorChanged(object sender, RoutedEventArgs e)
        {
            UpdateSelectedColor(imgPicker.SelectedColor);
        }

        #endregion

        #region From Palette

        private void btnLoadPal_Click(object sender, RoutedEventArgs e)
        {

        }

        public List<Color> LoadPal(string filename)
        {
            List<Color> colors = new List<Color>();
            FileStream stream = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
            using (BinaryReader br = new BinaryReader(stream))
            {
                // RIFF header
                string riff = ReadByteString(br, 4); // "RIFF"
                int dataSize = br.ReadInt32();
                string type = ReadByteString(br, 4); // "PAL "

                // Data chunk
                string chunkType = ReadByteString(br, 4); // "data"
                int chunkSize = br.ReadInt32();
                short palVersion = br.ReadInt16(); // always 0x0300
                short palEntries = br.ReadInt16();

                // Colors
                for (int i = 0; i < palEntries; i++)
                {
                    byte red = br.ReadByte();
                    byte green = br.ReadByte();
                    byte blue = br.ReadByte();
                    byte flags = br.ReadByte(); // always 0x00
                    colors.Add(Color.FromRgb(red, green, blue));
                }
            }
            return colors;
        }

        string ReadByteString(BinaryReader br, int length)
        {
            ASCIIEncoding ae = new ASCIIEncoding();

            return ae.GetString(br.ReadBytes(length));
        }

        #endregion

        private void btnOK_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
            Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }


    }
}
