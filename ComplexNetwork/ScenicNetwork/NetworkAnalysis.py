#-*- coding:utf-8 -*-
import networkx as nx
from matplotlib.font_manager import FontProperties
import codecs
from sklearn import linear_model
from scipy.stats import norm
import numpy as np
from collections import Counter
import math
import os


font = FontProperties(fname=r"C:\\WINDOWS\\Fonts\\simsun.ttc", size=14)#C:\WINDOWS\Fonts
labels = dict((n, n) for n in range(5))
G = nx.Graph()
'''
SzUser_Code_network
FarUser_Code_network
'''
NetworkName = "SzUser_Code_POInetwork"
codefilename = 'Data/ScenicAndCode.txt'
codefile = codecs.open(codefilename,encoding='utf-8')
CodeSenic = dict()
for i,line in enumerate(codefile.readlines()):
    scenic = line.strip('\r\n')
    CodeSenic[i] = scenic

ScenicLabel = dict()

ScenicMatrix = np.zeros((109,109))
gEdgeWeightList =[]
fileName = 'Data/%s.txt' % NetworkName
fileobj = codecs.open(filename=fileName,encoding='utf-8')
for line in fileobj.readlines():
    lineinfo = line.strip('\r\n').split(' ')
    ScenicLabel[(int)(lineinfo[0])] = CodeSenic[(int)(lineinfo[0])]
    G.add_weighted_edges_from([((int)(lineinfo[0]),(int)(lineinfo[1]),(float)(lineinfo[2]))])
    ScenicMatrix[(int)(lineinfo[0])][(int)(lineinfo[1])]= (float)(lineinfo[2])
    ScenicMatrix[(int)(lineinfo[1])][(int)(lineinfo[0])]= (float)(lineinfo[2])
    gEdgeWeightList.append((float)(lineinfo[2]))


import matplotlib.pyplot as plt
""""
# 导入科学绘图的matplotlib包
degree =  nx.degree_histogram(G)          #返回图中所有节点的度分布序列
x = range(len(degree))                             #生成x轴序列，从1到最大度
y = [z / float(sum(degree)) for z in degree]
#将频次转换为频率，这用到Python的一个小技巧：列表内涵，Python的确很方便：）
plt.loglog(x,y,color="blue",linewidth=2)           #在双对数坐标轴上绘制度分布曲线
plt.show()
"""
"""
randomG=nx.random_graphs.barabasi_albert_graph(1000,3)#生成n=1000,m=3的无标度的图
print u"所有节点的度分布序列:",nx.degree_histogram(G)#返回图中所有节点的度分布序列（从1至最大度的出现频次）
degree=nx.degree_histogram(G)#返回图中所有节点的度分布序列
x=range(len(degree))#生成X轴序列，从1到最大度
y=[z/float(sum(degree))for z in degree]#将频次转化为频率，利用列表内涵
plt.loglog(x,y,color="blue",linewidth=2)#在双对坐标轴上绘制度分布曲线
plt.show()#显示图表
"""
print u"群聚系数",nx.average_clustering(G)
print u"直径",nx.diameter(G)
pos0 = nx.spring_layout(G)
pos1 = dict()
fig0 = plt.figure(16)
nx.draw(G,with_labels=True,pos=pos0,node_size= 200,labels=ScenicLabel,font_family ="sans-serif")
plt.savefig("tmp_network.png")           #输出方式1: 将图像存为一个png格式的图片文件
plt.close(16)
#plt.show()

def DataGenerate():
    X = np.arange(10, 1010, 10)  # 0-1，每隔着0.02一个数据  0处取对数,会时负无穷  生成100个数据点
    noise=norm.rvs(0, size=100, scale=0.2)  # 生成50个正态分布  scale=0.1控制噪声强度
    Y=[]
    for i in range(len(X)):
       Y.append(10.8*pow(X[i],-0.3)+noise[i])  # 得到Y=10.8*x^-0.3+noise

    # plot raw data
    Y=np.array(Y)
    plt.title("Raw data")
    plt.scatter(X, Y,  color='black')
    plt.show()

    X=np.log10(X)  # 对X，Y取双对数
    Y=np.log10(Y)
    return X,Y

def DataFitAndVisualization(X,Y,filename=None):
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
    fig = plt.figure(64)
    plt.title("Log Data")
    plt.scatter(X_parameter, Y_parameter,  color='black')
    plt.plot(X_parameter, regr.predict(X_parameter), color='blue',linewidth=3)
    # plt.xticks(())
    # plt.yticks(())
    if  filename != None:
        plt.savefig(filename)
    #plt.show()
    plt.close(64)
    return regr

#求解皮尔逊相关系数
def computeCorrelation(X, Y):
    xBar = np.mean(X)
    yBar = np.mean(Y)
    SSR = 0
    varX = 0
    varY = 0
    for i in range(0, len(X)):
        #对应分子部分
        diffXXBar = X[i] - xBar
        diffYYBar = Y[i] - yBar
        SSR +=(diffXXBar * diffYYBar)
        #对应分母求和部分
        varX += diffXXBar**2
        varY += diffYYBar**2
    SST = math.sqrt(varX * varY)
    return SSR/SST

'''
gDegrees = nx.degree(G)

#gDegreesList 各节点的度中心度 
gDegreesList = []
for gD in gDegrees:
    gDegreesList.append(gD[1])
gDegreeDict = Counter(gDegreesList)

X = []
Y = []
sum = len(gDegreesList)
for gd in gDegreeDict.keys():
    X.append(gd)
    Y.append(sum)
    sum -=gDegreeDict[gd]
plt.title("Raw gDegrees")
plt.scatter(X, Y, color='black')
plt.show()
X = np.log10(X)
Y = np.log10(Y)
DataFitAndVisualization(X,Y)
'''
'''
#gVertexIntensity 各节点的顶点强度 
gVertexIntensity = ScenicMatrix.sum(axis = 0)
gVertexIntensity = gVertexIntensity[gVertexIntensity>0]
gVertexIntensityDict = Counter(gVertexIntensity)
X = []
Y = []
for gV in gVertexIntensityDict.keys():
    X.append(gV)
    Y.append(gVertexIntensityDict[gV])
plt.title("Raw gVertexIntensity")
plt.scatter(X, Y, color='green')
plt.show()
X = np.log10(X)
Y = np.log10(Y)
DataFitAndVisualization(X,Y)


#gVertexPressure 各节点的顶点压力 
gVertexPressure = gVertexIntensity/gDegreesList
gVertexPressureDict = Counter(gVertexPressure)
X = []
Y = []
for gV in gVertexPressureDict.keys():
    X.append(gV)
    Y.append(gVertexPressureDict[gV])
plt.title("Raw gVertexPressure")
plt.scatter(X, Y, color='blue')
plt.show()
X = np.log10(X)
Y = np.log10(Y)
DataFitAndVisualization(X,Y)

#gEdgeWeightList 各边的权重 
gEdgeWeightDict =  Counter(gEdgeWeightList)
X = []
Y = []
for gE in gEdgeWeightDict.keys():
    X.append(gE)
    Y.append(gEdgeWeightDict[gE])
plt.title("Raw gEdgeWeight")
plt.scatter(X, Y, color='red')
plt.show()
X = np.log10(X)
Y = np.log10(Y)
DataFitAndVisualization(X,Y)
'''
#gClusterCoefficient 各顶点的聚集系数
#gClusterCoefficient = list(map(lambda x: (x[0],x[1],nx.clustering(G,x[0])),[y for y in gDegrees]))
'''
X1,Y2=DataGenerate()
DataFitAndVisualization(X1,Y2)
print "Sample",computeCorrelation(X1,Y2)
print "sz network",computeCorrelation(X,Y)
'''

def GeneralDataFitVisulization(dataList,dataFileName,dataName):
    gDataDict = Counter(dataList)
    gDataItems = gDataDict.items()
    X = []
    Y = []
    sum = len(dataList)
    gDataItemsSorted = sorted(gDataItems,key=lambda x:x[0])
    for gD in gDataItemsSorted:
        X.append(gD[0])
        Y.append(sum)
        sum -= gD[1]
    fig2 = plt.figure(63)
    plt.title("Raw %s" % dataFileName)
    plt.scatter(X, Y, color='black')
    plt.savefig("PlotPng/%s/%s%s.png"% (dataFileName,dataName,"_RawData"))
    #plt.show()
    plt.close(63)
    X = np.log10(X)
    Y = np.log10(Y)
    regr = DataFitAndVisualization(X, Y,"PlotPng/%s/%s%s.png"% (dataFileName,dataName,"_Fit"))
    fitInfoFile = codecs.open("PlotPng/%s/%s%s.txt"% (dataFileName,dataName,"_FitInfo"),encoding="utf-8" ,mode="w+")
    fitInfoFile.write('Coefficients:%.8f\r\n'%regr.coef_)
    fitInfoFile.write( "Intercept:%.8f\r\n"% regr.intercept_)
    #fitInfoFile.write("Residual sum of squares: %.8f" % np.mean((regr.predict(X) - Y) ** 2)  )
    fitInfoFile.write("Correlation: %.8f" % computeCorrelation(X,Y)  )
    fitInfoFile.close()
    return

#gDegreesList 各节点的度中心度
gDegrees = nx.degree(G)
gDegreesList = []
for gD in gDegrees:
    gDegreesList.append(gD[1])
#gVertexIntensity 各节点的顶点强度
gVertexIntensity = ScenicMatrix.sum(axis = 0)
gVertexIntensity = gVertexIntensity[gVertexIntensity>0]
#gVertexPressure 各节点的顶点压力
gVertexPressure = gVertexIntensity/gDegreesList
#gEdgeWeightList 各边的权重
#gClusterCoefficient 各顶点的聚集系数
gClusterCoefficient = list(map(lambda x: (x[0],x[1],nx.clustering(G,x[0])),[y for y in gDegrees]))
gClusterCoefficient2 = list(map(lambda x: nx.clustering(G,x[0]),[y for y in gDegrees]))
if not os.path.exists("PlotPng/%s" % NetworkName):
    os.makedirs("PlotPng/%s" % NetworkName)
GeneralDataFitVisulization(gDegreesList,NetworkName,"Degree")
GeneralDataFitVisulization(gVertexIntensity,NetworkName,"VertexIntensity")
GeneralDataFitVisulization(gVertexPressure,NetworkName,"VertexPressure")
GeneralDataFitVisulization(gEdgeWeightList,NetworkName,"EdgeWeight")
#GeneralDataFitVisulization(gClusterCoefficient2,"FarUser_Code_network","ClusterCoefficient")
