    flapLever = 0.165; %  Abstand zwischen Flap und Aufhängung
    %% Experiment 1
    % engine 80
    % Akku nicht ganz voll
    % Hebelarm = 0.525m  achse war 0.045m über schwerpunkt --> 
    leverage1 =  0.525;
    angles1 = [0 10       20       30       40       50       60];
    forces1 = [0 0.5+1/20 0.5+9/20 1.5-2/20 1.5+4/20 1.5+4/20 1+6/20];
    F_flap1 = forces1.*(leverage1/(flapLever*2));
    %% Experiment 2
    % engine 80
    % Akku stand nach Experiment1 (zwischen 1&2 wurde nicht geladen)
    % Hebelarm = 0.525m achse war 0.045m über schwerpunkt -->
    leverage2 =  0.525-0.045;
    angles2 = [ 5        10       15       20     25     30     35       40       45       50];
    forces2 = [ 0.5-3/20 0.5 0.5+5/20 1-1/20 1+2/20 1+6/20 1.5+1/20 1.5+3/20 1.5+5/20 1.5+5/20];
    F_flap2 = forces2.*(leverage2/(flapLever*2));

    %% Experiment 3
    % aufbau wieder aufgebaut
    % engine 100
    % akku: einer voll der andere fehlt 45min ladung
    % Hebelarm: 0.53m achse war 0.045 über schwerpunkt -->
    leverage3 = 0.53-0.045;
    angles3 = [ 5        10       15     20     25       30       35       40     45     50];
    forces3 = [ 0.5+0/20 0.5+1/20 1+0/20 1+5/20 1.5+0/20 1.5+4/20 1.5+9/20 2+0/20 2+2/20 2+1/20];
    F_flap3 = forces3.*(leverage3/(flapLever*2));
    %% Experiment 4
    % engine 90
    % akku: nach #3
    % Hebelarm: 0.53m achse war 0.045 über schwerpunkt -->
    leverage4 = 0.53-0.045;
    angles4 = [ 0 5        10       15       20     25       30       35       40       45     50];
    forces4 = [ 0 0.5+0/20 0.5+4/20 0.5+9/20 1+3/20 1+9/20 1.5+0/20 1.5+5/20 1.5+8/20 2+0/20 1.5+9/20];
    F_flap4 = forces4.*(leverage4/(flapLever*2));
    %% Experiment 5
    % engine 95
    % akku: nach #4
    % Hebelarm: 0.53m achse war 0.045 über schwerpunkt -->
    leverage5 = 0.53-0.045;
    angles5 = [ 5        10       15       20     25     30];
    forces5 = [ 0.5+1/20 0.5+4/20 0.5+9/20 1+2/20 1+5/20 1+1/20 ];
    F_flap5 = forces5.*(leverage5/(flapLever*2));



