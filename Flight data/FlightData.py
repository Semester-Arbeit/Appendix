import numpy as np
import  sys
import csvFileParser
from complementaryFilter import  complementaryFilter
from plotly.subplots import make_subplots
import plotly.graph_objects as go
import plotly.express as px
import GPSFunctions


print("-------------------------Status Report----------------------------")

#rawData = csvFileParser.loadFile("/Volumes/NO NAME/LogFiles/FlightLog_2.csv",False,True)
rawData = csvFileParser.loadFile("/Volumes/TobysSSD/SemesterArbeit/Flightlogs/Dump1/FlightLog_2.csv",False,True)

print("Overview:")
print(f"            Number of entries           : {len(rawData['Time']):3d} (5000)")
print(f"            Number of parameters        : {len([*rawData]):3d} (20)")
print()


cycleTimeCutOff = 11000

numberOfOverTimes = 0
maxCycleTime = 0
minCycleTime = sys.maxsize
averageTime = 0

averageLineLenght = 0
maxLineLenght = 0
minLineLenght = sys.maxsize

timeInSeconds = [(micro/1000000)-rawData['Time'][0]/1000000 for micro in rawData['Time']]

CycleTimes = []
CycleTimes.append(0)

for i in range(len(rawData['Time'][:-1])):
    averageTime += rawData['Time'][i + 1] - rawData['Time'][i]
    CycleTimes.append(rawData['Time'][i + 1] - rawData['Time'][i])
    if (rawData['Time'][i + 1] - rawData['Time'][i]) > maxCycleTime:
        maxCycleTime = (rawData['Time'][i + 1] - rawData['Time'][i])
    elif (rawData['Time'][i + 1] - rawData['Time'][i]) < minCycleTime:
        minCycleTime = (rawData['Time'][i + 1] - rawData['Time'][i])
    elif (rawData['Time'][i + 1] - rawData['Time'][i]) >= cycleTimeCutOff:
        numberOfOverTimes += 1

averageTime /= len(rawData['Time'][:-1])


print("Time Report:")
print(f"            Average                     : {averageTime / 1000:<7.3f}ms -> {1 / (averageTime / 1000000)} hz -> Flight Time: {(averageTime / 1000000) * 5000}s")
print(f"            MaxCycle                    : {maxCycleTime:<7}us -> {maxCycleTime / 1000:.2f}ms")
print(f"            MinCycle                    : {minCycleTime:<7}us -> {minCycleTime / 1000:.2f}ms")
print(f"            NumberOfOver                : {numberOfOverTimes} > {cycleTimeCutOff}")
print()

fig_Time = px.line(x=timeInSeconds,y=CycleTimes)
fig_Time.update_xaxes(title = "flight time[s]")
fig_Time.update_yaxes(title = "cycle time[us]")
fig_Time.show()

#Complementary Filter:
alphaX = 0.01
driftX = 0.003

alphaY = 0.01
driftY = -0.021

alphaZ = 0.1
driftZ = 0.005

simulationXFilter = complementaryFilter(alphaX)
simulationYFilter = complementaryFilter(alphaY)
simulationZFilter = complementaryFilter(alphaZ)

speedXEstimate = []
errorXEstimateDrone = []
averageXSpeed = 0
averageXAcc = 0

speedYEstimate = []
errorYEstimateDrone = []
averageYSpeed = 0
averageYAcc = 0

speedZEstimate = []
errorZEstimateDrone = []
averageZSpeed = 0
averageZAcc = 0

for time,pos,acc in zip(rawData['Time'],rawData['latitude'],rawData['xAcc']):
    currentSpeed = simulationXFilter.newEstimate(time,acc,pos,driftX)
    speedXEstimate.append(currentSpeed)
    averageXSpeed += currentSpeed
    averageXAcc += acc


for time,estSpeed, speed in zip(rawData['Time'],speedXEstimate,rawData['xSpeed']):
    errorXEstimateDrone.append(speed-estSpeed)


for time,pos,acc in zip(rawData['Time'],rawData['longitude'],rawData['yAcc']):
    currentSpeed = simulationYFilter.newEstimate(time,acc,pos,driftY)
    speedYEstimate.append(currentSpeed)
    averageYSpeed += currentSpeed
    averageYAcc += acc


for time,estSpeed, speed in zip(rawData['Time'],speedYEstimate,rawData['ySpeed']):
    errorYEstimateDrone.append(speed-estSpeed)


for time,pos,acc in zip(rawData['Time'],rawData['alt'],rawData['zAcc']):
    currentSpeed = simulationZFilter.newEstimate(time,acc,pos,driftZ)
    speedZEstimate.append(currentSpeed)
    averageZSpeed += currentSpeed
    averageZAcc += acc


for time,estSpeed, speed in zip(rawData['Time'],speedZEstimate,rawData['zSpeed']):
    errorZEstimateDrone.append(speed-estSpeed)

averageXSpeed /= len(speedXEstimate)
averageXAcc /= len(speedXEstimate)

averageYSpeed /= len(speedYEstimate)
averageYAcc /= len(speedYEstimate)

averageZSpeed /= len(speedZEstimate)
averageZAcc /= len(speedZEstimate)

print("Filter Report:")
print(f"            X   Average Speed               : {averageXSpeed:<7.3f}m/s")
print(f"            X   Average Acceleration        : {averageXAcc:<7.3f}m/s^2")
print()
print(f"            Y   Average Speed               : {averageYSpeed:<7.3f}m/s")
print(f"            Y   Average Acceleration        : {averageYAcc:<7.3f}m/s^2")
print()
print(f"            Z   Average Speed               : {averageZSpeed:<7.3f}m/s")
print(f"            Z   Average Acceleration        : {averageZAcc:<7.3f}m/s^2")
print()

fig_ComplementarFilter = make_subplots(rows=4, cols=3, subplot_titles=("X Axis", "Y Axis", "Z Axis"))
fig_ComplementarFilter.add_trace(
    go.Scatter(x=timeInSeconds,y=rawData['xSpeed'], name = "Speed Drone"),
    row=1, col=1
)
fig_ComplementarFilter.update_yaxes(title_text ="v[m/s]", row=1, col = 1)
fig_ComplementarFilter.update_xaxes(title_text ="Flight time[s]", row=1, col = 1)

fig_ComplementarFilter.add_trace(
    go.Scatter(x=timeInSeconds,y=rawData['ySpeed'], name = "Speed Drone"),
    row=1, col=2
)
fig_ComplementarFilter.update_yaxes(title_text ="v[m/s]", row=1, col = 2)
fig_ComplementarFilter.update_xaxes(title_text ="Flight time[s]", row=1, col = 2)

fig_ComplementarFilter.add_trace(
    go.Scatter(x=timeInSeconds,y=rawData['zSpeed'], name = "Speed Drone"),
    row=1, col=3
)
fig_ComplementarFilter.update_yaxes(title_text ="v[m/s]", row=1, col = 3)
fig_ComplementarFilter.update_xaxes(title_text ="Flight time[s]", row=1, col = 3)

fig_ComplementarFilter.add_trace(
    go.Scatter(x=timeInSeconds,y=speedXEstimate, name = "Speed Simulation"),
    row=2, col=1
)
fig_ComplementarFilter.update_yaxes(title_text ="v[m/s]", row=2, col = 1)
fig_ComplementarFilter.update_xaxes(title_text ="Flight time[s]", row=2, col = 1)

fig_ComplementarFilter.add_trace(
    go.Scatter(x=timeInSeconds,y=speedYEstimate, name = "Speed Simulation"),
    row=2, col=2
)
fig_ComplementarFilter.update_yaxes(title_text ="v[m/s]", row=2, col = 2)
fig_ComplementarFilter.update_xaxes(title_text ="Flight time[s]", row=2, col = 2)

fig_ComplementarFilter.add_trace(
    go.Scatter(x=timeInSeconds,y=speedZEstimate, name = "Speed Simulation"),
    row=2, col=3
)
fig_ComplementarFilter.update_yaxes(title_text ="v[m/s]", row=2, col = 3)
fig_ComplementarFilter.update_xaxes(title_text ="Flight time[s]", row=2, col = 3)

fig_ComplementarFilter.add_trace(
    go.Scatter(x=timeInSeconds,y=rawData['xAcc'], name = "Measured Acceleration"),
    row=3, col=1
)
fig_ComplementarFilter.update_yaxes(title_text ="a[m/s^2]", row=3, col = 1)
fig_ComplementarFilter.update_xaxes(title_text ="Flight time[s]", row=3, col = 1)

fig_ComplementarFilter.add_trace(
    go.Scatter(x=timeInSeconds,y=rawData['yAcc'], name = "Measured Acceleration"),
    row=3, col=2
)
fig_ComplementarFilter.update_yaxes(title_text ="a[m/s^2]", row=3, col = 2)
fig_ComplementarFilter.update_xaxes(title_text ="Flight time[s]", row=3, col = 2)

fig_ComplementarFilter.add_trace(
    go.Scatter(x=timeInSeconds,y=rawData['zAcc'], name = "Measured Acceleration"),
    row=3, col=3
)
fig_ComplementarFilter.update_yaxes(title_text ="a[m/s^2]", row=3, col = 3)
fig_ComplementarFilter.update_xaxes(title_text ="Flight time[s]", row=3, col = 3)

fig_ComplementarFilter.add_trace(
    go.Scatter(x=timeInSeconds,y=rawData['PosXError'], name = "Measured Position"),
    row=4, col=1
)
fig_ComplementarFilter.update_yaxes(title_text ="PosXError[m]", row=4, col = 1)
fig_ComplementarFilter.update_xaxes(title_text ="Flight time[s]", row=4, col = 1)

fig_ComplementarFilter.add_trace(
    go.Scatter(x=timeInSeconds,y=rawData['PosYError'], name = "Measured Position"),
    row=4, col=2
)
fig_ComplementarFilter.update_yaxes(title_text ="PosYError[m]", row=4, col = 2)
fig_ComplementarFilter.update_xaxes(title_text ="Flight time[s]", row=4, col = 2)

fig_ComplementarFilter.add_trace(
    go.Scatter(x=timeInSeconds,y=rawData['alt'], name = "Measured Position"),
    row=4, col=3
)
fig_ComplementarFilter.update_yaxes(title_text ="Altitude[m]", row=4, col = 3)
fig_ComplementarFilter.update_xaxes(title_text ="Flight time[s]", row=4, col = 3)



fig_ComplementarFilter.update_layout(title_text=f"Complementary Filter Analysis:   Filter X: alpha ={alphaX}, driftX ={driftX}     Filter Y: alpha ={alphaY}, driftX ={driftY}     Filter Z: alpha ={alphaZ}, driftX ={driftZ}")
fig_ComplementarFilter.show()
#End Complementary Filter:

#Attitude Visulataion

averagePitch = 0
averageRoll =0
averageYaw = 0

averageGyroX = 0
averageGyroY = 0
averageGyroZ = 0

averageFlapPitch = 0
averageFlapRoll = 0
averageFlapYaw = 0

for pitch,roll,yaw,gyroX,gyroY,gyroZ,FPitch,FRoll,FYaw in zip(rawData['Pitch'],rawData['Roll'],rawData['Yaw'],rawData['xGyro'],rawData['yGyro'],rawData['ZGyro'],rawData['cP'],rawData['cR'],rawData['cY']):
    averagePitch += pitch
    averageRoll +=roll
    averageYaw += yaw
    averageGyroX += gyroX
    averageGyroY += gyroY
    averageGyroZ += gyroZ
    averageFlapPitch += FPitch
    averageFlapRoll += FRoll
    averageFlapYaw += FYaw

averagePitch /= len(rawData['Pitch'])
averageRoll /= len(rawData['Pitch'])
averageYaw /= len(rawData['Pitch'])
averageGyroX /= len(rawData['Pitch'])
averageGyroY /= len(rawData['Pitch'])
averageGyroZ /= len(rawData['Pitch'])
averageFlapPitch /= len(rawData['Pitch'])
averageFlapRoll /= len(rawData['Pitch'])
averageFlapYaw /= len(rawData['Pitch'])

print("Attitude Report:")
print(f"            Pitch Average                : {averagePitch:<7.3f}°")
print(f"            Roll  Average                : {averageRoll:<7.3f}°")
print(f"            Yaw   Average                : {averageYaw:<7.3f}°")
print()
print(f"            GyroX Average                : {averageGyroX:<7.3f}°/s")
print(f"            GyroY Average                : {averageGyroY:<7.3f}°/s")
print(f"            GyroZ Average                : {averageGyroZ:<7.3f}°/s")
print()
print(f"            Flap Pitch Average            : {averageFlapPitch:<7.3f}°")
print(f"            Flap Roll Average             : {averageFlapRoll:<7.3f}°")
print(f"            Flap Yaw Average              : {averageFlapYaw:<7.3f}°")
print()

fig_Attitude = make_subplots(rows=3, cols=3, subplot_titles=("Pitch", "Roll", "Yaw"))

fig_Attitude.add_trace(
    go.Scatter(x=timeInSeconds,y=rawData['Pitch'], name = "Angle Pitch"),
    row=1, col=1
)
fig_Attitude.update_yaxes(title_text ="dec[°]", row=1, col = 1)
fig_Attitude.update_xaxes(title_text ="Flight time[s]", row=1, col = 1)

fig_Attitude.add_trace(
    go.Scatter(x=timeInSeconds,y=rawData['Roll'], name = "Angle Roll"),
    row=1, col=2
)
fig_Attitude.update_yaxes(title_text ="dec[°]", row=1, col = 2)
fig_Attitude.update_xaxes(title_text ="Flight time[s]", row=1, col = 2)

fig_Attitude.add_trace(
    go.Scatter(x=timeInSeconds,y=rawData['Yaw'], name = "Angle Yaw"),
    row=1, col=3
)
fig_Attitude.update_yaxes(title_text ="dec[°]", row=1, col = 3)
fig_Attitude.update_xaxes(title_text ="Flight time[s]", row=1, col = 3)

fig_Attitude.add_trace(
    go.Scatter(x=timeInSeconds,y=rawData['xGyro'], name = "Gyro Pitch"),
    row=2, col=1
)
fig_Attitude.update_yaxes(title_text ="dec[°/s]", row=2, col = 1)
fig_Attitude.update_xaxes(title_text ="Flight time[s]", row=2, col = 1)

fig_Attitude.add_trace(
    go.Scatter(x=timeInSeconds,y=rawData['yGyro'], name = "Gyro Roll"),
    row=2, col=2
)
fig_Attitude.update_yaxes(title_text ="dec[°/s]", row=2, col = 2)
fig_Attitude.update_xaxes(title_text ="Flight time[s]", row=2, col = 2)

fig_Attitude.add_trace(
    go.Scatter(x=timeInSeconds,y=rawData['ZGyro'], name = "Gyro Yaw"),
    row=2, col=3
)
fig_Attitude.update_yaxes(title_text ="dec[°/s]", row=2, col = 3)
fig_Attitude.update_xaxes(title_text ="Flight time[s]", row=2, col = 3)

fig_Attitude.add_trace(
    go.Scatter(x=timeInSeconds,y=rawData['cP'], name = "Flaps Angle Pitch"),
    row=3, col=1
)
fig_Attitude.update_yaxes(title_text ="dec[°]", row=3, col = 1)
fig_Attitude.update_xaxes(title_text ="Flight time[s]", row=3, col = 1)

fig_Attitude.add_trace(
    go.Scatter(x=timeInSeconds,y=rawData['cR'], name = "Flaps Angle Roll"),
    row=3, col=2
)
fig_Attitude.update_yaxes(title_text ="dec[°]", row=3, col = 2)
fig_Attitude.update_xaxes(title_text ="Flight time[s]", row=3, col = 2)

fig_Attitude.add_trace(
    go.Scatter(x=timeInSeconds,y=rawData['cY'], name = "Flaps Angle Yaw"),
    row=3, col=3
)
fig_Attitude.update_yaxes(title_text ="dec[°]", row=3, col = 3)
fig_Attitude.update_xaxes(title_text ="Flight time[s]", row=3, col = 3)

fig_Attitude.update_layout(title_text=f"Attitude Analysis:")
fig_Attitude.show()
#End Attitude Visulataion

#Location Visulataion
print("Location Report:")

target_latitude = 47.22385655150614
target_longitude = 8.817799529349683

latitude = [l for l in rawData['latitude']]
longitude = [l for l in rawData['longitude']]
color = [i for i in range(len(rawData['latitude']))]

latitudeErrorsInTransmission = [i for i in rawData['latitude'] if i < 0.22]
longitudeErrorsInTransmission = [i for i in rawData['longitude'] if i < 0.81]

average_error_ValidData = 0
max_error_ValidData = 0
min_error_ValidData = sys.maxsize
numberOfValidDatapoints = 0
EstimateDistancePlotX = []
EstimateDistancePlotY = []
EstimateDistancePlotColor = []

for lat,lon,posX,poxY in zip(latitude,longitude,rawData['PosXError'],rawData['PosYError']):
    if lat >= 47.00 and lon >= 8.00:
        numberOfValidDatapoints += 1
        res = GPSFunctions.fastCalculation([target_latitude,target_longitude],[lat,lon])
        fast_res = np.sqrt(posX**2 + poxY**2)
        EstimateDistancePlotX.append(numberOfValidDatapoints)
        EstimateDistancePlotY.append(res)
        EstimateDistancePlotColor.append("Best")

        EstimateDistancePlotX.append(numberOfValidDatapoints)
        EstimateDistancePlotY.append(fast_res)
        EstimateDistancePlotColor.append("Fast")

        EstimateDistancePlotX.append(numberOfValidDatapoints)
        EstimateDistancePlotY.append(res-fast_res)
        EstimateDistancePlotColor.append("Error")

        average_error_ValidData += res
        if res > max_error_ValidData:
            max_error_ValidData = res
        if res < min_error_ValidData:
            min_error_ValidData = res

average_error_ValidData /= numberOfValidDatapoints

print(f"            Errors in latitude            : {len(latitudeErrorsInTransmission):<7d}")
print(f"            Errors in longitude           : {len(longitudeErrorsInTransmission):<7d}")
print()

print(f"            Average error valid data       : {average_error_ValidData:<7.3f}m")
print(f"            Max error valid data           : {max_error_ValidData:<7.3f}m")
print(f"            Min error valid data           : {min_error_ValidData:<7.3f}m")
print(f"            Number of valid data           : {numberOfValidDatapoints:<7d}")
print()


fig_gpsTrack = px.scatter_mapbox(lat=latitude, lon=longitude,color=color,
                  color_continuous_scale=px.colors.cyclical.IceFire, size_max=15, zoom=10,
                  mapbox_style="carto-positron")

fig_gpsTrack.show()

fig_distPlot = px.line(x=EstimateDistancePlotX,y=EstimateDistancePlotY,color=EstimateDistancePlotColor)
fig_distPlot.show()

print("------------------------------Done--------------------------------")