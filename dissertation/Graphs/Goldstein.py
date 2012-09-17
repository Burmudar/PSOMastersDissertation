from mpl_toolkits.mplot3d import Axes3D
from matplotlib import cm
from matplotlib.ticker import LinearLocator, FixedLocator, FormatStrFormatter
import matplotlib
matplotlib.use('PDF')
import matplotlib.pyplot as plt
import numpy as np

AMOUNT_OF_POINTS = 150
def Goldstein(x,y):
	answ = (1 + (( x + y + 1) ** 2) * (19 - 14 * x + ((3*x)**2) - 14 * y + 6 * x * y + ((3 * y) ** 2)))
	answ *= (30 + ((2 * x - 3 * y)**2) * (18 -32 * x +  (12 * x)**2 + 48 * y - 36 * x * y + (27 * y)**2))
	return answ  / 1000000


fig = plt.figure()
ax = Axes3D(fig)
X = np.linspace(-2, 2, AMOUNT_OF_POINTS)
Y = np.linspace(-2, 2, AMOUNT_OF_POINTS)
X, Y = np.meshgrid(X, Y)

print 'Initializing Function'
zValues = []
for i in range(AMOUNT_OF_POINTS):
	val = Goldstein(X[i],Y[i])
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
plt.savefig('Goldstein')
print '----Complete----'
