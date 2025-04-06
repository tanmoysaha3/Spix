using System.Windows;
using System.Windows.Controls;

namespace SpixManga.Misc;

public class ListViewHelper
{
    internal static void ListView_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        var listView = sender as ListView;
        var gridView = listView!.View as GridView;

        if (gridView != null)
        {
            double totalAvailableWidth = listView.ActualWidth - SystemParameters.VerticalScrollBarWidth - 10.0;
            int columnCount = gridView.Columns.Count;

            if (columnCount > 0)
            {
                double newWidth = totalAvailableWidth / columnCount;

                foreach (var column in gridView.Columns)
                {
                    column.Width = newWidth;
                }
            }
        }
    }
}