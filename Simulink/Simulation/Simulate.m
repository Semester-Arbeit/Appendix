function SF = Simulate()

out=sim("DuctedFanModell_V1.slx");


Tmp = timeseries2timetable(out.RM_State);
SF.Time = table(Tmp.Time);
SF.Time = SF.Time.Var1;
state = array2table(Tmp.Data(:,:));

state.Properties.VariableNames = {'Xpos','Xspeed','Xacc',...
                                  'Ypos','Yspeed','Yacc',...
                                  'Zpos','Zspeed','Zacc',...
                                  'Rollpos','Rollspeed','Rollacc',...
                                  'Pitchpos','Pitchspeed','Pitchacc',...
                                  'Yawpos','Yawspeed','Yawacc'};

SF.state = state;

Tmp = timeseries2timetable(out.stellgroessen);
stell = array2table(Tmp.Data(:,1:5));
stell.Properties.VariableNames = {'prop','flapYP','flapYN','flapXP','flapXN'};


SF.stell= stell;


Tmp= timeseries2timetable(out.SimRegler);
SF.Corr = array2table(Tmp.Data(:,1:6));
SF.Corr.Properties.VariableNames = {'X','Y','Z','Pitch','Roll','Yaw'};


%clear("Tmp","out")

end

