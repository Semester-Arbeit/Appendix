using System;
using UIKit;
using System.Collections.Generic;

namespace GroundStation
{
    public class UIDataSnapshot : UILabel
    {
        private string valueName = "";
        private string unit = "";

        private int averageOver = 0;

        private List<double> ringBufferForAverage = new List<double>();

        public UIDataSnapshot(string valueName, string unit, int averageOver = 40)
        {
            this.valueName = valueName;
            this.unit = unit;
            this.Text = valueName + ": ";
            this.averageOver = averageOver;
            this.Font = UIFont.GetMonospacedSystemFont(15, 0.1f);
        }

        public void setValue(double value)
        {
            ringBufferForAverage.Add(value);
            if (ringBufferForAverage.Count > averageOver)
            {
                ringBufferForAverage.Remove(ringBufferForAverage[0]);
            }

            this.Text = valueName + ":" + String.Format("{0,6:0.00}", calculateAverage()) + unit;
        }

        private double calculateAverage()
        {
            double res = 0;
            foreach(double val in ringBufferForAverage)
            {
                res += val;
            }
            return res / ringBufferForAverage.Count;
        }

        public void resetRingBuffer()
        {
            ringBufferForAverage.Clear();
        }
    }
}
