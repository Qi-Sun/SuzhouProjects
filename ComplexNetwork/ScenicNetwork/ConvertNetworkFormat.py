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

def EdgeListToMatrix(edgeFile,matrixFile,NodeCount):
    edgeFileObject = codecs.open(edgeFile,encoding= "utf-8")
    Network = np.zeros((NodeCount, NodeCount))
    for line in edgeFileObject.readlines():
        lineinfo = line.strip('\n').split(' ')
        Network[(int)(lineinfo[0])][(int)(lineinfo[1])] = (float)(lineinfo[2])
        Network[(int)(lineinfo[1])][(int)(lineinfo[0])] = (float)(lineinfo[2])
    edgeFileObject.close()
    networkfile = codecs.open(matrixFile, encoding='utf-8', mode='w')
    for i in range(NodeCount):
        networkfile.write(u',%d' % i)
    networkfile.write('\r\n')
    for j in range(NodeCount):
        networkfile.write(str(j))
        for k in range(NodeCount):
            if j == k:
                networkfile.write(u',%s' % (0))
            else:
                networkfile.write(u',%s' % (Network[j][k]))
        networkfile.write('\r\n')
    networkfile.close()
    return

def MatrixToEdgeList(matrixFile,edgeFile,NodeCount):
    Network = np.zeros((NodeCount, NodeCount))
    networkfile = codecs.open(matrixFile, encoding='utf-8')
    headline = networkfile.readline()
    for line in networkfile.readlines():
        weightData = line.strip("\r\n").split(',')
        for i in range(NodeCount):
            Network[(int)(weightData[0])][i+1] = (float)(weightData[i+1])
    networkfile.close()
    edgeFileObject = codecs.open(edgeFile, encoding="utf-8", mode='w')
    for i in range(NodeCount):
        for j in range(NodeCount):
            if i == j or Network[i][j] == 0:
                continue
            else:
                edgeFileObject.writelines(u'%s %s %s\r\n' % (i, j, Network[i][j]))
    edgeFileObject.close()
    return

if __name__ == "__main__":
    EdgeListToMatrix("Data/FarUser_Code_POInetwork_Closedto.txt","Data/FarUser_Code_POInetwork_Closedto_Matrix_nodiag.txt",115)