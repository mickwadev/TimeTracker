using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Drawing.Geometries;

namespace TimeTracker.PageModels
{
    // https://livecharts.dev/docs/Maui/2.0.0-rc5.4/Overview.Installation
    class ChartTestPageModel
    {
        public ISeries[] Series { get; set; } = [
        new ColumnSeries<int>(3, 4, 2),
        new ColumnSeries<int>(4, 2, 6),
      //  new ColumnSeries<double, DiamondGeometry>(4, 3, 4)
    ];
    }
}
