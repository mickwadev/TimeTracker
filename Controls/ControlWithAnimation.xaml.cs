using System.Diagnostics;

namespace TimeTracker.Controls;

public partial class ControlWithAnimation : ContentView
{
	Animation a;
	int reps = 0;
	public ControlWithAnimation()
	{
		InitializeComponent();
		a = new Animation(v => Labelka.Scale =v,
			// ten start i end to by³o u¿ywane jak siê te 
			start:0,
			end:1
			);
	}

    private void StartScaleAnimationButtonClicked(object sender, EventArgs e)
    {
		Trace.WriteLine("zaczynamy skalowanie...");
		a.Commit(this, "skalowanie", 
			length: 2000,  // ile animacja ma trwaæ?
			easing: Easing.Linear,
			finished: (d, aborted) => 
			{
				Labelka.Text = $"Reps: {++reps} {d} {aborted}";
			},
			repeat: () => true);
    }

	private void StopAnimationButtonClicked(object sender, EventArgs e)
	{
		this.AbortAnimation("skalowanie");
		
	}
}