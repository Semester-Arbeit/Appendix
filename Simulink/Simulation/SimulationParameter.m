
%%  Physical Parameter
%gopro with handle 0.192 kg
%gopro lever roll 0.15 m
%gopro lever pitch 0.22 m
%gopro lever yaw 0.14 m
%PitchTraegheit = 0.0362;   %gemessen 29.10 
%RollTraegheit = 0.0430;    %gemessen 29.10
%YawTraegheit = 0.0335;     %gemessen 29.10
%ObjectMass = 3.34;         %gemessen 29.10




Gravitation = 9.8067;                   %nach metas.ch... gravitationszonen schweiz
ObjectMass = 3.34+0.129;                %mit GoPro
RollTraegheit = 0.0430+(0.15)^2*0.129;  %mit GoPro
PitchTraegheit = 0.0362+(0.22)^2*0.129; %mit GoPro
YawTraegheit = 0.0335+(0.14)^2*0.129;   %mit GoPro

PropOffset = 0.09; %Abstand von Propeller zu Massepunkt
PitchTraegheit_shift=PitchTraegheit+ObjectMass*PropOffset^2;
RollTraegheit_shift=RollTraegheit+ObjectMass*PropOffset^2;



PropGain = -(ObjectMass*Gravitation)/0.8; % prop signal * Propgain = auftriebKraft .. Annahme bei 0.8 schwebend

%DriftGain = 1; %  wieviel Kraft generiert eine Schieflage in rad
FlapGain = 0.0671; % (abweichung von 0 pos in deg) * FlapGain = Drehmoment auf Objekt @ 0.8 schub
MaxFlapAngle = 25;
MaxTiltActuation = 10;

SimParam.Physics = table(Gravitation,ObjectMass,PitchTraegheit,RollTraegheit,YawTraegheit,PitchTraegheit_shift,RollTraegheit_shift,PropOffset,PropGain,FlapGain,MaxFlapAngle,MaxTiltActuation);
clear("Gravitation","ObjectMass","PitchTraegheit","RollTraegheit","YawTraegheit","PitchTraegheit_shift","RollTraegheit_shift","PropOffset","PropGain","FlapGain","MaxFlapAngle","MaxTiltActuation")



%% Regler Parameter
KiX = 0.6;
KpX = 1.2;
KiY = 0.6;
KpY = 0.9;
KiZ = 60;
KpZ = 10;
KiRoll = 1;
KpRoll = 1;
KiPitch = 1.4;
KpPitch = 1.5 ;
KiYaw = 1;
KpYaw = 1;


SimParam.Controler = table(KiX,KpX,KiY,KpY,KiZ,KpZ,KiRoll,KpRoll,KiPitch,KpPitch,KiYaw,KpYaw);

clear("KpX","KiX","KpY","KiY","KpZ","KiZ","KpPitch","KiPitch","KpRoll","KiRoll","KpYaw","KiYaw")
%run ../Data/Parameter/RegPar_15_12.m

%% StepOn
         %X  Y  Z Roll Pitch Yaw    
SimParam.Step = array2table(zeros(1,6));
SimParam.Step.Properties.VariableNames={'X','Y','Z','Roll','Pitch','Yaw'};


%% Disturbance

SimParam.Disturbance= array2table(zeros(1,8));
SimParam.Disturbance.Properties.VariableNames={'X','Y','Z','Roll','Pitch','Yaw','stepON','stepOFF'};
SimParam.Disturbance.stepON=10;
SimParam.Disturbance.stepOFF=11;


%% Update SimParam




