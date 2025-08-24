using SkiaSharp;
using Microsoft.Maui.Graphics;
using TimeTracker.Data;

namespace TimeTracker.Helpers
{
    public static class GradientSampler
    {
        const double blackThreshold = 1;
        const double redThreshold = 2;
        const double yellowThreshold = 4;
        const double greenThreshold = 6;

        static List<(double hourThreshold, SKColor color)> d = new() 
        {
            (0, SKColors.Black),
            (blackThreshold, SKColors.Black),
            (redThreshold, SKColors.Red),
            (yellowThreshold, SKColors.Yellow),
            (greenThreshold, SKColors.Green),
        };
        
        public static SKColor GetWorkTimeColor(double workTimeInSeconds)
        {
            double currentHour = workTimeInSeconds / AppConsts.SecondsInHours;
            for (int i = 1; i < d.Count; i++)
            {
                double startHour = d[i - 1].hourThreshold;
                double endHour = d[i].hourThreshold;
                if (currentHour >= startHour  && currentHour < endHour)
                {
                    var factor = (currentHour - startHour) / (endHour - startHour); // 0-1
                    SKColor stColor = d[i - 1].color;
                    SKColor endColor = d[i].color;
                    return SKColorLerp(stColor, endColor, factor);
                }
            }
            return SKColors.Green;
        }

        // v => 0-1
        static double Lerp(double st, double end, double v)
        {
           return st + (end - st) * v;
        }
        // v => 0-1
        static SKColor SKColorLerp(SKColor st, SKColor end, double v)
        {
            var r = (byte) Lerp(st.Red,end.Red,v);
            var g = (byte) Lerp(st.Green, end.Green, v);
            var b = (byte) Lerp(st.Blue, end.Blue, v);

            return new SKColor (r , g , b );
            
        }

        static Color ColorLerp(SKColor st, SKColor end, double v)
        {
            var r = (float)Lerp(st.Red, end.Red, v);
            var g = (float)Lerp(st.Green, end.Green, v);
            var b = (float)Lerp(st.Blue, end.Blue, v);

            return new Color(r / 255f, g / 255f, b / 255f);

        }

    }
}
