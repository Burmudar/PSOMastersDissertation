import numpy as np
import matplotlib
matplotlib.use('PDF')
import matplotlib.pyplot as plt
import matplotlib.mlab as mlab
import matplotlib.cbook as cbook
import os

def GetCSVFileList(path=None):
	listPath = path or os.getcwd()#If no path is given, use current working directory
	fileList = os.listdir(os.getcwd())
	csvList = [csvfile for csvfile in fileList if csvfile.endswith(".csv")]
	return csvList

def createCSVDataListFrom(path):
	csvFile = open(path,'r')
	csvData = []
	for line in csvFile:
		csvData.append(line.strip())
	return csvData

def createCSVTupleForNPFrom(csvData):
	x = []
	y = []
	csvTuple = {}
	for csvline in csvData:
		splittedLine = csvline.split(',')
		x.append(splittedLine[0])
		y.append(splittedLine[1])
	csvTuple['x'] = x
	csvTuple['y'] = y
	return csvTuple

def extractTitle(filename):
	parts = filename.split('(')
	benchname = parts[0].split('-')
	method = parts[1].strip(')')
	if method == "ParticlePerTrxFunction":
		method = "Velocity Method 1"
	else:
		method = "Velocity Method 2"
	gbest = parts[2].strip(')')
	if gbest == "BuildGBestFromTrxs":
		gbest = "GBest from Trx's"
	elif gbest == "BuildGBestFromCells":
		gbest = "GBest from Cells"
	else:
		gbest = "Std Gbest"
	pop = parts[3].split(')')
	return str(benchname[1])+"-"+str(method)+"-"+str(gbest)+"-"+str(pop[0])

def plotGraph(filename, csvTuple,bestFile):
	x = np.array(csvTuple['x'])
	y = np.array(csvTuple['y'])
	fig = plt.figure()
	fig.clear()
	title = extractTitle(filename)
	plt.title(title)
	plt.xlabel("Iteration")
	plt.ylabel("Interference")
	plt.plot(x,y,figure=fig)
	
	plt.savefig(filename.strip('.csv'))

if __name__ == "__main__":
	csvList = GetCSVFileList()
	for csv in csvList:
		print "Creating graph for " + str(csv)
		csvData = createCSVDataListFrom(csv)
		data = createCSVTupleForNPFrom(csvData)
		plotGraph(str(csv),data)
		print "---- done ----"

