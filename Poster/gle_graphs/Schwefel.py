import math

if __name__ == "__main__":
	f = open("Schwefel.data",'w')
	for x in range(-400,400):
		for y in range(-400,400):
			z = -x*math.sin(math.sqrt(math.fabs(x)) - math.sin(math.sqrt(math.fabs(y))))
			f.write(str(x) + ' ' + str(y) + ' ' + str(z)+'\n')
	f.close()
