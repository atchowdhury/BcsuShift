using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shift.DAL;
using Shift.DAO;

namespace BcsuShift
{
    class Program
    {
        static void Main(string[] args)
        {
            string line;
            System.IO.StreamReader file =
           new System.IO.StreamReader(@"D:\input\input.txt");
            List<InputData> aInputList=new List<InputData>();
            ShiftManager aManager=new ShiftManager();
            while ((line = file.ReadLine()) != null)
            {
                InputData aData=new InputData();
                string[] input = line.Split(' ');
                aData.BSC=input[0];
                aData.BCF = input[1];
                aData.BTS = input[2];
                aData.TRX = input[3];
                aData.LAPD = input[4];
                aData.BCSU = input[5];
                aInputList.Add(aData);
            }

            aManager.BcsuShift(aInputList);
        }
    }
}
