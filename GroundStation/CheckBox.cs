using UIKit;
using System;
using System.Collections.Generic;

namespace GroundStation
{
    public class CheckBox : UIView
    {
        private UISwitch checkSwitch = new UISwitch();
        private UIButton autoCheckButton = new UIButton();
        private UILabel switchText = new UILabel();
        private UIView switchBackground = new UIView();
        private UIActivityIndicatorView autoCheckActivityIndicator = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.White);

        private UIView autoCheckButtonBackground = new UIView();

        private DuctedFanPrototype connectedVehicle;

        private bool checkStatus = false;
        
        
        public CheckBox(string text, CoreGraphics.CGRect frameNew, DuctedFanPrototype connectedVehicle, bool withAutoTest = false)
        {
            this.Frame = frameNew;
            this.connectedVehicle = connectedVehicle;
            switchBackground.Frame = new CoreGraphics.CGRect(0, 0, this.Frame.Width, this.Frame.Height);
            switchBackground.BackgroundColor = UIColor.SecondarySystemBackgroundColor;              
            this.AddSubview(switchBackground);

            switchText.Frame = new CoreGraphics.CGRect(10, 0, this.Frame.Width - 120, this.Frame.Height);
            
            switchText.Text = text;
            this.AddSubview(switchText);
            if(withAutoTest)
            {
                autoCheckButton.Frame = new CoreGraphics.CGRect(this.Frame.Width - 70, 9, 60, this.Frame.Height-18);
                autoCheckButton.SetTitle("Check", UIControlState.Normal);

                autoCheckButton.AddTarget(makeAutoCheck, UIControlEvent.TouchDown);
                this.AddSubview(autoCheckButton);

                autoCheckButtonBackground.Frame = new CoreGraphics.CGRect(this.Frame.Width - 70, 9, 60, this.Frame.Height - 18);
                autoCheckButtonBackground.BackgroundColor = UIColor.FromRGBA(100, 100, 100, 100);
                autoCheckButtonBackground.Layer.CornerRadius = 10;
                autoCheckButtonBackground.Layer.ShadowColor = new CoreGraphics.CGColor(0, 0, 0, 1);
                autoCheckButtonBackground.Layer.ShadowOffset = new CoreGraphics.CGSize(5, 5);
                autoCheckButtonBackground.Layer.ShadowOpacity = 0.2F;
                this.AddSubview(autoCheckButtonBackground);
                this.SendSubviewToBack(autoCheckButtonBackground);

                autoCheckActivityIndicator.Frame = new CoreGraphics.CGRect(this.Frame.Width - 70 + (this.Frame.Height - 18), 9, this.Frame.Height - 18, this.Frame.Height - 18);
                autoCheckActivityIndicator.Alpha = 0;
                this.AddSubview(autoCheckActivityIndicator);
            }
            else
            {
                checkSwitch.Frame = new CoreGraphics.CGRect(this.Frame.Width - 60, 9, 0, 0);
                checkSwitch.AddTarget(elementHasBeenChecked, UIControlEvent.ValueChanged);
                this.AddSubview(checkSwitch);
            }


            this.SendSubviewToBack(switchBackground);



        }

        private void elementHasBeenChecked(object sender, EventArgs e)
        {
            UISwitch uISwitch = sender as UISwitch;
            if(uISwitch.On)
            {
                switchBackground.BackgroundColor = UIColor.SystemGreenColor;
                checkStatus = true;
            }
            else
            {
                switchBackground.BackgroundColor = UIColor.SystemRedColor;
                checkStatus = false;
            }

        }

        private void makeAutoCheck(object sender, EventArgs e)
        {
            UIButton uIButton = sender as UIButton;
            autoCheckActivityIndicator.StartAnimating();
            UIView.Animate(0.5, () => {
                autoCheckButtonBackground.Frame = new CoreGraphics.CGRect(this.Frame.Width - 70 - (this.Frame.Height - 18), 9, 60 + (this.Frame.Height - 18), this.Frame.Height - 18);
                autoCheckButton.Frame = new CoreGraphics.CGRect(this.Frame.Width - 70 - (this.Frame.Height - 18), 9, 60, this.Frame.Height - 18);
                autoCheckActivityIndicator.Frame = new CoreGraphics.CGRect(this.Frame.Width - 70 + (this.Frame.Height - 18)-5, 9, this.Frame.Height - 18, this.Frame.Height - 18);
                autoCheckActivityIndicator.Alpha = 1;
            });
            connectedVehicle.getRocketStatus();
            uIButton.Enabled = false;
        }



        public void updateAutoComplete(TelemetryData telemetry)
        {
            autoCheckActivityIndicator.StopAnimating();
            checkStatus = parseTelemetry(telemetry);

            UIView.Animate(0.5, () => {

                autoCheckActivityIndicator.Alpha = 0;
                autoCheckButtonBackground.Frame = new CoreGraphics.CGRect(this.Frame.Width - 70, 9, 60, this.Frame.Height - 18);
                autoCheckButton.Frame = new CoreGraphics.CGRect(this.Frame.Width - 70, 9, 60, this.Frame.Height - 18);
            });

            if (checkStatus)
            {
                switchBackground.BackgroundColor = UIColor.SystemGreenColor;
            }
            else
            {
                switchBackground.BackgroundColor = UIColor.SystemRedColor;
            }
        }

        public bool parseTelemetry(TelemetryData telemetry)
        {
            Console.WriteLine(telemetry.rawData);
            var splitedData = telemetry.rawData.Split(',');
            for(int i =0; i < splitedData.Length-1; i++)
            {
                if(splitedData[i] != "1")
                {
                    return false;
                }
            }
            return true;
        }

        public bool isChecked()
        {
            if(checkStatus)
            {
                return true;
            }
            else
            {
                switchBackground.BackgroundColor = UIColor.SystemRedColor;
                return false;
            }
        }
    }
}