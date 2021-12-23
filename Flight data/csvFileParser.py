def loadFile(filepath = "/Volumes/NO NAME/LogFiles/FlightLog.csv",printRes = True, coordinaateConverter = True):
    Data = {}

    with open(filepath, 'r') as csvFile:
        headerList = [head.replace("\n","") for head in csvFile.readline().split(',')[:22]]
        lineLists = [[] for i in range(len(headerList))]
        for line in csvFile.read().split('\n'):
            for i, value in zip(range(len(line.split(',')[:22])),
                                line.split(',')[:22]):
                try:
                    lineLists[i].append(float(value))
                except:
                    print(f"            Error in Line {i}           value = ({value})")

        for i, header in zip(range(len(headerList)), headerList):
            Data.update({header: lineLists[i]})

    if printRes:
        for header in [*Data]:
            clean = header.rstrip("\n")
            print(f"{clean:>8s}  ", end="")
        print()
        for line in range(len(Data[[*Data][0]])):
            for dataName in [*Data]:

                print(f"{Data[dataName][line]:08.2f}  ", end="")
            print()
        print("Parsing Done")
    return Data