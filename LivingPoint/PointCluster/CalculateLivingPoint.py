# -*- coding:utf-8 -*-
import pymysql
import os
import sys
import ReadConfigFile
import UserWeiboInCity
import datetime
import PointClass

MysqlDBinfo = ReadConfigFile.GetMysqlDBinfo()

def CalculateOneUser(userid,radius):
    CurUser = UserWeiboInCity.UserWeiboInCity(userid)
    GetWbInSuzhou_conn = pymysql.connect(host=MysqlDBinfo["db_host"], port=int(MysqlDBinfo["db_port"]),  user=MysqlDBinfo["db_user"],
                                   passwd=MysqlDBinfo["db_passwd"], db=MysqlDBinfo["db_database"],charset='utf8')
    GetWbInSuzhou_cursor = GetWbInSuzhou_conn.cursor(cursor=pymysql.cursors.DictCursor)
    recordCount =  GetWbInSuzhou_cursor.execute("select gps_latitude,gps_longitude,time from suzhou.suzhou_weibos_sq where userid = %s ;",
                                 (userid,))
    records = GetWbInSuzhou_cursor.fetchall()
    for i in range (recordCount):
        if float(records[i]["gps_latitude"]) < 0.1 or float(records[i]["gps_longitude"]) < 0.1 :
            continue
        else:
            CurUser.RawWeiboList.append((PointClass.wbPoint(float(records[i]["gps_latitude"]),float(records[i]["gps_longitude"])),records[i]["time"]))
            #datetime.datetime.strptime(records[i]["time"], '%Y-%m-%d %H:%M:%S')
    for weiboInfo in CurUser.RawWeiboList:
        if weiboInfo[1].hour <= 8  or  weiboInfo[1].hour > 20 :
            CurUser.TimeFilterWeiboList.append(weiboInfo)

    #统计原始数据中每个点的次数
    '''
    rawWeiboCount = len(CurUser.RawWeiboList)
    for i in range(rawWeiboCount):
        alreadyInList = False
        for point in CurUser.RawWeiboGroup.keys():
            if point == CurUser.RawWeiboList[i][0]:
                alreadyInList = True
                CurUser.RawWeiboGroup[point] += 1
        if alreadyInList is False :
            CurUser.RawWeiboGroup[CurUser.RawWeiboList[i][0]] = 1
    '''
    rawWeiboCount = len(CurUser.RawWeiboList)
    for i in range(rawWeiboCount):
        if CurUser.RawWeiboList[i][0] not in CurUser.RawWeiboGroup.keys():
            CurUser.RawWeiboGroup[CurUser.RawWeiboList[i][0]] = 0
        CurUser.RawWeiboGroup[CurUser.RawWeiboList[i][0]] += 1
    # 统计选取时间段内数据中每个点的次数
    '''
    rawWeiboCount = len(CurUser.TimeFilterWeiboList)
    for i in range(rawWeiboCount):
        alreadyInList = False
        for point in CurUser.TimeFilterWeiboGroup.keys():
            if point == CurUser.TimeFilterWeiboList[i][0]:
                alreadyInList = True
                CurUser.TimeFilterWeiboGroup[point] += 1
        if alreadyInList is False:
            CurUser.TimeFilterWeiboGroup[CurUser.TimeFilterWeiboList[i][0]] = 1
    '''
    timeFilterWeiboCount = len(CurUser.TimeFilterWeiboList)
    for i in range(timeFilterWeiboCount):
        if CurUser.TimeFilterWeiboList[i][0] not in CurUser.TimeFilterWeiboGroup.keys():
            CurUser.TimeFilterWeiboGroup[CurUser.TimeFilterWeiboList[i][0]] = 0
        CurUser.TimeFilterWeiboGroup[CurUser.TimeFilterWeiboList[i][0]] += 1

    #计算各点密度值
    for point in CurUser.RawWeiboGroup.keys():
        for point2 in CurUser.RawWeiboGroup.keys():
            if point == point2:
                continue
            point.DensityValue += point.GetLocalDensity(point2,radius) * CurUser.RawWeiboGroup[point2]

    #检验
    print len(CurUser.RawWeiboGroup)
    print len(CurUser.TimeFilterWeiboGroup)
    for sortedItem in sorted(CurUser.RawWeiboGroup.keys(), key=lambda item: -item.DensityValue):
        print "\tPoint:%s\t\tValue:%.5f" % (sortedItem, sortedItem.DensityValue)
    # -- print sorted(CurUser.RawWeiboGroup.keys())

    ActivePoint = sorted(CurUser.RawWeiboGroup.keys(), key=lambda item: -item.DensityValue)[0]
    GetWbInSuzhou_cursor.execute("update suzhou.all_user_info set activepoint = Point(%f,%f) where userid = %s" % (float(ActivePoint.latitude),float(ActivePoint.longitude),userid))

    #计算筛选时间段内的各点密度值
    if len(CurUser.TimeFilterWeiboGroup.keys())  > 0:
        for point in CurUser.TimeFilterWeiboGroup.keys():
            for point2 in CurUser.TimeFilterWeiboGroup.keys():
                if point == point2:
                    continue
                point.DensityValue_tf += point.GetLocalDensity(point2, radius) * CurUser.RawWeiboGroup[point2]

    ActivePoint_tf = sorted(CurUser.TimeFilterWeiboGroup.keys(), key=lambda item: -item.DensityValue_tf)[0]
    GetWbInSuzhou_cursor.execute("update suzhou.all_user_info set activepoint_tf = Point(%f,%f) where userid = %s" % (float(ActivePoint_tf.latitude),float(ActivePoint_tf.longitude),userid))

    GetWbInSuzhou_cursor.close()
    GetWbInSuzhou_conn.close()


if __name__ == "__main__":
    #CalculateOneUser(37237,200)
    print datetime.datetime.now().strftime('%Y-%m-%d %H:%M:%s')
    GetAllUser_Conn = pymysql.connect(host=MysqlDBinfo["db_host"], port=int(MysqlDBinfo["db_port"]),
                                      user=MysqlDBinfo["db_user"],
                                      passwd=MysqlDBinfo["db_passwd"], db=MysqlDBinfo["db_database"], charset='utf8')
    GetAllUser_Cursor = GetAllUser_Conn.cursor(cursor=pymysql.cursors.DictCursor)
    recordsCount = GetAllUser_Cursor.execute(r"SELECT userid FROM  suzhou.travel_users_prcity ;")
    records = GetAllUser_Cursor.fetchall()
    for i in range(recordsCount):
        CalculateOneUser(records[i]["userid"],200)
        print "%d \t %.3f%%" % (i, float(i * 100) / recordsCount)
    GetAllUser_Cursor.close()
    GetAllUser_Conn.close()
    print "All Done!"
    print datetime.datetime.now().strftime('%Y-%m-%d %H:%M:%s')