# -*- coding:utf-8 -*-

import pymysql
import os
import codecs
import math
import ReadConfigFile
import WeiboUser

MysqlDBinfo = ReadConfigFile.GetMysqlDBinfo()
print MysqlDBinfo

def UpdateUser_PCity(userid):
    # 一个特别的案例：1628855320
    # 错误的例子：2955138727
    # 错误的例子：3024194905
    # 错误的例子：1944038004
    # 又一个特殊的例子：1886756671
    testUserID = userid
    GetUserWb_Conn = pymysql.connect(host=MysqlDBinfo["db_host"], port=int(MysqlDBinfo["db_port"]),  user=MysqlDBinfo["db_user"],
                                   passwd=MysqlDBinfo["db_passwd"], db=MysqlDBinfo["db_database"],charset='utf8')
    GetUserWb_Cursor = GetUserWb_Conn.cursor(cursor=pymysql.cursors.DictCursor)
    recordsCount = GetUserWb_Cursor.execute(r"SELECT cityname , DATE_FORMAT(time,'%%Y%%m') as months "
                                            r"FROM  travel_poi_users_weibodata_suzhou WHERE  userid = %s ;",(testUserID))
    CityTime = []
    testUser = WeiboUser.wbUser(testUserID)
    #获取用户所有微博的发布时间与发布城市
    for record in range(recordsCount):
        recordLine = GetUserWb_Cursor.fetchone()
        CityTime.append((recordLine["cityname"],recordLine["months"]))
    testUser.All_CityTime = CityTime
    City_TimeDistribute = {}
    #将数据按发布城市进行划分
    for (city,months) in CityTime:
        if  city not in City_TimeDistribute.keys():
            City_TimeDistribute[city] = []
        City_TimeDistribute[city].append(months)
    testUser.TimeDistributeByCity = City_TimeDistribute
    #计算信息熵
    for city in City_TimeDistribute.keys():
        cityEnt = testUser.calc_entropy(City_TimeDistribute[city])
        testUser.WeiboEntropyInCity[city] = cityEnt
    '''
    #对信息熵进行排序
    print "信息熵："
    for sortedItem in sorted(testUser.WeiboEntropyInCity.items(),key=lambda item: -item[1]):
        print "\tCity:%s\t\tEntropy:%.5f" % (sortedItem[0],sortedItem[1])
    #进一步进行分析
    '''
    GetUserWb_Cursor.execute("update suzhou.all_user_info set city_byEntropy = %s where userid = %s",
                             (sorted(testUser.WeiboEntropyInCity.items(),key=lambda item: -item[1]).__getitem__(0)[0],testUserID))
    #计算伪信息熵 ，对每个城市的熵分别乘以各自的发布数目
    for city in City_TimeDistribute.keys():
        cityEnt = testUser.calc_PseudoEntropy(City_TimeDistribute[city])
        testUser.WeiboPseudoEntropyInCity[city] = cityEnt
    GetUserWb_Cursor.execute("update suzhou.all_user_info set city_byPseudoEntropy = %s where userid = %s",
                             (sorted(testUser.WeiboPseudoEntropyInCity.items(),key=lambda item: -item[1]).__getitem__(0)[0],testUserID))
    '''
    #对伪信息熵进行排序
    print "Pseudo信息熵："
    for sortedItem in sorted(testUser.WeiboPseudoEntropyInCity.items(),key=lambda item: -item[1]):
        print "\tCity:%s\t\tEntropy:%.5f" % (sortedItem[0],sortedItem[1])
    '''
    GetUserWb_Cursor.close()
    GetUserWb_Conn.close()

def GetAllUser():
    GetAllUser_Conn = pymysql.connect(host=MysqlDBinfo["db_host"], port=int(MysqlDBinfo["db_port"]),
                                     user=MysqlDBinfo["db_user"],
                                     passwd=MysqlDBinfo["db_passwd"], db=MysqlDBinfo["db_database"], charset='utf8')
    GetAllUser_Cursor = GetAllUser_Conn.cursor(cursor=pymysql.cursors.DictCursor)
    recordsCount = GetAllUser_Cursor.execute(r"SELECT userid FROM  suzhou.travel_users_prcity ;")
    records = GetAllUser_Cursor.fetchall()
    for i in range(recordsCount):
        UpdateUser_PCity(records[i]["userid"])
        print "%d \t %.3f%%" % (i, float(i*100)/recordsCount)
    GetAllUser_Cursor.close()
    GetAllUser_Conn.close()
    print "All Done!"

GetAllUser()