#-*- coding:utf-8 -*-
import igraph
from PIL import Image
import networkx as nx
import pymysql

G = nx.Graph()
G.add_node('a')
G.add_node('b')
G.add_edge('b','c')
G.add_edge('a','c',2)
G.add_edge('c','b',3)

print G.nodes()
print G.edges()

path=nx.all_pairs_shortest_path(G)
print path
print G.degree()
print nx.degree_histogram(G)

import matplotlib.pyplot as plt
# 导入科学绘图的matplotlib包
degree =  nx.degree_histogram(G)          #返回图中所有节点的度分布序列
x = range(len(degree))                             #生成x轴序列，从1到最大度
y = [z / float(sum(degree)) for z in degree]
#将频次转换为频率，这用到Python的一个小技巧：列表内涵，Python的确很方便：）
plt.loglog(x,y,color="blue",linewidth=2)           #在双对数坐标轴上绘制度分布曲线
plt.show()                                                          #显示图表
