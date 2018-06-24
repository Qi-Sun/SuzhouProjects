#-*- coding:utf-8 -*-
import pymysql
import datetime
import time
# 打开数据库连接
db = pymysql.connect(host="127.0.0.1",port =3306,user ="root",passwd ='19950310',db ="suzhou",charset = 'utf8')
# 使用 cursor() 方法创建一个游标对象 cursor
db_cursor = db.cursor(cursor=pymysql.cursors.DictCursor)

recordCount = db_cursor.execute("SELECT uid,id,date_format(time ,'%Y-%m-%d %H:%i:%S') as timestr FROM suzhou.travel_poi_checkin_weibos_suzhou "
                                "where uid not in (SELECT userid FROM suzhou.user_group_sz) order BY uid, time;")

writedb =pymysql.connect(host="127.0.0.1",port =3306,user ="root",passwd ='19950310',db ="suzhou",charset = 'utf8')
writedb_cursor =writedb.cursor(cursor=pymysql.cursors.DictCursor)

firstLine = db_cursor.fetchone()
LastUserid = firstLine["uid"]
LastWeiboId = firstLine["id"]
LastTime =  datetime.datetime.strptime(firstLine["timestr"],'%Y-%m-%d %H:%M:%S')
LastActionId = 0
writedb_cursor.execute(
            "Update suzhou.travel_poi_checkin_weibos_suzhou set ms_actionid_l0n3 = %s where id = %s;",
            (LastActionId, LastWeiboId))

for i in range(1,recordCount):
    CurLine = db_cursor.fetchone()
    CurUserid = CurLine["uid"]
    CurWeiboId = CurLine["id"]
    CurTime =datetime.datetime.strptime(CurLine["timestr"],'%Y-%m-%d %H:%M:%S')
    if  CurUserid != LastUserid:
        LastActionId = 0
        LastUserid = CurUserid
        LastTime = CurTime
    else:
        if  (CurTime -LastTime).days <=3:
            LastActionId += 0
            LastTime = CurTime
        else:
            LastActionId += 1
            LastTime = CurTime
    writedb_cursor.execute(
            "Update suzhou.travel_poi_checkin_weibos_suzhou set ms_actionid_l0n3 = %s where id = %s;",
            (LastActionId, CurWeiboId))
    print i,(i*100.0/recordCount),'%'