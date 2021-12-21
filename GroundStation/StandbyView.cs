using System;
using UIKit;
using System.Collections.Generic;
using NetworkExtension;
using System.Threading.Tasks;

namespace GroundStation
{
    public class StandByView : UIView
    {

        private DuctedFanPrototype connectedVehicle;

        private UILabel ConParamTitle = new UILabel();

        private UIButton sendButton = new UIButton();
        private UIButton loadParamButton = new UIButton();

        private UIActivityIndicatorView autoCheckActivityIndicator = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.Gray);

        private UIView backgroundLoadButton = new UIView();

        private UIView backgroundSendButton = new UIView();

        private List<ParameterConfig> listOfParameterConfigs = new List<ParameterConfig>();

        private List<List<String>> configurationParameters = new List<List<string>>();

        private UIProgressView sendParamProgressView = new UIProgressView();

        private UILabel calibrationTitle = new UILabel();

        private UIButton calibrationButton = new UIButton();
        private UIActivityIndicatorView calibrationProgressView = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.Gray);
        private UIView backgroundCalibrationButton = new UIView();

        public StandByView(CoreGraphics.CGRect Frame, DuctedFanPrototype connectedVehicle, int numberOfParameters = 12)
        {
            this.Frame = Frame;
            ConParamTitle.Text = "Control Parameters";
            ConParamTitle.Frame = new CoreGraphics.CGRect(0, 0, 200, 50);
            this.AddSubview(ConParamTitle);

            this.connectedVehicle = connectedVehicle;

            sendButton.SetTitle("Send", new UIControlState());
            sendButton.Frame = new CoreGraphics.CGRect(850, 110, 100, 50);
            sendButton.BackgroundColor = UIColor.SystemGreenColor;
            sendButton.Layer.CornerRadius = 10;
            sendButton.AddTarget(sendParameters, UIControlEvent.TouchDown);
            this.AddSubview(sendButton);

            sendParamProgressView.Frame = new CoreGraphics.CGRect(860, 140, 80, 5);
            sendParamProgressView.Alpha = 0;
            sendParamProgressView.SetProgress(0, false);
            sendParamProgressView.TrackTintColor = UIColor.TertiaryLabelColor;
            sendParamProgressView.ProgressTintColor = UIColor.White;
            this.AddSubview(sendParamProgressView);

            backgroundSendButton.Frame = new CoreGraphics.CGRect(850, 110, 100, 50);
            backgroundSendButton.BackgroundColor = UIColor.SystemGreenColor;
            backgroundSendButton.Layer.CornerRadius = 10;
            backgroundSendButton.Layer.ShadowColor = new CoreGraphics.CGColor(0, 0, 0, 1);
            backgroundSendButton.Layer.ShadowOffset = new CoreGraphics.CGSize(5, 5);
            backgroundSendButton.Layer.ShadowOpacity = 0.2F;
            this.AddSubview(backgroundSendButton);
            this.SendSubviewToBack(backgroundSendButton);

            loadParamButton.SetTitle("Load", new UIControlState());
            loadParamButton.Frame = new CoreGraphics.CGRect(850, 50, 100, 50);
            loadParamButton.BackgroundColor = UIColor.SystemBlueColor;
            loadParamButton.AddTarget(loadParamButtonPressed, UIControlEvent.TouchDown);
            loadParamButton.Layer.CornerRadius = 10;
            this.AddSubview(loadParamButton);

            autoCheckActivityIndicator.Frame = new CoreGraphics.CGRect(900, 50, 30, 50);
            autoCheckActivityIndicator.Alpha = 0;
            this.AddSubview(autoCheckActivityIndicator);

            backgroundLoadButton.Frame = new CoreGraphics.CGRect(850, 50, 100, 50);
            backgroundLoadButton.BackgroundColor = UIColor.SystemBlueColor;
            backgroundLoadButton.Layer.CornerRadius = 10;
            backgroundLoadButton.Layer.ShadowColor = new CoreGraphics.CGColor(0, 0, 0, 1);
            backgroundLoadButton.Layer.ShadowOffset = new CoreGraphics.CGSize(5, 5);
            backgroundLoadButton.Layer.ShadowOpacity = 0.2F;
            this.AddSubview(backgroundLoadButton);
            this.SendSubviewToBack(backgroundLoadButton);

            calibrationTitle.Text = "Calibration";
            calibrationTitle.Frame = new CoreGraphics.CGRect(0, 250, 200, 50);
            this.AddSubview(calibrationTitle);

            calibrationButton.SetTitle("Calibration", new UIControlState());
            calibrationButton.Frame = new CoreGraphics.CGRect(0, 300, 180, 50);
            calibrationButton.BackgroundColor = UIColor.SystemYellowColor;
            calibrationButton.Layer.CornerRadius = 10;
            calibrationButton.AddTarget(calibrateSystem, UIControlEvent.TouchDown);
            this.AddSubview(calibrationButton);

            calibrationProgressView.Frame = new CoreGraphics.CGRect(100, 300, 30, 50);
            calibrationProgressView.Alpha = 0;
            this.AddSubview(calibrationProgressView);

            backgroundCalibrationButton.Frame = new CoreGraphics.CGRect(0, 300, 180, 50);
            backgroundCalibrationButton.BackgroundColor = UIColor.SystemYellowColor;
            backgroundCalibrationButton.Layer.CornerRadius = 10;
            backgroundCalibrationButton.Layer.ShadowColor = new CoreGraphics.CGColor(0, 0, 0, 1);
            backgroundCalibrationButton.Layer.ShadowOffset = new CoreGraphics.CGSize(5, 5);
            backgroundCalibrationButton.Layer.ShadowOpacity = 0.2F;
            this.AddSubview(backgroundCalibrationButton);
            this.SendSubviewToBack(backgroundCalibrationButton);

            configurationParameters.Add(new List<string> { "Pitch Ki (p)", "Pitch Kp (v)", "XPos Ki (p)", "XPos Kp (v)" });
            configurationParameters.Add(new List<string> { "Roll  Ki (p)", "Roll  Kp (v)", "YPos Ki (p)", "YPos Kp (v)" });
            configurationParameters.Add(new List<string> { "Yaw   Ki (p)", "Yaw   Kp (v)", "Alt  Ki (p)", "Alt  Kp (v)" });

            for (int i = 0; i < numberOfParameters / 3; i++)
            {
                for (int m = 0; m < 3; m++)
                {
                    listOfParameterConfigs.Add(new ParameterConfig(configurationParameters[m][i], new CoreGraphics.CGRect(210 * i, (60 * m) + 50, 200, 50)));
                    this.AddSubview(listOfParameterConfigs[i*3 + m]);
                }
            }
        }

        private void loadParamButtonPressed(object sender, EventArgs e)
        {
            connectedVehicle.getParameters();
            UIView.Animate(0.5, () => {
                loadParamButton.Frame = new CoreGraphics.CGRect(850, 50, 60, 50);
                autoCheckActivityIndicator.Frame = new CoreGraphics.CGRect(910, 50, 30, 50);
                autoCheckActivityIndicator.Alpha = 1;
            });
            autoCheckActivityIndicator.StartAnimating();
        }

        private async void sendParameters(object sender, EventArgs e)
        {
            UIView.Animate(0.5, () => {
                sendButton.Frame = new CoreGraphics.CGRect(850, 110, 100, 40);
                sendParamProgressView.Frame = new CoreGraphics.CGRect(860, 145, 80, 5);
                sendParamProgressView.Alpha = 1;
                
            });

            UIView.Animate(1, () => {
                sendParamProgressView.SetProgress(50, true);
            });

            
            string newValues = "";
            foreach(ParameterConfig param in listOfParameterConfigs)
            {
                newValues += param.getTextFliedValue()+ ",";
            }
            connectedVehicle.sendControlParametersData(newValues.Remove(newValues.Length-1));

            await Task.Delay(2500);
            UIView.Animate(0.5, () => {
                sendButton.Frame = new CoreGraphics.CGRect(850, 110, 100, 50);
                sendParamProgressView.Frame = new CoreGraphics.CGRect(860, 140, 80, 5);
                sendParamProgressView.Alpha = 0;
                
            });
            loadParamButtonPressed(sender, e);
        }

        private async void calibrateSystem(object sender, EventArgs e)
        {
            connectedVehicle.calibrate();
            UIView.Animate(0.5, () => {
                calibrationButton.Frame = new CoreGraphics.CGRect(0, 300, 100, 50);
                calibrationProgressView.Frame = new CoreGraphics.CGRect(100, 300, 30, 50);
                calibrationProgressView.Alpha = 1;
            });

            calibrationProgressView.StartAnimating();
            await Task.Delay(10000);
            UIView.Animate(0.5, () => {
                calibrationButton.Frame = new CoreGraphics.CGRect(0, 300, 180, 50);
                calibrationProgressView.Frame = new CoreGraphics.CGRect(100, 300, 30, 50);
                calibrationProgressView.Alpha = 1;
            });
            calibrationProgressView.StopAnimating();

            loadParamButtonPressed(sender, e);
        }

        public void newParameters(TelemetryData telemetry)
        {
            UIView.Animate(0.5, () => {
                loadParamButton.Frame = new CoreGraphics.CGRect(850, 50, 100, 50);
                autoCheckActivityIndicator.Alpha = 0;
            });
            var splitedRocketTelemetry = telemetry.rawData.Split(',');
            if(listOfParameterConfigs.Count == splitedRocketTelemetry.Length)
            {
                for(int i = 0; i < listOfParameterConfigs.Count; i++)
                {
                    listOfParameterConfigs[i].setTextFieldValue(splitedRocketTelemetry[i]);
                }
            }
            else
            {
                Console.WriteLine("Error in transmittion");
            }
        }
    }
}