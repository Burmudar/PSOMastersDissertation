from mpl_toolkits.mplot3d import Axes3D
from matplotlib import cm
from matplotlib.ticker import LinearLocator, FixedLocator, FormatStrFormatter
import matplotlib
matplotlib.use('PDF')
import matplotlib.pyplot as plt
import numpy as np

AMOUNT_OF_POINTS = 150
#http://www.geatbx.com/docu/fcnindex-01.html
def Ackley(x,y):
	a = 20
	b = 0.2
	c = 2* np.pi
	ndiv = 1.0 / 2.0
	AckleySum = -a * np.exp(-b * np.sqrt(ndiv*(x** 2 + y ** 2)))

	AckleySum -= np.exp(ndiv * (np.cos(c*x) + np.cos(c*y))) + a + np.exp(1)
	return AckleySum


fig = plt.figure()
ax = Axes3D(fig)
X = np.linspace(-32.768, 32.768, AMOUNT_OF_POINTS)
Y = np.linspace(-32.768, 32.768, AMOUNT_OF_POINTS)
X, Y = np.meshgrid(X, Y)

print 'Initializing Function'
zValues = []
for i in range(AMOUNT_OF_POINTS):
	val = Ackley(X[i],Y[i])
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
plt.savefig('Ackley')
print '----Complete----'
