clc
close all
run SimulationParameter.m
SimParam.Step.Roll = 1;
SimParam.Step.Z = -1;










SF=Simulate;

close all 

% Behavioral
f1=figure;
f1.Position = [925,579,990,398];
grid on
hold on
legend()



plot(SF.Time,SF.state.Xpos,'Color',[0.6350 0.0780 0.1840],'DisplayName','Xpos')
plot(SF.Time,SF.state.Xspeed,'-.','Color',[0.6350 0.0780 0.1840],'DisplayName','Xspeed')
plot(SF.Time,SF.state.Xacc,'--','Color',[0.6350 0.0780 0.1840],'DisplayName','Xacc')

plot(SF.Time,SF.state.Ypos,'Color',[0.3010 0.7450 0.9330],'DisplayName','Ypos')
plot(SF.Time,SF.state.Yspeed,'-.','Color',[0.3010 0.7450 0.9330],'DisplayName','Yspeed')
plot(SF.Time,SF.state.Yacc,':','Color',[0.3010 0.7450 0.9330],'DisplayName','Yacc')

plot(SF.Time,SF.state.Zpos,'Color',[0.4660 0.6740 0.1880],'DisplayName','Zpos')
plot(SF.Time,SF.state.Zspeed,'-.','Color',[0.4660 0.6740 0.1880],'DisplayName','Zspeed')
plot(SF.Time,SF.state.Zacc,':','Color',[0.4660 0.6740 0.1880],'DisplayName','Zacc')

plot(SF.Time,SF.state.Rollpos,'Color',[0.8500 0.3250 0.0980],'DisplayName','Rollpos')
plot(SF.Time,SF.state.Rollspeed,'-.','Color',[0.8500 0.3250 0.0980],'DisplayName','Rollspeed')
plot(SF.Time,SF.state.Rollspeed,':','Color',[0.8500 0.3250 0.0980],'DisplayName','Rollspeed')

plot(SF.Time,SF.state.Pitchpos,'Color',[0.9290 0.6940 0.1250],'DisplayName','Pitchpos')
plot(SF.Time,SF.state.Pitchspeed,'-.','Color',[0.9290 0.6940 0.1250],'DisplayName','Pitchspeed')
plot(SF.Time,SF.state.Pitchacc,':','Color',[0.9290 0.6940 0.1250],'DisplayName','Pitchacc')

plot(SF.Time,SF.state.Yawpos,'Color',[0 0.4470 0.7410],'DisplayName','Yawpos')
plot(SF.Time,SF.state.Yawspeed,'-.','Color',[0 0.4470 0.7410],'DisplayName','Yawspeed')
plot(SF.Time,SF.state.Yawacc,':','Color',[0 0.4470 0.7410],'DisplayName','Yawspeed')



% Sollwerte
f2=figure;

f2.Position = [923,75,988,424];
subplot(2,1,1)
legend()
title('Aktor Werte')
hold on

plot(SF.Time,SF.stell.prop,'DisplayName','prop')
plot(SF.Time,SF.stell.flapXP+0.01,'DisplayName','flapXP')
plot(SF.Time,SF.stell.flapXN+0.02,'DisplayName','flapXN')
plot(SF.Time,SF.stell.flapYP+0.03,'DisplayName','flapYP')
plot(SF.Time,SF.stell.flapYN+0.04,'DisplayName','flapYN')

% Regler Berechnung

subplot(2,1,2)
legend()
title('Simulierter Regler')
hold on


plot(SF.Time,SF.Corr.Roll,'DisplayName','Roll')
plot(SF.Time,SF.Corr.Pitch+0.01,'DisplayName','Pitch')
plot(SF.Time,SF.Corr.Yaw+0.02,'DisplayName','Yaw')
plot(SF.Time,SF.Corr.X+0.03,'DisplayName','X')
plot(SF.Time,SF.Corr.Y+0.04,'DisplayName','Y')
plot(SF.Time,SF.Corr.Z+0.05,'DisplayName','Z')
