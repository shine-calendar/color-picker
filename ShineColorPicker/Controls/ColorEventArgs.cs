using System;
using System.Windows.Media;

namespace ShineColorPicker.Controls
{
    public class ColorEventArgs : EventArgs
    {
        readonly Color c;

        public ColorEventArgs(Color col)
        {
            c = col;
        }

        //public ColorEventArgs(RoutedEvent re, Color col) : base(re)
        //{
        //    c = col;
        //}

        //public ColorEventArgs(RoutedEvent re, object source, Color col) : base(re, source)
        //{
        //    c = col;
        //}

        public Color Color
        {
            get
            {
                return c;
            }
        }

    }
}
