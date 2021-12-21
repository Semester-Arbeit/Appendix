using System;
using UIKit;
using Microcharts;                                                            
using Microcharts.iOS;
using SkiaSharp;
using System.Collections.Generic;

namespace GroundStation
{
    public class HistoryPlot: UIView
    {
        LineChart myChart = new LineChart();
        ChartView myChartView = new ChartView();
        public List<ChartEntry> valueEntries = new List<ChartEntry>();
        private string dataPointColor = "";
        private UILabel title;

        private nfloat gridXOffset = 12;
        private nfloat gridYOffset = 15;
        private nfloat gridSingleTopOffset = -5;

        private List<UIView> BackgroundGrid = new List<UIView>();

        private List<UILabel> BackgroundGridLables = new List<UILabel>();

        private List<UILabel> HorizontalGridLabels = new List<UILabel>();


        public HistoryPlot(CoreGraphics.CGRect frame, string name, UIColor titelBackgroundColor, int MaxValue = 100, int MinValue = -100, string backgroundColor = "#16131b", int numberOfBackgorundLines = 12, bool isAltitude = false)
        {
            this.Frame = frame;
            title = new UILabel();
            title.Text = name;
            title.BackgroundColor = titelBackgroundColor;
            title.Frame = new CoreGraphics.CGRect(-10, 0, frame.Width+10, 20);
            this.AddSubview(title);
            this.dataPointColor = backgroundColor;
            this.BringSubviewToFront(title);

            

            for (int i = 0; i <= numberOfBackgorundLines; i++)
            {
                BackgroundGrid.Add(new UIView(new CoreGraphics.CGRect(gridXOffset, ((frame.Height-title.Frame.Height-2*gridYOffset)/numberOfBackgorundLines)*i + (title.Frame.Height + gridYOffset)+ gridSingleTopOffset, frame.Width-2*gridXOffset, 1)));
                BackgroundGrid[i].BackgroundColor = UIColor.Black;

                BackgroundGridLables.Add(new UILabel(new CoreGraphics.CGRect(gridXOffset - 20, ((frame.Height - title.Frame.Height - 2 * gridYOffset) / numberOfBackgorundLines) * i + (title.Frame.Height + gridYOffset) + gridSingleTopOffset - 10, 18, 20)));
                
                BackgroundGridLables[i].Font = UIFont.SystemFontOfSize(9);
                BackgroundGridLables[i].TextAlignment = UITextAlignment.Right;
                BackgroundGridLables[i].BackgroundColor = UIColor.SecondarySystemBackgroundColor;


                if (i == numberOfBackgorundLines /2 && !isAltitude)
                {
                    BackgroundGrid[i].Alpha = 0.4f;
                    BackgroundGridLables[i].Text = "0";
                }
                else if(i < numberOfBackgorundLines / 2 && !isAltitude)
                {
                    BackgroundGrid[i].Alpha = 0.1f;
                    BackgroundGridLables[i].Text = ((MaxValue/(numberOfBackgorundLines/2)) * (numberOfBackgorundLines/2 - i)).ToString();
                }
                else if(!isAltitude)
                {
                    BackgroundGrid[i].Alpha = 0.1f;
                    BackgroundGridLables[i].Text = ((MinValue / (numberOfBackgorundLines / 2)) * (i- numberOfBackgorundLines / 2)).ToString(); ;
                }
                else if (i == numberOfBackgorundLines && isAltitude)
                {
                    BackgroundGrid[i].Alpha = 0.4f;
                    BackgroundGridLables[i].Text = "0";
                }
                else
                {
                    BackgroundGrid[i].Alpha = 0.1f;
                    double currentValue = (Convert.ToDouble(MaxValue) / Convert.ToDouble(numberOfBackgorundLines)) * (Convert.ToDouble(numberOfBackgorundLines - i));
                    BackgroundGridLables[i].Text = (Math.Round(currentValue,1)).ToString();
                }


                this.AddSubview(BackgroundGrid[i]);
                this.SendSubviewToBack(BackgroundGrid[i]);

                
                this.AddSubview(BackgroundGridLables[i]);
                this.SendSubviewToBack(BackgroundGridLables[i]);
            }

            int numberOfLabels = 0;
            for (int i = 0; i <= 15; i++)
            {
                BackgroundGrid.Add(new UIView(new CoreGraphics.CGRect(((frame.Width - 2 * gridXOffset) / 15) * i + (gridXOffset), title.Frame.Height+ gridYOffset + gridSingleTopOffset, 1, (frame.Height - title.Frame.Height - 2 * gridYOffset))));
                BackgroundGrid[numberOfBackgorundLines+1+i].BackgroundColor = UIColor.Black;
                
                if(i%2 == 0)
                {
                    BackgroundGrid[numberOfBackgorundLines + 1 + i].Alpha = 0.2f;
                    HorizontalGridLabels.Add(new UILabel(new CoreGraphics.CGRect(((frame.Width - 2 * gridXOffset) / 15) * i + (gridXOffset) - 6, frame.Height - gridYOffset + gridSingleTopOffset, 25, 20)));
                    HorizontalGridLabels[numberOfLabels].Font = UIFont.SystemFontOfSize(9);
                    HorizontalGridLabels[numberOfLabels].TextAlignment = UITextAlignment.Left;
                    HorizontalGridLabels[numberOfLabels].Text = ((15 * -10 / 15) * (15 - i)).ToString();
                    this.AddSubview(HorizontalGridLabels[numberOfLabels]);
                    numberOfLabels++;
                }
                else
                {
                    BackgroundGrid[numberOfBackgorundLines + 1 + i].Alpha = 0.1f;
                }
                

                this.AddSubview(BackgroundGrid[numberOfBackgorundLines+1 + i]);
                this.SendSubviewToBack(BackgroundGrid[numberOfBackgorundLines+1 +i]);

                
                
            }

            myChart.Entries = valueEntries ;
            
            myChart.MaxValue = MaxValue;                                               
            myChart.MinValue = MinValue;
            myChart.LabelTextSize = 10;
            myChart.BackgroundColor = SKColor.Parse(getSystemColorAsString());

            myChartView.Chart = myChart;
            myChartView.Frame = new CoreGraphics.CGRect(0,20,this.Frame.Width,this.Frame.Height-20);
            this.AddSubview(myChartView);
            this.SendSubviewToBack(myChartView);

        }

    public void AddNewValue(double valueDouble)
    {
            float value = (float)valueDouble;
            if(valueEntries.Count > 15)
            {
                valueEntries.Remove(valueEntries[0]);
            }
        valueEntries.Add(new ChartEntry(value)
        {
            Color = SKColor.Parse(dataPointColor)
        });
        myChart.Entries = valueEntries;
            myChart.IsAnimated = false;

           
    }

    public string getSystemColorAsString()
    {
        if(UIColor.SecondarySystemBackgroundColor.ToString() == "UIColor [A=255, R=28, G=28, B=30]")
            {
                return "#1C1C1E";
            }
            else
            {
                return "#F2F2F6";
            }
            
    }
}
}
