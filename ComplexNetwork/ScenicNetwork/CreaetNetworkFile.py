#-*- coding:utf-8 -*-
import pymysql
import numpy as np
import codecs
import socket


socket.getaddrinfo('222.29.117.240',6667)
filename = 'Data/SzUser_Code_network_Closedto.txt'
codefilename = 'Data/ScenicAndCode.txt'
codefile = codecs.open(codefilename,encoding='utf-8')
CodeSenic = dict()
scenicCode = dict()
for i,line in enumerate(codefile.readlines()):
    scenic = line.strip('\r\n')
    scenicCode[scenic] = i
    CodeSenic[i] = scenic

scenicNetwork = np.zeros((110,110))

# 打开数据库连接
db = pymysql.connect(host="222.29.117.240",port =6667,user ="root",passwd ='zhangruijie',db ="suzhou",charset = 'utf8')
# 使用 cursor() 方法创建一个游标对象 cursor
tb_cursor = db.cursor(cursor=pymysql.cursors.DictCursor)
# 使用 execute()  方法执行 SQL 查询
recordCount = tb_cursor.execute("SELECT userid,ms_action_id_l0n3 FROM suzhou.suzhou_ms_action_l0n3 where userid in (SELECT userid FROM suzhou.user_group_sz);")

actionConn = pymysql.connect(host="222.29.117.240",port =6667,user ="root",passwd ='zhangruijie',db ="suzhou",charset = 'utf8')
ac_cursor = actionConn.cursor(cursor=pymysql.cursors.DictCursor)


# 使用 fetchone() 方法获取单条数据.
for i in range(recordCount):
    data = tb_cursor.fetchone()
    userid = data["userid"]
    actionid = data["ms_action_id_l0n3"]
    wbCount = ac_cursor.execute("SELECT Place_1110_r200_k20 FROM suzhou.suzhou_weibos_sq where userid = %s  and action_id = %s and Value_1110_r200_k20 >= 1;"  % (userid,actionid))
    placeList = []
    for j in range (wbCount):
        wb = ac_cursor.fetchone()
        if  wb["Place_1110_r200_k20"] is not None:
            placeList.append(scenicCode[wb["Place_1110_r200_k20"]])
    placeCount = len(placeList)

    for k in range(placeCount -1 ):
        scenicNetwork[placeList[k]][placeList[k+1]] += 1
        scenicNetwork[placeList[k+1]][placeList[k]] += 1
    '''
    for k in range(placeCount):
        for l in range(k,placeCount):
            scenicNetwork[placeList[k]][placeList[l]] += 1
            scenicNetwork[placeList[l]][placeList[k]] += 1
    '''

    print i , i *100.0 / recordCount

networkfile = codecs.open(filename,encoding='utf-8',mode='w')

for i in range(108):
    for j in range(108):
        if i == j or scenicNetwork[i][j] == 0:
            continue
        else:
            #networkfile.writelines( u'%s %s %s\r\n' % (CodeSenic[i],CodeSenic[j],scenicNetwork[i][j]))
            networkfile.writelines( u'%s %s %s\r\n' % (i,j,scenicNetwork[i][j]))
networkfile.close()
'''
for i in range(108):
    networkfile.write(u',%d' % i)
networkfile.write('\r\n')
for j in range(108):
    networkfile.write(str(j))
    for k in range(108):
        if j == k :
            networkfile.write(u',%s' % (0))
        else:
            networkfile.write(u',%s' % (scenicNetwork[j][k]))
    networkfile.write('\r\n')
networkfile.close()
'''
actionConn.close()
# 关闭数据库连接
db.close()