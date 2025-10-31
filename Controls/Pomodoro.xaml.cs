using Microsoft.Maui.Graphics;
using System.Diagnostics;

namespace TimeTracker.Controls;

public partial class Pomodoro : IDrawable
{
    public static readonly BindableProperty CurrentTimeProperty =
        BindableProperty.Create(nameof(CurrentTime),
            typeof(TimeSpan),
            typeof(Pomodoro),
            TimeSpan.FromSeconds(42),
            propertyChanged: OnMyTextPropertyChanged);

    public TimeSpan CurrentTime
    {
        get => (TimeSpan)GetValue(CurrentTimeProperty);
        set => SetValue(CurrentTimeProperty, value);
    }

    public static readonly BindableProperty StartTimeProperty =
        BindableProperty.Create(nameof(StartTime),
            typeof(int),
            typeof(Pomodoro),
            0,
            propertyChanged: OnMyTextPropertyChanged);

    public int StartTime
    {
        get => (int)GetValue(StartTimeProperty);
        set => SetValue(StartTimeProperty, value);
    }

    public static readonly BindableProperty EndTimeProperty =
        BindableProperty.Create(nameof(EndTime),
            typeof(int),
            typeof(Pomodoro),
            42,
            propertyChanged: OnMyTextPropertyChanged);

    public int EndTime
    {
        get => (int)GetValue(EndTimeProperty);
        set => SetValue(EndTimeProperty, value);
    }

    public static readonly BindableProperty ClockWiseProperty =
        BindableProperty.Create(nameof(ClockWise),
            typeof(bool),
            typeof(Pomodoro),
            false,
            propertyChanged: OnMyTextPropertyChanged);

    public bool ClockWise
    {
        get => (bool)GetValue(ClockWiseProperty);
        set => SetValue(ClockWiseProperty, value);
    }


    public static readonly BindableProperty DurationProperty =
		BindableProperty.Create(nameof(Duration), 
			typeof(int),
			typeof(Pomodoro),
			0,
			propertyChanged:OnMyTextPropertyChanged);

    public int Duration
    {
		get => (int)GetValue(DurationProperty);
		set => SetValue(DurationProperty, value);
	}

    public static readonly BindableProperty FillColorProperty =
        BindableProperty.Create(nameof(FillColor),
            typeof(Color),
            typeof(Pomodoro),
            Colors.Beige,
            propertyChanged: OnMyTextPropertyChanged);

    public Color FillColor
    {
        get => (Color)GetValue(FillColorProperty);
        set => SetValue(FillColorProperty, value);
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
        var smallerSize = MathF.Min(dirtyRect.Width, dirtyRect.Height);
        var arcSize = smallerSize * 0.8f;
        // draw full rect:
      //  canvas.StrokeColor = Colors.Gainsboro;
      //  canvas.DrawRectangle(dirtyRect);
		//Trace.WriteLine($"Draw {CurrentTime.TotalSeconds}...{dirtyRect.Center}");
        //Trace.WriteLine($"{StartTime} to {EndTime} {ClockWise}");
        canvas.StrokeSize = 3;
        
 
        canvas.StrokeSize = 16;
       
		canvas.StrokeColor = FillColor;
        canvas.StrokeLineCap = LineCap.Round;
        var normalized = (CurrentTime.TotalSeconds) / Duration;
        
        float clamped = (float) Math.Clamp(normalized, 0.0f, 0.99999f);
        float angle = 90 - 360 * clamped;
        Trace.WriteLine($"{CurrentTime.TotalSeconds}/{Duration}, clamped: {clamped}, {angle}  ");
        // 90 do -269.999
        canvas.DrawArc(         
            dirtyRect.Center.X- arcSize/2f, 
            dirtyRect.Center.Y- arcSize/2f,
            arcSize, arcSize,
           90,//StartTime,
            angle,// EndTime,
            true,//ClockWise,
            false);
    }

    private void StartHoverInteraction(object sender, TouchEventArgs e)
    {
		Trace.WriteLine("Hover interaction");
		Invalidate();
    }

    /*
        Debug bindings:
        StartTime="{Binding Source={x:Reference StartTimeSlider}, Path=Value}"
        EndTime="{Binding Source={x:Reference EndTimeSlider}, Path=Value}"
        ClockWise="{Binding Source={x:Reference Clockwise}, Path=IsToggled}"
     */
}