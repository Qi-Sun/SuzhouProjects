# -*- coding:utf-8 -*-
import pymysql
import os
import sys
import ReadConfigFile
import UserWeiboInCity
import datetime
import PointClass

MysqlDBinfo = ReadConfigFile.GetMysqlDBinfo()

def CalculateOneUser(userid):
    CurUser = UserWeiboInCity.UserWeiboInCity(userid)
    GetWbInSuzhou_conn = pymysql.connect(host=MysqlDBinfo["db_host"], port=int(MysqlDBinfo["db_port"]),  user=MysqlDBinfo["db_user"],
                                   passwd=MysqlDBinfo["db_passwd"], db=MysqlDBinfo["db_database"],charset='utf8')
    GetWbInSuzhou_cursor = GetWbInSuzhou_conn.cursor(cursor=pymysql.cursors.DictCursor)
    recordCount =  GetWbInSuzhou_cursor.execute("select gps_latitude,gps_longitude,time from suzhou.suzhou_weibo_sq where userid = %s ;",
                                 (userid,))
    records = GetWbInSuzhou_cursor.fetchall()
    for i in range (recordCount):
        CurUser.WeiboList.append((PointClass.wbPoint(float(records[i]["gps_latitude"]),float(records[i]["gps_longitude"])),
                                 datetime.datetime.strptime(records[i]["time"], '%Y-%m-%d %H:%M:%S')))
    #TODO