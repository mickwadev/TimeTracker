using System.Diagnostics;

namespace TimeTracker.Controls;

public partial class Pomodoro : IDrawable
{
	public static readonly BindableProperty MyTextProperty =
		BindableProperty.Create(nameof(MyText), 
			typeof(TimeSpan),
			typeof(Pomodoro),
			TimeSpan.FromSeconds(42),
			propertyChanged:OnMyTextPropertyChanged);

    public TimeSpan MyText
	{
		get => (TimeSpan)GetValue(MyTextProperty);
		set => SetValue(MyTextProperty,value);
	}

	public static void OnMyTextPropertyChanged(BindableObject bindable, object oldValue, object newValue)
	{ 
		var pomodoro = (Pomodoro)bindable;
		pomodoro.Invalidate();
	}

	public Pomodoro()
	{
		InitializeComponent();
		this.Drawable = this;
		Trace.WriteLine("Pomodoro ctor...");
	}

    public void Draw(ICanvas canvas, RectF dirtyRect)
    {
		Trace.WriteLine($"Draw {MyText.TotalSeconds}...");
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