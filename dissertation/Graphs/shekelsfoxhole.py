from mpl_toolkits.mplot3d import Axes3D
from matplotlib import cm
from matplotlib.ticker import LinearLocator, FixedLocator, FormatStrFormatter
import matplotlib
matplotlib.use('PDF')
import matplotlib.pyplot as plt
import numpy as np

AMOUNT_OF_POINTS = 150

def insertValuesIntoMatrix(matrix,value,row,index,timesToInsert):
	if matrix.size / 2 >= index + timesToInsert:
		for i in range(timesToInsert):
			matrix[row,index+i] = value
	

def createDeJongF5Matrix():
	a = np.array([])
	a.resize(2,25)
	for i in range(2):
		value = -32
		for j in range(25):
			a[0,j] = value
			value = value + 16
			if j > 0 and (j+1) % 5 == 0:
				value = -32
				valueIndex = ((j + 1) / 5) - 1
				startIndex = valueIndex * 5
				insertValuesIntoMatrix(a,a[0,valueIndex],1,startIndex,5) 
	return a

def DeJongF5(x,y,matrix):
	sumj = 0;
	for j in range(25):
		sumi = 0
		sumi = (x - matrix[0,j])**6 + (y - matrix[1,j])**6
		sumj = sumj + (j + sumi)**-1
	return (0.002 + sumj)**-1


fig = plt.figure()
ax = Axes3D(fig)
X = np.linspace(-65.356, 65.356, AMOUNT_OF_POINTS)
Y = np.linspace(-65.356, 65.356, AMOUNT_OF_POINTS)
X, Y = np.meshgrid(X, Y)

dejongList = []
print 'Initializing Function'
deJongMatrix = createDeJongF5Matrix()
for i in range(AMOUNT_OF_POINTS):
	val = DeJongF5(X[i],Y[i],deJongMatrix)
	dejongList.append(val)
print '----Complete----'
print 'Create Numpy array from Function results'
Z = np.array(dejongList)
print '----Complete----'
print 'Plotting function'
surf = ax.plot_surface(X, Y, Z, rstride=1, cstride=1, cmap=cm.jet,
        linewidth=0, antialiased=False)
print '----Complete----'
print 'Setting various graph properties'

ax.set_zlim3d(-40,500)
#ax.w_zaxis.set_major_locator(LinearLocator(10))
ax.w_zaxis.set_major_formatter(FormatStrFormatter('%.0f'))
fig.colorbar(surf, shrink=0.5, aspect=5)
print '----Complete----'
print 'Saving the finished graph to disk'
plt.savefig('Shekel Foxhole')
print '----Complete----'
