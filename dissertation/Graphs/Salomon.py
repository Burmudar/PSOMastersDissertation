from mpl_toolkits.mplot3d import Axes3D
from matplotlib import cm
from matplotlib.ticker import LinearLocator, FixedLocator, FormatStrFormatter
import matplotlib
matplotlib.use('PDF')
import matplotlib.pyplot as plt
import numpy as np

AMOUNT_OF_POINTS = 350

def Salomon(x,y):
	SalomonSum1 = 0
	SalomonSum1 += x ** 2
	SalomonSum1 += y ** 2
	SalomonSum1 = -np.cos(2*np.pi*np.sqrt(SalomonSum1))
	SalomonSum2 = 0
	SalomonSum2 += (x ** 2)# + 1
	SalomonSum2 += (y ** 2)# + 1
	SalomonSum2 = 0.1 * np.sqrt(SalomonSum2)+1
	return SalomonSum1 + SalomonSum2


fig = plt.figure()
ax = Axes3D(fig)
X = np.linspace(-5, 5, AMOUNT_OF_POINTS)
Y = np.linspace(-5, 5, AMOUNT_OF_POINTS)
X, Y = np.meshgrid(X, Y)

print 'Initializing Function'
zValues = []
for i in range(AMOUNT_OF_POINTS):
	val = Salomon(X[i],Y[i])
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
ax.w_zaxis.set_major_formatter(FormatStrFormatter('%.0f'))
fig.colorbar(surf, shrink=0.5, aspect=5)
print '----Complete----'
print 'Saving the finished graph to disk'
plt.savefig('SalomonTest')
print '----Complete----'
