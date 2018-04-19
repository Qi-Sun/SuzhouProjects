# -*- coding:utf-8 -*-
import PointClass

class UserWeiboInCity:
    def __init__(self,userid):
        self.UserId =userid
        self.RawWeiboList = []
        self.TimeFilterWeiboList = []
        self.LivingPoint = PointClass.wbPoint()

        self.RawWeiboGroup = {}
        self.TimeFilterWeiboGroup = {}
