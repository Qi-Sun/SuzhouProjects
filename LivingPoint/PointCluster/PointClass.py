# -*- coding:utf-8 -*-
import os
import math

class wbPoint:
    EarthRadius = 6371004

    def __init__(self,lat = 0,lng = 0):
        """

        :rtype: object
        """
        self.latitude = lat
        self.longitude =lng

    def GetDistance(self, otherPoint):
        arc = math.sin(self.latitude * math.pi / 180) * math.sin(otherPoint.latitude * math.pi / 180) +\
              math.cos(self.latitude * math.pi / 180) * math.cos(otherPoint.latitude * math.pi / 180) * \
              math.cos((self.longitude - otherPoint.longitude) * math.pi / 180)
        return self.EarthRadius * math.acos(arc) * math.pi / 180

if __name__ == "__main__":
    print math.sin(math.pi/2)
