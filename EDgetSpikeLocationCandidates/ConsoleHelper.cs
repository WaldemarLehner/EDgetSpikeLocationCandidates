using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace EDgetSpikeLocationCandidates
{
    public static class ConsoleHelper
    {
        public static void PrintProgress(float progress, TimeSpan ts)
        {
            if (progress < 0 || progress > 1)
            {
                throw new ArgumentOutOfRangeException(nameof(progress), "Value needs to be in the Range of [0...1]");
            }

            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            int charCountDone = (int)(progress * 20);
            int charCountNotDone = 20 - charCountDone;
            for (int i = 0; i < charCountDone; i++)
            {
                sb.Append("#");
            }

            for (int i = 0; i < charCountNotDone; i++)
            {
                sb.Append("-");
            }

            sb
                .Append($"] {Math.Round(progress * 100, 2)} % ")
                .Append(ConsoleHelper.CalculateETA(progress, ts))
                .Append("\n");
            Console.WriteLine(sb.ToString());
        }

        public static string GetTimeSpanTimeAsString(TimeSpan ts)
        {
            return $"{ts.Hours:00}:{ts.Minutes:00}:{ts.Seconds:00}.{ts.Milliseconds / 10:00}";
        }

        public static void PrintElapsedTime(TimeSpan ts)
        {
            Console.WriteLine($"Elapsed Time: {GetTimeSpanTimeAsString(ts)}");
        }

        private static string CalculateETA(float progress, TimeSpan ts)
        {
            if (progress == 0)
            {
                return "ETA: TBA";
            }

            if (progress == 1)
            {
                return "Finished";
            }

            TimeSpan estimatedEntireTime = ts.Divide((double)progress);
            TimeSpan estimatedTimeToFinish = estimatedEntireTime - ts;
            return "ETA: " + GetTimeSpanTimeAsString(estimatedTimeToFinish);
        }
    }
}
