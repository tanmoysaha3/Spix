using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace SpixManga.Misc;

public class InputValidationHelper
{
    public static void AllowOnlyNumbers(object sender, TextCompositionEventArgs e)
    {
        TextBox textBox = sender as TextBox;
        if (textBox == null) return;

        e.Handled = !IsValidNumericInput(textBox.Text, e.Text);
    }

    public static void PreventNonNumericPaste(object sender, DataObjectPastingEventArgs e)
    {
        if (e.DataObject.GetDataPresent(typeof(string)))
        {
            string pastedText = (string)e.DataObject.GetData(typeof(string));
            TextBox textBox = sender as TextBox;
            if (textBox == null || !IsValidNumericInput(textBox.Text, pastedText))
            {
                e.CancelCommand();
            }
        }
        else
        {
            e.CancelCommand();
        }
    }

    private static bool IsValidNumericInput(string existingText, string newText)
    {
        string fullText = existingText + newText;

        if (!double.TryParse(fullText, System.Globalization.NumberStyles.AllowDecimalPoint,
                System.Globalization.CultureInfo.InvariantCulture, out _))
            return false;

        int decimalIndex = fullText.IndexOf('.');
        if (decimalIndex != -1 && fullText.Length > decimalIndex + 2)
        {
            return false;
        }

        return true;
    }
}