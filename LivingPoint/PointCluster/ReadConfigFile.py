# -*- coding:utf-8 -*-

import ConfigParser
import os

def GetMysqlDBinfo():
    mysqldbinfo ={}
    os.chdir(os.path.split(os.path.realpath(__file__))[0])
    cf = ConfigParser.ConfigParser()
    cf.read("mysqldb.conf")
    sects = cf.sections()
    if 'db' in sects:
        opts = cf.options('db')
        for option  in opts:
            mysqldbinfo[option] = cf.get("db", option)
    return mysqldbinfo

if __name__ == "__main__":
    print __file__
    print os.path.split(os.path.realpath(__file__))[0]
    print GetMysqlDBinfo()