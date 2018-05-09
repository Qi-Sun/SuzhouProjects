using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PointToArea.Para
{
    public class P2PmethodPara
    {
        public int TaskMaxNum;
        public int StartIndex;
        public int EndIndex;
        public int Count;
        public int EachFileInterval;
        public int EachTaskInterval;
        public double Para_K;

        public double Density_Tao;
        public P2PmethodPara(int start,int end,int taskCount,int fileInterval,int taskInterval,double density,double k)
        {
            StartIndex = start;
            EndIndex = end;
            TaskMaxNum = taskCount;
            EachFileInterval = fileInterval;
            EachTaskInterval = taskInterval;
            Count = end - start;
            Density_Tao = density;
            Para_K = k;
        }
    }
}
