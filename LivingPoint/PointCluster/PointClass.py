# -*- coding:utf-8 -*-
import os
import math

class wbPoint:
    EarthRadius = 6371004

    def __init__(self,lat = 0,lng = 0):
        """

        :rtype: object
        """
        self.latitude = float(lat)
        self.longitude = float(lng)

        self.DensityValue = 0
        self.DensityValue_tf = 0

    def GetDistance(self, otherPoint):
        arc = math.sin(self.latitude * math.pi / 180) * math.sin(otherPoint.latitude * math.pi / 180) +\
              math.cos(self.latitude * math.pi / 180) * math.cos(otherPoint.latitude * math.pi / 180) * \
              math.cos((self.longitude - otherPoint.longitude) * math.pi / 180)
        return self.EarthRadius * math.acos(arc) * math.pi / 180

    def __eq__(self, other):
        return self.latitude == other.latitude and self.longitude == other.longitude

    def __str__(self):
        return "Point(%.8f,%.8f)" % (self.latitude,self.longitude)

    def __hash__(self):
        return hash(self.latitude)*hash(self.longitude)



    def GetLocalDensity(self,other,radius):
        distance = self.GetDistance(other)
        return math.exp( 0 - math.pow(distance,2)*1.0 / (2 * math.pow(radius,2)))

if __name__ == "__main__":
    print math.sin(math.pi/2)
    print wbPoint(1,0)
