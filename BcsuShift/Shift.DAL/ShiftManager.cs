using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Shift.DAO;
using MinimalisticTelnet;

namespace Shift.DAL
{
    public class ShiftManager
    {
        FetchDB fetchDB = new FetchDB();
        public void BcsuShift(List<InputData> aInputList)
        {
            BscMap aMap = new BscMap();
            string bscMap;
            bool connected;
            string createLAPD = "";
            string createTRX = "";
            string deleteTRX = "";
            string queryPart = "";
            List<string> bscList=new List<string>();
            List<BscMap> aMapList=new List<BscMap>();
            List<BCSU> bcsuList = new List<BCSU>();
            for (int i = 0; i < aInputList.Count - 1; i++)
            {
                queryPart += "(trx.BSC=" + aInputList[i].BSC + " and trx.BCF=" + aInputList[i].BCF + " and trx.TRX=" + aInputList[i].TRX + " and lapd.name='" + aInputList[i].LAPD + "') or";
                if (!bscList.Contains(aInputList[i].BSC))
                {
                    bscList.Add(aInputList[i].BSC);
                }
            }

            System.IO.StreamReader ipMap = new StreamReader(@"D:\input\ip_map.csv");
            while ((bscMap = ipMap.ReadLine()) != null)
            {
                BscMap aBscMap = new BscMap();

                string[] map = bscMap.Split(',');
                aBscMap.BSC = map[0];
                aBscMap.Name = map[1];
                aBscMap.IPaddress = map[2];
                aBscMap.BSCuser = map[3];
                aBscMap.BSCpass = map[4];

                aMapList.Add(aBscMap);


            }


            foreach (string aBsc in bscList)
            {

                BscMap aBscMap=aMapList.Where(d => d.BSC == aBsc).First();

                TelnetConnection aTelnetConnection = new TelnetConnection(aBscMap.IPaddress, 23);
                aTelnetConnection.Login(aBscMap.BSCuser, aBscMap.BSCpass, 100, out connected);

                aTelnetConnection.ExecuteCommand("ZDDS;");
                string logBCSU = aTelnetConnection.ExecuteCommand("ZLE:U,RCBUGGGX");
                logBCSU += aTelnetConnection.ExecuteCommand("U");
                logBCSU = aTelnetConnection.ExecuteCommand("ZUSI:BCSU");
                aTelnetConnection.ExecuteCommand("Z");
                aTelnetConnection.ExecuteCommand("Z");
                aTelnetConnection.ExecuteCommand("ZE");
                // System.Security

                logBCSU = Regex.Unescape(logBCSU);
                logBCSU = logBCSU.Replace("     ", " ");
                
                string[] aStrings = logBCSU.Split('\n');

                foreach (string line in aStrings)
                {
                    if (line.Contains("BCSU-"))
                    {
                        string[] a1 = line.Split(' ');
                        BCSU aBcsu = new BCSU();
                        aBcsu.BSC = aBsc;
                        string[] bcsuSplit=a1[0].Split('-');
                        aBcsu.BCSUiD = bcsuSplit[1];
                        int decAgain = int.Parse(a1[2], System.Globalization.NumberStyles.HexNumber);
                        aBcsu.BCSUlogical = decAgain.ToString();
                        bcsuList.Add(aBcsu);
                    }
                }



            }




            queryPart += "(trx.BSC=" + aInputList[aInputList.Count - 1].BSC + " and trx.BCF=" + aInputList[aInputList.Count - 1].BCF + " and trx.TRX=" + aInputList[aInputList.Count - 1].TRX + " and lapd.name='" + aInputList[aInputList.Count - 1].LAPD + "')";
            List<BCSU> aBCSUList = new List<BCSU>();
            List<TrxLAPD> trxLapds = fetchDB.GetLAPDBitrate(queryPart, out aBCSUList);
            foreach (InputData data in aInputList)
            {
                

                try
                {
                    //string targetBCSU = aBCSUList.Where(d => d.BSC == data.BSC && d.BCSUiD == data.BCSU).First().BCSUlogical;

                    //trxLapds.Where(d => d.BSC == data.BSC && d.BCF == data.BCF && d.TRX == data.TRX)
                    //    .First()
                    //    .TargetLAPDlogicalBCSUAddress = targetBCSU;

                    string LogicalBCSU =
                        bcsuList.Where(d => d.BSC == data.BSC && d.BCSUiD == data.BCSU).First().BCSUlogical;

                    trxLapds.Where(d => d.BSC == data.BSC && d.BCF == data.BCF && d.TRX == data.TRX)
                        .First()
                        .TargetLAPDlogicalBCSUAddress = LogicalBCSU;
                }
                catch
                {
                    trxLapds.RemoveAll(d => d.BSC == data.BSC && d.BCF == data.BCF && d.TRX == data.TRX);
                }

                //trxLapds.Where(d => d.BSC == data.BSC && d.BCF == data.BCF && d.TRX == data.TRX)
                //    .First()
                //    .TargetLAPDlogicalBCSUAddress = "16734";

            }

            foreach (TrxLAPD trxLapd in trxLapds)
            {

               deleteTRX+= "<managedObject class=\"LAPD\" version=\"S15\" distName=\""+trxLapd.LAPDplmn+"\" operation=\"delete\"> </managedObject>\n";
	deleteTRX+="<managedObject class=\"TRX\" version=\"S15\" distName=\""+trxLapd.TRXPlmn+"\" operation=\"delete\"> </managedObject>\n";

                createLAPD = "<managedObject class=\"LAPD\" version=\"S15.3\" distName=\"" + trxLapd.LAPDplmn + "\" operation=\"create\">\n";
                
              

                createLAPD += "<p name=\"bitRate\">" + trxLapd.LAPDbitRate + "</p>\n";
                createLAPD += "<p name=\"abisSigChannelTimeSlotPcm\">" + trxLapd.LAPDabisSigChannelTimeSlotPcm + "</p>\n";
                createLAPD += "<p name=\"abisSigChannelTimeSlotTsl\">" + trxLapd.LAPDabisSigChannelTimeSlotTsl + "</p>\n";
                createLAPD += "<p name=\"abisSigChannelSubSlot\">" + trxLapd.LAPDabisSigChannelSubSlot + "</p>\n";
                createLAPD += "<p name=\"adminState\">" + trxLapd.LAPDadminState + "</p>\n";
                createLAPD += "<p name=\"parentBSCId\">" + trxLapd.LAPDBSCId + "</p>\n";
                createLAPD += "<p name=\"dChannelType\">" + trxLapd.LAPDdChannelType + "</p>\n";
                createLAPD += "<p name=\"name\">" + trxLapd.LAPDname + "</p>\n";
                createLAPD += "<p name=\"parameterSetNumber\">" + trxLapd.LAPDparameterSetNumber + "</p>\n";
                createLAPD += "<p name=\"sapi\">" + trxLapd.LAPDsapi + "</p>\n";
                createLAPD += "<p name=\"tei\">" + trxLapd.LAPDtei + "</p>\n";
                createLAPD += "<p name=\"logicalBCSUAddress\">" + trxLapd.TargetLAPDlogicalBCSUAddress + "</p>\n";
               // createLAPD += "<p name=\"bcsuID\">" + trxLapd.TargetLAPDlogicalBCSUAddress + "</p>\n";
                createLAPD += "</managedObject>\n";
                




                createTRX+=createLAPD+"<managedObject class=\"TRX\" version=\"S15.3\" distName=\""+trxLapd.TRXPlmn+"\" operation=\"create\">\n";
               





               // createTRX += "<p name=\"trxPlmn\">" + trxLapd.TRXPlmn +"</p>\n";
                createTRX += "<p name=\"name\">" + trxLapd.TRXname +"</p>\n";
                createTRX += "<p name=\"adminState\">" + trxLapd.TRXadminState +"</p>\n";
                createTRX += "<p name=\"channel0AdminState\">" + trxLapd.TRXchannel0AdminState +"</p>\n";
                createTRX += "<p name=\"channel0Pcm\">" + trxLapd.TRXchannel0Pcm +"</p>\n";
                createTRX += "<p name=\"channel0Subslot\">" + trxLapd.TRXchannel0Subslot +"</p>\n";
                createTRX += "<p name=\"channel0Tsl\">" + trxLapd.TRXchannel0Tsl +"</p>\n";
                createTRX += "<p name=\"channel0Type\">" + trxLapd.TRXchannel0Type +"</p>\n";
                createTRX += "<p name=\"channel1AdminState\">" + trxLapd.TRXchannel1AdminState +"</p>\n";
                createTRX += "<p name=\"channel1Pcm\">" + trxLapd.TRXchannel1Pcm +"</p>\n";
                createTRX += "<p name=\"channel1Subslot\">" + trxLapd.TRXchannel1Subslot +"</p>\n";
                createTRX += "<p name=\"channel1Tsl\">" + trxLapd.TRXchannel1Tsl +"</p>\n";
                createTRX += "<p name=\"channel1Type\">" + trxLapd.TRXchannel1Type +"</p>\n";
                createTRX += "<p name=\"channel2AdminState\">" + trxLapd.TRXchannel2AdminState +"</p>\n";
                createTRX += "<p name=\"channel2Pcm\">" + trxLapd.TRXchannel2Pcm +"</p>\n";
                createTRX += "<p name=\"channel2Subslot\">" + trxLapd.TRXchannel2Subslot +"</p>\n";
                createTRX += "<p name=\"channel2Tsl\">" + trxLapd.TRXchannel2Tsl +"</p>\n";
                createTRX += "<p name=\"channel2Type\">" + trxLapd.TRXchannel2Type +"</p>\n";
                createTRX += "<p name=\"channel3AdminState\">" + trxLapd.TRXchannel3AdminState +"</p>\n";
                createTRX += "<p name=\"channel3Pcm\">" + trxLapd.TRXchannel3Pcm +"</p>\n";
                createTRX += "<p name=\"channel3Subslot\">" + trxLapd.TRXchannel3Subslot +"</p>\n";
                createTRX += "<p name=\"channel3Tsl\">" + trxLapd.TRXchannel3Tsl +"</p>\n";
                createTRX += "<p name=\"channel3Type\">" + trxLapd.TRXchannel3Type +"</p>\n";
                createTRX += "<p name=\"channel4AdminState\">" + trxLapd.TRXchannel4AdminState +"</p>\n";
                createTRX += "<p name=\"channel4Pcm\">" + trxLapd.TRXchannel4Pcm +"</p>\n";
                createTRX += "<p name=\"channel4Subslot\">" + trxLapd.TRXchannel4Subslot +"</p>\n";
                createTRX += "<p name=\"channel4Tsl\">" + trxLapd.TRXchannel4Tsl +"</p>\n";
                createTRX += "<p name=\"channel4Type\">" + trxLapd.TRXchannel4Type +"</p>\n";
                createTRX += "<p name=\"channel5AdminState\">" + trxLapd.TRXchannel5AdminState +"</p>\n";
                createTRX += "<p name=\"channel5Pcm\">" + trxLapd.TRXchannel5Pcm +"</p>\n";
                createTRX += "<p name=\"channel5Subslot\">" + trxLapd.TRXchannel5Subslot +"</p>\n";
                createTRX += "<p name=\"channel5Tsl\">" + trxLapd.TRXchannel5Tsl +"</p>\n";
                createTRX += "<p name=\"channel5Type\">" + trxLapd.TRXchannel5Type +"</p>\n";
                createTRX += "<p name=\"channel6AdminState\">" + trxLapd.TRXchannel6AdminState +"</p>\n";
                createTRX += "<p name=\"channel6Pcm\">" + trxLapd.TRXchannel6Pcm +"</p>\n";
                createTRX += "<p name=\"channel6Subslot\">" + trxLapd.TRXchannel6Subslot +"</p>\n";
                createTRX += "<p name=\"channel6Tsl\">" + trxLapd.TRXchannel6Tsl +"</p>\n";
                createTRX += "<p name=\"channel6Type\">" + trxLapd.TRXchannel6Type +"</p>\n";
                createTRX += "<p name=\"channel7AdminState\">" + trxLapd.TRXchannel7AdminState +"</p>\n";
                createTRX += "<p name=\"channel7Pcm\">" + trxLapd.TRXchannel7Pcm +"</p>\n";
                createTRX += "<p name=\"channel7Subslot\">" + trxLapd.TRXchannel7Subslot +"</p>\n";
                createTRX += "<p name=\"channel7Tsl\">" + trxLapd.TRXchannel7Tsl +"</p>\n";
                createTRX += "<p name=\"channel7Type\">" + trxLapd.TRXchannel7Type +"</p>\n";
                createTRX += "<p name=\"daPool_ID\">" + trxLapd.TRXdaPool_ID +"</p>\n";
                createTRX += "<p name=\"gprsEnabledTrx\">" + trxLapd.TRXgprsEnabledTrx +"</p>\n";
                createTRX += "<p name=\"halfRateSupport\">" + trxLapd.TRXhalfRateSupport +"</p>\n";
                createTRX += "<p name=\"initialFrequency\">" + trxLapd.TRXinitialFrequency +"</p>\n";
                createTRX += "<p name=\"lapdLinkName\">" + trxLapd.TRXlapdLinkName +"</p>\n";
                createTRX += "<p name=\"subslotsForSignalling\">" + trxLapd.TRXsubslotsForSignalling +"</p>\n";
                createTRX += "<p name=\"tsc\">" + trxLapd.TRXtsc +"</p>\n";
                createTRX += "<p name=\"preferredBcchMark\">" + trxLapd.PreferedBcchTRX +"</p>\n";
                



                createTRX += "</managedObject>\n";




            }

            string xmlFileCreate="<?xml version=\"1.0\"?>\n <!DOCTYPE raml SYSTEM 'raml20.dtd'>\n <raml version=\"2.0\" xmlns=\"raml20.xsd\">\n<cmData type=\"plan\">\n<header>\n<log dateTime=\"\" action=\"created\" user=\"blOMC\" appInfo=\"blNokiaTool\"/>\n</header>";
            xmlFileCreate += createTRX;
            xmlFileCreate += "</cmData>\n</raml>";

            


            using (StreamWriter aWriter = new StreamWriter("createTRX.xml"))
            {
                aWriter.Write(xmlFileCreate);
                aWriter.Close();
            }

            string xmlFileDelete = "<?xml version=\"1.0\"?>\n <!DOCTYPE raml SYSTEM 'raml20.dtd'>\n <raml version=\"2.0\" xmlns=\"raml20.xsd\">\n<cmData type=\"plan\">\n<header>\n<log dateTime=\"\" action=\"created\" user=\"blOMC\" appInfo=\"blNokiaTool\"/>\n</header>";
            xmlFileDelete += deleteTRX;
            xmlFileDelete += "</cmData>\n</raml>";
            using (StreamWriter aWriter = new StreamWriter("deleteTRX.xml"))
            {

                aWriter.Write(xmlFileDelete);
                aWriter.Close();
            }
        }


    }
}
