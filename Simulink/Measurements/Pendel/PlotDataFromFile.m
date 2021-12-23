function [] = PlotDataFromFile(filenameOhneEndung)

%% load new Data

% data from file
[time,sensPitch,sensRoll,sensYaw,xGyro,yGyro,zGyro,xAcc,yAcc,zAcc,latitude,longitude,altitude] = readvars(filenameOhneEndung + ".csv");
% calc loopTime
loopTime = zeros(length(time)-1,1);
for i = 1:size(loopTime)
    loopTime(i)= time(i+1)-time(i);
end

setupTime = time(1);
meanLoopTime = mean(loopTime);
varLoopTime = var(loopTime);

% calc integrated XYZ Speed from Accelerator


%% Rotation
figure('Name',"Flight: "+filenameOhneEndung,'NumberTitle','off');
subplot(4,1,1);
title('Axis Position')
hold on
plot(time, sensRoll)
plot(time, sensPitch)
plot(time, sensYaw)
legend('sensRoll','sensPitch','sensYaw')


subplot(4,1,2);
title('Axis Acceleration')
hold on
plot(time, xGyro)
plot(time, yGyro)
plot(time, zGyro)
legend('Roll','Pitch','Yaw')

%% Position


subplot(4,1,3);
title('Spacial Position')
hold on
plot(time, latitude)
plot(time, longitude)
plot(time, altitude)
legend('latitude','longitude','altitude')


subplot(4,1,4);
title('Spacial Acceleration')
hold on
plot(time, xAcc)
plot(time, yAcc)
plot(time, zAcc)
legend('xAcc','yAcc','zAcc')


end



