using System.Diagnostics;

namespace TimeTracker.Controls;

public partial class Pomodoro : IDrawable
{
	public Pomodoro()
	{
		InitializeComponent();
		this.Drawable = this;
		Trace.WriteLine("Pomodoro ctor...");
		Task.Run(async () =>
		{
			await Task.Delay(2222);
			Invalidate();
			Trace.WriteLine("invalidate...");
		});
		
	}

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
		Trace.WriteLine("Draw...");
		canvas.StrokeColor = Colors.AntiqueWhite;
		canvas.StrokeSize = 6;
		Point point = new Point(0,0);
		canvas.DrawCircle(point, 33.0);
    }

    private void StartHoverInteraction(object sender, TouchEventArgs e)
    {
		Trace.WriteLine("Hover interaction");
		Invalidate();
    }
}