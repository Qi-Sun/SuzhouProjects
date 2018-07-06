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
import types

G = nx.Graph()
#复杂网络文件路径
NetworkName = "FarUser_Code_network"
codefilename = 'Data/ScenicAndCode.txt'
resultOutputPath ="NodeEvaluation/%s"%NetworkName
if not os.path.exists(resultOutputPath):
    os.makedirs(resultOutputPath)
codefile = codecs.open(codefilename,encoding='utf-8')
#景点和编码对应关系
CodeSenic = dict()
for i,line in enumerate(codefile.readlines()):
    scenic = line.strip('\r\n')
    CodeSenic[i] = scenic

ScenicLabel = dict()
#景区网络
ScenicMatrix = np.zeros((109,109))

#读入景区网络
fileName = 'Data/%s.txt' % NetworkName
fileobj = codecs.open(filename=fileName,encoding='utf-8')
for line in fileobj.readlines():
    lineinfo = line.strip('\r\n').split(' ')
    ScenicLabel[(int)(lineinfo[0])] = CodeSenic[(int)(lineinfo[0])]
    G.add_weighted_edges_from([((int)(lineinfo[0]),(int)(lineinfo[1]),(float)(lineinfo[2]))])
    ScenicMatrix[(int)(lineinfo[0])][(int)(lineinfo[1])]= (float)(lineinfo[2])
    ScenicMatrix[(int)(lineinfo[1])][(int)(lineinfo[0])]= (float)(lineinfo[2])

#储存景点相关评价指标
ScenicNodeMessage= np.zeros((115,9))
#获取网络结点
NodeList = []
NetworkNodes = nx.nodes(G)
for node in NetworkNodes:
    NodeList.append(node)
# - 1: 景区名称s
# 0: 景区编号
for i in range (115):
    ScenicNodeMessage[i,0] = (int)(i)
# 1 ：度中心度
gDegrees = nx.degree(G)
print type(gDegrees)
gDegreesList = []
for i in range(len(gDegrees)):
    ScenicNodeMessage[NodeList[i],1] = gDegrees[NodeList[i]]
#2: 中介中心度
gBetweenDegrees = nx.betweenness_centrality(G)
for i in range(len(gBetweenDegrees)):
    ScenicNodeMessage[NodeList[i],2] = gBetweenDegrees[NodeList[i]]
# 3:亲近中心度
gClosedDegrees = nx.closeness_centrality(G)
for i in range(len(gClosedDegrees)):
    ScenicNodeMessage[NodeList[i],3] = gClosedDegrees[NodeList[i]]

#讲结果写入文件
resultFile = codecs.open("%s/%s%s.txt"% (resultOutputPath,"Summary","_Info"),encoding="utf-8" ,mode="w+")
resultFile.write("%s\t%s\t%s\t%s\t%s\t%s\t%s\t%s\t%s\t%s\t\r\n"% ("ScenicName","Code","Degrees","BetweenDegrees","ClosedDegrees","Res5","Res6","Res7","Res8","Res9"))
for i in range(115):
    resultFile.write("%s\t" % CodeSenic[i])
    for j in range (9):
        resultFile.write("%s\t"%ScenicNodeMessage[i,j])
    resultFile.write("\r\n")
resultFile.close()


