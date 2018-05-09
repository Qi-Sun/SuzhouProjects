using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace PointToArea.Para
{
    public class DataDirPara
    {
        public string RootDirectory = "";
        public string SampleSetDirectory = "";
        public string DbscanResultDirectory = "";
        public string Point2PlaceDirectory = "";
        public string LogDirectory = "";

        public DataDirPara() { }
        public DataDirPara(string rootDir)
        {
            RootDirectory = rootDir;
            SampleSetDirectory = rootDir + "1_SampleSetData\\";
            DbscanResultDirectory = rootDir + "2_DbscanResult\\";
            Point2PlaceDirectory = rootDir + "3_Point2Place\\";
            LogDirectory = rootDir + "Log\\";
            if (!Directory.Exists(RootDirectory))
                Directory.CreateDirectory(RootDirectory);
            if (!Directory.Exists(SampleSetDirectory))
                Directory.CreateDirectory(SampleSetDirectory);
            if (!Directory.Exists(DbscanResultDirectory))
                Directory.CreateDirectory(DbscanResultDirectory);
            if (!Directory.Exists(LogDirectory))
                Directory.CreateDirectory(LogDirectory);
        }
    }
}
