from mpl_toolkits.mplot3d import Axes3D
from matplotlib import cm
from matplotlib.ticker import LinearLocator, FixedLocator, FormatStrFormatter
import matplotlib
matplotlib.use('PDF')
import matplotlib.pyplot as plt
import numpy as np

AMOUNT_OF_POINTS = 150
SCHWEFEL_CONSTANT = 418.9829 * 3
def Schwefel(x,y):
	schwefelSum = 0
	schwefelSum += (-1 * x)*np.sin(np.sqrt(abs(x)))
	schwefelSum += (-1 * y)*np.sin(np.sqrt(abs(y)))
	return SCHWEFEL_CONSTANT + schwefelSum


fig = plt.figure()
ax = Axes3D(fig)
X = np.linspace(-500, 500, AMOUNT_OF_POINTS)
Y = np.linspace(-500, 500, AMOUNT_OF_POINTS)
X, Y = np.meshgrid(X, Y)

print 'Initializing Function'
zValues = []
for i in range(AMOUNT_OF_POINTS):
	val = Schwefel(X[i],Y[i])
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
plt.savefig('Schwefel')
print '----Complete----'
