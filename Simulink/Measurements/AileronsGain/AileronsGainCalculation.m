close
clc

run("getAileronsData.m")

figure('Name',"Force of two Ailerons",'NumberTitle','off');
hold on
plot(angles1,F_flap1,"-*",'DisplayName','#1 Engine:80% ')
plot(angles2,F_flap2,"-*",'DisplayName','#2 Engine:80% ')
%plot(angles3,F_flap3,"-*",'DisplayName','#3 Engine:100% ')
plot(angles4,F_flap4,"-*",'DisplayName','#3 Engine:90% ')
%plot(angles5,F_flap5,"-*",'DisplayName','#5 Engine:95% ')

grid on

x=1:60;


ang=[angles1(1,[1:3]) angles2(1,1:7)].';
Nm= [F_flap1(1,[1:3]) F_flap2(1,1:7)].';

flapgain  = (ang.'*ang\ang.'*Nm)

yLS= x.*flapgain;
plot(x,yLS,"--",'DisplayName','Linear Aprox.')
legend()
ylabel('Force of flap [N]')
xlabel('Flap angle [ ° ]')
title('FlapGain measurement')