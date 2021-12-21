using System;
using UIKit;
using Microcharts;
using Microcharts.iOS;
using SkiaSharp;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GroundStation
{
    public class InflightView : UIView
    {


        private ValuePlot YawAngle = new ValuePlot(new CoreGraphics.CGRect(250, 0, 50, 250), "",UIColor.SystemBlueColor, 90,-90,"#006FF9");
        private HistoryPlot YawHistory = new HistoryPlot(new CoreGraphics.CGRect(0, 0, 250, 250), "Yaw", UIColor.SystemBlueColor, 90, -90, "#006FF9");

        private ValuePlot PitchAngle = new ValuePlot(new CoreGraphics.CGRect(250, 300, 50, 250), "", UIColor.SystemGreenColor, 90, -90, "#00BF55");
        private HistoryPlot PitchHistory = new HistoryPlot(new CoreGraphics.CGRect(0, 300, 250, 250), "Pitch", UIColor.SystemGreenColor, 90, -90, "#00BF55");

        private ValuePlot RollAngle = new ValuePlot(new CoreGraphics.CGRect(600, 0, 50, 250), "", UIColor.SystemRedColor, 90, -90, "#FF3432");
        private HistoryPlot RollHistory = new HistoryPlot(new CoreGraphics.CGRect(350, 0, 250, 250), "Roll", UIColor.SystemRedColor, 90, -90, "#FF3432");

        private ValuePlot Altitude = new ValuePlot(new CoreGraphics.CGRect(600, 300, 50, 250), "", UIColor.LightGray, 5, 0,"#A0A0A0");
        private HistoryPlot AltitudeHistory = new HistoryPlot(new CoreGraphics.CGRect(350, 300, 250, 250), "Altitude", UIColor.LightGray, 5, 0,"#A0A0A0",12,true);

        private DuctedFanPrototype connectedVehicle;

        private UIButton getSensorValues = new UIButton();

        private UIDataSnapshot currentPitch = new UIDataSnapshot("Pitch","°");
        private UIDataSnapshot currentRoll = new UIDataSnapshot("Roll", "°");
        private UIDataSnapshot currentYaw = new UIDataSnapshot("Yaw", "°");

        private UIDataSnapshot currentAlt = new UIDataSnapshot("Alt", "m");
        private UIDataSnapshot currentLat = new UIDataSnapshot("PosX", "m");
        private UIDataSnapshot currentLong = new UIDataSnapshot("PosY", "m");

        private UIDataSnapshot currentGyroX = new UIDataSnapshot("GyroX", "°/s");
        private UIDataSnapshot currentGyroY = new UIDataSnapshot("GyroY", "°/s");
        private UIDataSnapshot currentGyroZ = new UIDataSnapshot("GyroZ", "°/s");

        private UIDataSnapshot currentAccX = new UIDataSnapshot("AccX", "m/s2");
        private UIDataSnapshot currentAccY = new UIDataSnapshot("AccY", "m/s2");
        private UIDataSnapshot currentAccZ = new UIDataSnapshot("AccZ", "m/s2");

        private UIDataSnapshot currentSpeedX = new UIDataSnapshot("SpeedX", "m/s");
        private UIDataSnapshot currentSpeedY = new UIDataSnapshot("SpeedY", "m/s");
        private UIDataSnapshot currentSpeedZ = new UIDataSnapshot("SpeedZ", "m/s");

        private UIDataSnapshot currentCorrrectionPitch = new UIDataSnapshot("corrP", "°");
        private UIDataSnapshot currentCorrrectionRoll = new UIDataSnapshot("corrR", "°");
        private UIDataSnapshot currentCorrrectionYaw = new UIDataSnapshot("corrY", "°");

        private UIFrequencyView currentProcessorPerformace = new UIFrequencyView();

        private UIDataSnapshot currentPower = new UIDataSnapshot("PowerOut", "%");

        public InflightView(CoreGraphics.CGRect Frame, DuctedFanPrototype connectedVehicle)
        {
            this.Frame = Frame;
            this.connectedVehicle = connectedVehicle;

            this.AddSubview(YawAngle);
            this.AddSubview(YawHistory);

            this.AddSubview(PitchAngle);
            this.AddSubview(PitchHistory);

            this.AddSubview(RollAngle);
            this.AddSubview(RollHistory);

            this.AddSubview(Altitude);
            this.AddSubview(AltitudeHistory);

            UILabel sliderTitle = new UILabel();
            sliderTitle.Text = "Slide to fly";
            sliderTitle.Frame = new CoreGraphics.CGRect(800, 30, 200, 50);
            this.AddSubview(sliderTitle);

           

            UISlider startSlider = new UISlider();
            startSlider.Frame = new CoreGraphics.CGRect(800, 80, 200, 50);
            startSlider.AddTarget(StartSliderTouched,UIControlEvent.TouchUpInside);
            startSlider.AddTarget(StartSliderTouched, UIControlEvent.TouchUpOutside);
            this.AddSubview(startSlider);

            getSensorValues.SetTitle("Get Sensor Values", new UIControlState());
            getSensorValues.Frame = new CoreGraphics.CGRect(800, 180, 200, 50);
            getSensorValues.BackgroundColor = UIColor.SystemGreenColor;
            getSensorValues.AddTarget(GetSensorValuesPressed, UIControlEvent.TouchDown);
            getSensorValues.Layer.CornerRadius = 10;
            getSensorValues.Layer.ShadowColor = new CoreGraphics.CGColor(0, 0, 0, 1);
            getSensorValues.Layer.ShadowOffset = new CoreGraphics.CGSize(5, 5);
            getSensorValues.Layer.ShadowOpacity = 0.2F;
            this.AddSubview(getSensorValues);

            float x = 700;
            float y = 250;

            float ySep = 50;
            float xSep = 200;

            currentPitch.setValue(0);
            currentPitch.Frame = new CoreGraphics.CGRect(x + 0 * xSep, y+ ySep*0, xSep, ySep);
            this.AddSubview(currentPitch);

            currentRoll.setValue(0);
            currentRoll.Frame = new CoreGraphics.CGRect(x+ 1* xSep, y + ySep * 0, xSep, ySep);
            this.AddSubview(currentRoll);

            currentYaw.setValue(0);
            currentYaw.Frame = new CoreGraphics.CGRect(x+ 2* xSep, y + ySep * 0, xSep, ySep);
            this.AddSubview(currentYaw);

            currentAlt.setValue(0);
            currentAlt.Frame = new CoreGraphics.CGRect(x + 2 * xSep, y + ySep * 1, xSep, ySep);
            this.AddSubview(currentAlt);

            currentLat.setValue(0);
            currentLat.Frame =  new CoreGraphics.CGRect(x + 0 * xSep, y + ySep * 1, xSep, ySep);
            this.AddSubview(currentLat);

            currentLong.setValue(0);
            currentLong.Frame = new CoreGraphics.CGRect(x + 1 * xSep, y + ySep * 1, xSep, ySep);
            this.AddSubview(currentLong);

            currentGyroX.setValue(0);
            currentGyroX.Frame = new CoreGraphics.CGRect(x + 0 * xSep, y + ySep * 2, xSep, ySep);
            this.AddSubview(currentGyroX);

            currentGyroY.setValue(0);
            currentGyroY.Frame = new CoreGraphics.CGRect(x + 1 * xSep, y + ySep * 2, xSep, ySep);
            this.AddSubview(currentGyroY);

            currentGyroZ.setValue(0);
            currentGyroZ.Frame = new CoreGraphics.CGRect(x + 2 * xSep, y + ySep * 2, xSep, ySep);
            this.AddSubview(currentGyroZ);

            currentAccX.setValue(0);
            currentAccX.Frame = new CoreGraphics.CGRect(x + 0 * xSep, y + ySep * 3, xSep, ySep);
            this.AddSubview(currentAccX);

            currentAccY.setValue(0);
            currentAccY.Frame = new CoreGraphics.CGRect(x + 1 * xSep, y + ySep * 3, xSep, ySep);
            this.AddSubview(currentAccY);

            currentAccZ.setValue(0);
            currentAccZ.Frame = new CoreGraphics.CGRect(x + 2 * xSep, y + ySep * 3, xSep, ySep);
            this.AddSubview(currentAccZ);

            currentSpeedX.setValue(0);
            currentSpeedX.Frame = new CoreGraphics.CGRect(x+ 0* xSep, y + ySep * 4, xSep, ySep);
            this.AddSubview(currentSpeedX);

            currentSpeedY.setValue(0);
            currentSpeedY.Frame = new CoreGraphics.CGRect(x + 1 * xSep, y + ySep * 4, xSep, ySep);
            this.AddSubview(currentSpeedY);

            currentSpeedZ.setValue(0);
            currentSpeedZ.Frame = new CoreGraphics.CGRect(x + 2 * xSep, y + ySep * 4, xSep, ySep);
            this.AddSubview(currentSpeedZ);

            currentCorrrectionPitch.setValue(0);
            currentCorrrectionPitch.Frame = new CoreGraphics.CGRect(x + 0 * xSep, y + ySep * 5, xSep, ySep);
            this.AddSubview(currentCorrrectionPitch);

            currentCorrrectionRoll.setValue(0);
            currentCorrrectionRoll.Frame = new CoreGraphics.CGRect(x + 1 * xSep, y + ySep * 5, xSep, ySep);
            this.AddSubview(currentCorrrectionRoll);

            currentCorrrectionYaw.setValue(0);
            currentCorrrectionYaw.Frame = new CoreGraphics.CGRect(x + 2 * xSep, y + ySep * 5, xSep, ySep);
            this.AddSubview(currentCorrrectionYaw);

            currentPower.setValue(0);
            currentPower.Frame = new CoreGraphics.CGRect(x + 0 * xSep, y + ySep * 6, 2*xSep, ySep);
            this.AddSubview(currentPower);

            currentProcessorPerformace.Frame = new CoreGraphics.CGRect(x + 0 * xSep, y + ySep * 7, 2*xSep, ySep);
            this.AddSubview(currentProcessorPerformace);
        }



        public void updateCharts(List<double> parsedData)
        {
            double pitch = parsedData[1];
            double roll = parsedData[2];
            double yaw = parsedData[3];
            double alt = parsedData[12];

            currentPitch.setValue(parsedData[1]);
            currentRoll.setValue(parsedData[2]);
            currentYaw.setValue(parsedData[3]);

            currentAlt.setValue(parsedData[12]);
            currentLat.setValue(parsedData[20]);
            currentLong.setValue(parsedData[21]);


            currentGyroX.setValue(parsedData[4]);
            currentGyroY.setValue(parsedData[5]);
            currentGyroZ.setValue(parsedData[6]);

            currentAccX.setValue(parsedData[7]);
            currentAccY.setValue(parsedData[8]);
            currentAccZ.setValue(parsedData[9]);

            currentSpeedX.setValue(parsedData[13]);
            currentSpeedY.setValue(parsedData[14]);
            currentSpeedZ.setValue(parsedData[15]);

            currentCorrrectionPitch.setValue(parsedData[16]);
            currentCorrrectionRoll.setValue(parsedData[17]);
            currentCorrrectionYaw.setValue(parsedData[18]);

            currentProcessorPerformace.setValue(parsedData[0]);

            currentPower.setValue(80 + parsedData[19]);

            PitchHistory.AddNewValue(pitch);
            PitchAngle.AddNewValue(pitch);

            RollHistory.AddNewValue(roll);
            RollAngle.AddNewValue(roll);

            YawHistory.AddNewValue(yaw);
            YawAngle.AddNewValue(yaw);

            AltitudeHistory.AddNewValue(alt);
            Altitude.AddNewValue(alt);
        }

        
        private async void StartSliderTouched(object sender, EventArgs e)
        {
            UISlider currentSlider = sender as UISlider;
            if (currentSlider.Value == 1)
            {
                resetView();
                connectedVehicle.launch();

                await Task.Delay(2000);
                currentSlider.SetValue(0, true);
            }
            else
            {
                currentSlider.SetValue(0, true);
            }
        }

        private async void GetSensorValuesPressed(object sender, EventArgs e)
        {
            resetView();
            for ( int i = 0; i < 15; i++)
            {
                connectedVehicle.getSensorData();
                await Task.Delay(1000);
            }
        }

        private void resetView()
        {
            currentPitch.resetRingBuffer();
            currentRoll.resetRingBuffer();
            currentYaw.resetRingBuffer();

            currentAlt.resetRingBuffer();
            currentLat.resetRingBuffer();
            currentLong.resetRingBuffer();


            currentGyroX.resetRingBuffer();
            currentGyroY.resetRingBuffer();
            currentGyroZ.resetRingBuffer();

            currentAccX.resetRingBuffer();
            currentAccY.resetRingBuffer();
            currentAccZ.resetRingBuffer();

            currentSpeedX.resetRingBuffer();
            currentSpeedY.resetRingBuffer();
            currentSpeedZ.resetRingBuffer();

            currentCorrrectionPitch.resetRingBuffer();
            currentCorrrectionRoll.resetRingBuffer();
            currentCorrrectionYaw.resetRingBuffer();

            currentProcessorPerformace.resetRingBuffer();

            currentPower.resetRingBuffer();
        }

    }
}
