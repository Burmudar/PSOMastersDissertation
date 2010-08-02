from mpl_toolkits.mplot3d import Axes3D
from matplotlib import cm
from matplotlib.ticker import LinearLocator, FixedLocator, FormatStrFormatter
import matplotlib
matplotlib.use('PDF')
import matplotlib.pyplot as plt
import numpy as np

AMOUNT_OF_POINTS = 150
#http://www.geatbx.com/docu/fcnindex-01.html
def Camel(x,y):
	CamelSum = (4 - 2.1 * (x ** 2) + (x ** 4.0)/3.0) * x ** 2 + (x * y) + (-4 + 4 * y **2)* y ** 2
	return CamelSum


fig = plt.figure()
ax = Axes3D(fig)
X = np.linspace(-3, 3, AMOUNT_OF_POINTS)
Y = np.linspace(-2, 2, AMOUNT_OF_POINTS)
X, Y = np.meshgrid(X, Y)

print 'Initializing Function'
zValues = []
for i in range(AMOUNT_OF_POINTS):
	val = Camel(X[i],Y[i])
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
plt.savefig('Camel')
print '----Complete----'
