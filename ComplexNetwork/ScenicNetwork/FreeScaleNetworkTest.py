#-*- coding:utf-8 -*-
import networkx as nx
import matplotlib.pyplot as plt
from sklearn import linear_model
from scipy.stats import norm
from collections import Counter
import numpy as np

# BA scale-free degree network
# generalize BA network which has 20 nodes, m = 1
BA = nx.random_graphs.barabasi_albert_graph(2000, 1)
# spring layout
pos = nx.spring_layout(BA)
nx.draw(BA, pos, with_labels = False, node_size = 30)
plt.show()

gDegrees = nx.degree(BA)
gDegreesList = []
for gD in gDegrees:
    gDegreesList.append(gD[1])
gDegreeDict = Counter(gDegreesList)
X = []
Y = []
sum = sum(gDegreesList)
for gd in gDegreeDict.keys():
    X.append(gd)
    sum =gDegreeDict[gd]
    Y.append(sum)
plt.title("Raw Data")
plt.scatter(X, Y,  color='black')
plt.show()
X= np.log10(X)
Y = np.log10(Y)


def DataFitAndVisualization(X,Y):
    # 模型数据准备
    X_parameter=[]
    Y_parameter=[]
    for single_square_feet ,single_price_value in zip(X,Y):
       X_parameter.append([float(single_square_feet)])
       Y_parameter.append(float(single_price_value))

    # 模型拟合
    regr = linear_model.LinearRegression()
    regr.fit(X_parameter, Y_parameter)
    # 模型结果与得分
    print 'Coefficients: \n', regr.coef_
    print "Intercept:\n",regr.intercept_
    # The mean square error
    print "Residual sum of squares: %.8f" % np.mean((regr.predict(X_parameter) - Y_parameter) ** 2)  # 残差平方和

    # 可视化
    plt.title("Log Data")
    plt.scatter(X_parameter, Y_parameter,  color='black')
    plt.plot(X_parameter, regr.predict(X_parameter), color='blue',linewidth=3)
    # plt.xticks(())
    # plt.yticks(())
    plt.show()

DataFitAndVisualization(X,Y)