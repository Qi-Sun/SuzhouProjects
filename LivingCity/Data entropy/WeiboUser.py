# -*- coding:utf-8 -*-
import math
import numpy as np

class wbUser:
    def __init__(self,uid,created_at = None):
        self.userid = uid
        self.created_at = created_at
        self.CityList = []
        self.WeiboCount = 0
        self.TimeDistributeByCity = {}
        self.All_CityTime = []
        self.WeiboCountByMonthInCity = {}
        self.WeiboEntropyInCity = {}
        self.WeiboPseudoEntropyInCity = {}
        self.WeiboGiniInCity = {}

    def calc_entropy(self,OneCity_TimeSequence):
        timeList = set([OneCity_TimeSequence[i] for i in range(len(OneCity_TimeSequence))])
        ent = 0.0
        for OneTime in timeList:
            p = float(sum(map(lambda time: 1 if time == OneTime else 0,OneCity_TimeSequence)))/ len(OneCity_TimeSequence)
            logp = math.log10(p)
            ent -= p * logp
        return ent

    def calc_PseudoEntropy(self,OneCity_TimeSequence):
        timeList = set([OneCity_TimeSequence[i] for i in range(len(OneCity_TimeSequence))])
        ent = 0.0
        for OneTime in timeList:
            p = float(sum(map(lambda time: 1 if time == OneTime else 0,OneCity_TimeSequence)))/ len(OneCity_TimeSequence)
            logp = math.log10(p)
            ent -= p * logp
        return ent * len(OneCity_TimeSequence)
