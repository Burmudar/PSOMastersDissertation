from mpl_toolkits.mplot3d import Axes3D
from matplotlib import cm
from matplotlib.ticker import LinearLocator, FixedLocator, FormatStrFormatter
import matplotlib
matplotlib.use('PDF')
import matplotlib.pyplot as plt
import numpy as np

AMOUNT_OF_POINTS = 150
def Michalewicz(x,y):
	m = 20
	sumAnswx = np.sin(x) * (np.sin((1 - x ** 2) / np.pi))**(2 * m)
	sumAnswy = np.sin(y) * (np.sin((1 - y ** 2) / np.pi))**(2 * m)
	return -1 * (sumAnswx + sumAnswy) 


fig = plt.figure()
ax = Axes3D(fig)
X = np.linspace(0, np.pi, AMOUNT_OF_POINTS)
Y = np.linspace(0, np.pi, AMOUNT_OF_POINTS)
X, Y = np.meshgrid(X, Y)

print 'Initializing Function'
zValues = []
for i in range(AMOUNT_OF_POINTS):
	val = Michalewicz(X[i],Y[i])
	zValues.append(val)
print '----Complete----'
print 'Create Numpy array from Function results'
Z = np.array(zValues)
print '----Complete----'
print 'Plotting function'
surf = ax.plot_surface(X, Y, Z, rstride=1, cstride=1, cmap=cm.jet,
        linewidth=0, antialiased=False)
print '----Complete----'
print 'Setting various graph properties'

#ax.w_zaxis.set_major_locator(LinearLocator(10))
ax.w_zaxis.set_major_formatter(FormatStrFormatter('%.01f'))
fig.colorbar(surf, shrink=0.5, aspect=5)
print '----Complete----'
print 'Saving the finished graph to disk'
plt.savefig('Michalewicz')
print '----Complete----'
