# Shine WPF Color Picker
Have you ever woken up to find your hometown ravaged by the giant monster that is "no easily accessible/modern-looking color picker in WPF"? Or perhaps you grow weary of the superheroes that have come before now, with WPF color pickers that weren't to your liking?

Well, fret no more, fellow citizen! For we, the Shine Development Group, have come to save the day! Presenting the **Shine WPF Color Picker**!

## How does it work?
Simple!

Download and reference the ShineColorPicker.dll file, and then add this where you need it:

```c#
        ShineColorPicker.ColorPickerDialog cpd = new ShineColorPicker.ColorPickerDialog(myColor); 
        cpd.Owner = this;
        cpd.ShowDialog();

        if (cpd.DialogResult)
        {
            myColor = cpd.SelectedColor;
        }
```

With that, you're set to go!

There is also an included ```ColorPicker``` control to add right into your WPF application!

## What features are available?
When you get to feast your eyes upon the beauty that is the Shine WPF Color Picker, there will be three tabs before you:

1. **Swatches** Includes all of the colors of the X11 color system, organized into 15 columns.
2. **Sliders** Includes the sliders and up-down buttons you want, for RGB and HSV, and also a hex string.
3. **From Image** Open up an image and select a pixel with the color you want. Used from [WPF Image Pixel Color Picker Element on CodeProject](https://www.codeproject.com/Articles/36848/WPF-Image-Pixel-Color-Picker-Element).

### Screenshots

[View screenshots of the color picker dialog!](https://github.com/shine-calendar/color-picker/blob/master/Screenshots.md)

## Plans for Future Versions

* Finish "From Palette" tab
* Make the ```ColorPicker``` control's ```Color``` property a DependencyProperty (for more usage with XAML)
* Add second box in ```ColorPickerDialog``` to display color when opened, for comparison with newly-selected colors
* Create custom number up-down control to remove WpfToolkit dependency
* (maybe) create new ```ToHexString(Color)``` method

## License
The Shine Color Picker is released under the [MIT License](https://github.com/shine-calendar/color-picker/blob/master/LICENSE).

Also utlizes code from Oleg V. Polikarpotchkin and Steve Lautenschlager. Finally, also utilizes the [Xceed Wpf Toolkit](wpftoolkit.codeplex.com) (licensed under [MS-PL License](http://wpftoolkit.codeplex.com/license)).

## About Us
Shine Calendar is striving to be the best calendar experience available on Windows, Windows 10, and Android.

Learn more at [shinecalendar.com](http://shinecalendar.com), or follow us on [Twitter](https://twitter.com/ShineCalendar) or [Tumblr](http://shinecalendar.tumblr.com).

Have questions? Email us at [shine-calendar@outlook.com](shine-calendar@outlook.com).
