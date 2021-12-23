import numpy as np


def calculacteDistance(pointOne, pointTwo):
    r = 6371000
    phi1 = pointOne[0] * (np.pi/180)
    phi2 = pointTwo[0] * (np.pi/180)
    deltaPhi = (pointTwo[0] - pointOne[0]) * (np.pi/180)
    deltaLambda = (pointTwo[1] - pointOne[1]) * (np.pi/180)

    a = np.sin(deltaPhi/2) * np.sin(deltaPhi/2) + np.cos(phi1) * np.cos(phi2) * np.sin(deltaLambda/2) * np.sin(deltaLambda/2)

    c = 2 * np.arctan2(np.sqrt(a), np.sqrt(1-a))

    return r*c


def fastCalculation(pointOne,pointTwo):
    d = 2*6371000

    k = (d*np.pi)/360
    k = 111194.9266

    latDist = pointTwo[0] - pointOne[0]
    lonDist = pointTwo[1] - pointOne[1]

    return np.sqrt((k*latDist)**2 + (k*lonDist)**2)

