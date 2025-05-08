using System.Windows;
using System.Windows.Controls;

namespace SpixManga.UserControls;

public partial class SeriesFilterUC : UserControl
{
    private Dictionary<Grid, double> gridOriginalWidths = new Dictionary<Grid, double>();
    List<string> years = new List<string>() { "---" };
    List<string> startRating = new List<string>();


    public SeriesFilterUC()
    {
        InitializeComponent();
        TypeSeriesFilterComboBox.ItemsSource = new List<string> { "---", "Manga", "Manhwa", "Manhua", 
            "Novel", "Doujinshi", "Artbook", "Drama CD", "Filipino", "Indonesian", "OEL", "Thai", 
            "Vietnamese", "Malaysian", "Nordic", "French", "Spanish", "German" };
        for (int i = 2025; i >= 1900; i--)
        {
            years.Add(i.ToString());
        }
        YearSeriesFilterComboBox.ItemsSource = years;

        for (int i = 0; i <= 20; i++)
        {
            startRating.Add((i / 2.0).ToString("0.0"));
        }
        List<string> endRating = new List<string>(startRating);
        endRating.Reverse();
        startRating.Insert(0, "---");
        StartRatingSeriesFilterComboBox.ItemsSource = startRating;
        endRating.Insert(0, "---");
        EndRatingSeriesFilterComboBox.ItemsSource = endRating;
        StoreGridOriginalWidths();
        this.Loaded += SeriesFilterUC_Loaded;
    }

    private void SeriesFilterUC_Loaded(object sender, RoutedEventArgs e)
    {
        // Ensure layout is adjusted after the window is fully loaded
        AdjustWrapPanelItemWidthSame();
    }

    private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        AdjustWrapPanelItemWidthSame();
    }

    private void StoreGridOriginalWidths()
    {
        foreach (UIElement child in searchOptionsWPanel.Children)
        {
            if (child is Grid grid)
            {
                gridOriginalWidths[grid] = grid.Width;
            }
        }
    }

    private void AdjustWrapPanelItemWidthSame()
    {
        double availableWidth = Math.Max(0, genreOptionsWPanel.ActualWidth - 5);
        int numberOfItems = genreOptionsWPanel.Children.Count;
        int itemsPerRow = Math.Max(1, (int)(availableWidth / 110));
        double itemWidth = availableWidth / itemsPerRow;

        foreach (UIElement child in genreOptionsWPanel.Children)
        {
            if (child is StackPanel panel)
            {
                panel.Width = itemWidth;
            }
        }
    }

    private void filterSeriesListB_Click(object sender, RoutedEventArgs e)
    {

    }
}