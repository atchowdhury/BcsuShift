using System;
using System.Collections.Generic;
//using System.Data.SqlClient;
using System.Data.Odbc;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using Shift.DAO;
using MinimalisticTelnet;




namespace Shift.DAO
{
    public class FetchDB
    {
        string server = "localhost";
        string database = "myflexml";
        string uid = "root";
        string password = "";
       


        // string connectionStr = ConfigurationManager.ConnectionStrings["connectionStringForEmployeDB"].ConnectionString;
        string connectionStr = string.Format("Server=localhost; database={0}; UID=root;", "myflexml");
        private MySqlConnection aSqlConnection;
        private MySqlCommand aSqlCommand;
        public FetchDB()
        {
            string connectionString = "SERVER=" + server + ";" + "DATABASE=" +
                database + ";" + "UID=" + uid + ";" + "Pwd=" + password + "; Port=3306;";

            string conn = "Server = localhost;  Database = myflexml; Uid = root;Pwd = '';";
            //aSqlConnection = new SqlConnection(connectionStr);
            aSqlConnection = new MySqlConnection(conn);

        }


        public List<BCSU> GetLogicalBCSU(List<string> bscList )
        {
            System.IO.StreamReader ipMap = new StreamReader(@"D:\input\ip_map.csv");
            BscMap aMap = new BscMap();
            string bscMap;
            bool connected;
            foreach (string bsc in bscList)
            {


                while ((bscMap = ipMap.ReadLine()) != null)
                {

                    if (bscMap.Contains(bsc))
                    {
                        string[] map = bscMap.Split(',');
                        aMap.Name = map[1];
                        aMap.IPaddress = map[2];
                        aMap.BSCuser = map[3];
                        aMap.BSCpass = map[4];
                    }
                }
                try
                {
                    TelnetConnection aTelnetConnection = new TelnetConnection(aMap.IPaddress, 23);
                    aTelnetConnection.Login(aMap.BSCuser, aMap.BSCpass, 100, out connected);
                    aTelnetConnection.ExecuteCommand("ZDDS;");
                    string logBCSU = aTelnetConnection.ExecuteCommand("ZLE:U,RCBUGGGX");
                    logBCSU += aTelnetConnection.ExecuteCommand("U");
                    logBCSU += aTelnetConnection.ExecuteCommand("ZUSI:BCSU");
                    logBCSU += aTelnetConnection.ExecuteCommand("Z");
                    logBCSU += aTelnetConnection.ExecuteCommand("Z");
                    logBCSU += aTelnetConnection.ExecuteCommand("ZE");
                }
                catch
                {
                    connected = false;
                }
            }


            return null;
        }  



        public List<TrxLAPD> GetLAPDBitrate(string queryPart, out List<BCSU> aBCSuList)
        {
            //string qLapd = string.Format("select bsc,lapd,bitrate from lapd where bsc={0} and name='{1}'", bsc, lapd);
            //string bcsuQuery = "select distinct bsc,bcsuid,logicalBCSUAddress from dap";
             aBCSuList=new List<BCSU>();

             aSqlConnection.Open();
            //aSqlCommand = new MySqlCommand(bcsuQuery, aSqlConnection);
            //MySqlDataReader aDataReader = aSqlCommand.ExecuteReader();

            //while (aDataReader.Read())
            //{
            //    BCSU aBcsu = new BCSU();
            //    aBcsu.BSC = aDataReader[0].ToString();
            //    aBcsu.BCSUiD = aDataReader[1].ToString();
            //    aBcsu.BCSUlogical = aDataReader[2].ToString();

            //    aBCSuList.Add(aBcsu);
            //}
            //aDataReader.Close();

            string query = string.Format("select trx.plmn as trxPlmn, trx.name as trxName, trx.adminState as trxAdminState,trx.channel0AdminState,trx.channel0Pcm,trx.channel0Subslot,trx.channel0Tsl,trx.channel0Type,trx.channel1AdminState,trx.channel1Pcm,trx.channel1Subslot,trx.channel1Tsl,trx.channel1Type,trx.channel2AdminState,trx.channel2Pcm,trx.channel2Subslot,trx.channel2Tsl,trx.channel2Type,trx.channel3AdminState,trx.channel3Pcm,trx.channel3Subslot,trx.channel3Tsl,trx.channel3Type,trx.channel4AdminState,trx.channel4Pcm,trx.channel4Subslot,trx.channel4Tsl,trx.channel4Type,trx.channel5AdminState,trx.channel5Pcm,trx.channel5Subslot,trx.channel5Tsl,trx.channel5Type,trx.channel6AdminState,trx.channel6Pcm,trx.channel6Subslot,trx.channel6Tsl,trx.channel6Type,trx.channel7AdminState,trx.channel7Pcm,trx.channel7Subslot,trx.channel7Tsl,trx.channel7Type,trx.daPool_ID,trx.gprsEnabledTrx,trx.halfRateSupport,trx.initialFrequency,trx.lapdLinkName,trx.subslotsForSignalling,trx.tsc,lapd.plmn as lapdPlmn,lapd.bitRate,lapd.abisSigChannelTimeSlotPcm,lapd.abisSigChannelTimeSlotTsl,lapd.abisSigChannelSubSlot,lapd.adminState  as lapdAdminState,lapd.bsc,lapd.dChannelType,lapd.name as lapdName,lapd.parameterSetNumber,lapd.sapi,lapd.tei,lapd.logicalBCSUAddress,trx.bsc,trx.bcf,trx.bts,trx.trx,trx.preferredBcchMark from trx,lapd where ({0}) and trx.bsc=lapd.bsc and trx.lapdLinkName=lapd.name", queryPart);
            
            aSqlCommand = new MySqlCommand(query, aSqlConnection);
            MySqlDataReader aDataReaderInfo = aSqlCommand.ExecuteReader();
            List<TrxLAPD> aList = new List<TrxLAPD>();

            while (aDataReaderInfo.Read())
            {
                TrxLAPD aLapd = new TrxLAPD();

               // aLapd.TRXPlmn = aDataReaderInfo[0].ToString();
               // aLapd.TRXname = aDataReaderInfo[1].ToString();
               // aLapd.TRXadminState = aDataReaderInfo[2].ToString();
               // aLapd.TRXchannel0AdminState = aDataReaderInfo[3].ToString();
               // aLapd.TRXchannel0Pcm = aDataReaderInfo[4].ToString();
               // aLapd.TRXchannel0Subslot = aDataReaderInfo[5].ToString();
               // aLapd.TRXchannel0Tsl = aDataReaderInfo[6].ToString();
               // aLapd.TRXchannel0Type = aDataReaderInfo[7].ToString();
               // aLapd.TRXchannel1AdminState = aDataReaderInfo[8].ToString();
               // aLapd.TRXchannel1Pcm = aDataReaderInfo[9].ToString();
               // aLapd.TRXchannel1Subslot = aDataReaderInfo[10].ToString();
               // aLapd.TRXchannel1Tsl = aDataReaderInfo[11].ToString();
               // aLapd.TRXchannel1Type = aDataReaderInfo[12].ToString();
               // aLapd.TRXchannel2AdminState = aDataReaderInfo[13].ToString();
               // aLapd.TRXchannel2Pcm = aDataReaderInfo[14].ToString();
               // aLapd.TRXchannel2Subslot = aDataReaderInfo[15].ToString();
               // aLapd.TRXchannel2Tsl = aDataReaderInfo[16].ToString();
               // aLapd.TRXchannel2Type = aDataReaderInfo[17].ToString();
               // aLapd.TRXchannel3AdminState = aDataReaderInfo[18].ToString();
               // aLapd.TRXchannel3Pcm = aDataReaderInfo[19].ToString();
               // aLapd.TRXchannel3Subslot = aDataReaderInfo[20].ToString();
               // aLapd.TRXchannel3Tsl = aDataReaderInfo[21].ToString();
               // aLapd.TRXchannel3Type = aDataReaderInfo[22].ToString();
               // aLapd.TRXchannel4AdminState = aDataReaderInfo[23].ToString();
               // aLapd.TRXchannel4Pcm = aDataReaderInfo[24].ToString();
               // aLapd.TRXchannel4Subslot = aDataReaderInfo[25].ToString();
               // aLapd.TRXchannel4Tsl = aDataReaderInfo[26].ToString();
               // aLapd.TRXchannel4Type = aDataReaderInfo[27].ToString();
               // aLapd.TRXchannel5AdminState = aDataReaderInfo[28].ToString();
               // aLapd.TRXchannel5Pcm = aDataReaderInfo[29].ToString();
               // aLapd.TRXchannel5Subslot = aDataReaderInfo[30].ToString();
               // aLapd.TRXchannel5Tsl = aDataReaderInfo[31].ToString();
               // aLapd.TRXchannel5Type = aDataReaderInfo[32].ToString();
               // aLapd.TRXchannel6AdminState = aDataReaderInfo[33].ToString();
               // aLapd.TRXchannel6Pcm = aDataReaderInfo[34].ToString();
               // aLapd.TRXchannel6Subslot = aDataReaderInfo[35].ToString();
               // aLapd.TRXchannel6Tsl = aDataReaderInfo[36].ToString();
               // aLapd.TRXchannel6Type = aDataReaderInfo[37].ToString();
               // aLapd.TRXchannel7AdminState = aDataReaderInfo[38].ToString();
               // aLapd.TRXchannel7Pcm = aDataReaderInfo[39].ToString();
               // aLapd.TRXchannel7Subslot = aDataReaderInfo[40].ToString();
               // aLapd.TRXchannel7Tsl = aDataReaderInfo[41].ToString();
               // aLapd.TRXchannel7Type = aDataReaderInfo[42].ToString();
               // aLapd.TRXdaPool_ID = aDataReaderInfo[43].ToString();
               // aLapd.TRXgprsEnabledTrx = aDataReaderInfo[44].ToString();
               // aLapd.TRXhalfRateSupport = aDataReaderInfo[45].ToString();
               // aLapd.TRXinitialFrequency = aDataReaderInfo[46].ToString();
               // aLapd.TRXlapdLinkName = aDataReaderInfo[47].ToString();
               // aLapd.TRXsubslotsForSignalling = aDataReaderInfo[48].ToString();
               // aLapd.TRXtsc = aDataReaderInfo[49].ToString();
               // aLapd.LAPDdName = aDataReaderInfo[50].ToString();
               //aLapd.LAPDplmn = aDataReaderInfo[51].ToString();
               // aLapd.LAPDbitRate = aDataReaderInfo[52].ToString();
               // aLapd.LAPDabisSigChannelTimeSlotPcm = aDataReaderInfo[53].ToString();
               // aLapd.LAPDabisSigChannelTimeSlotTsl = aDataReaderInfo[54].ToString();
               // aLapd.LAPDabisSigChannelSubSlot = aDataReaderInfo[55].ToString();
               // aLapd.LAPDadminState = aDataReaderInfo[56].ToString();
               // aLapd.LAPDBSCId = aDataReaderInfo[57].ToString();
               // aLapd.LAPDdChannelType = aDataReaderInfo[58].ToString();
               // aLapd.LAPDname = aDataReaderInfo[59].ToString();
               // aLapd.LAPDparameterSetNumber = aDataReaderInfo[60].ToString();
               // aLapd.LAPDsapi = aDataReaderInfo[61].ToString();
               // aLapd.LAPDtei = aDataReaderInfo[62].ToString();
               // aLapd.LAPDlogicalBCSUAddress = aDataReaderInfo[63].ToString();
               // aLapd.BSC = aDataReaderInfo[64].ToString();
               // aLapd.BCF = aDataReaderInfo[65].ToString();
               // aLapd.BTS = aDataReaderInfo[66].ToString();
               // aLapd.TRX = aDataReaderInfo[67].ToString();


                aLapd.TRXPlmn = aDataReaderInfo[0].ToString();
                aLapd.TRXname = aDataReaderInfo[1].ToString();
                aLapd.TRXadminState = aDataReaderInfo[2].ToString();
                aLapd.TRXchannel0AdminState = aDataReaderInfo[3].ToString();
                aLapd.TRXchannel0Pcm = aDataReaderInfo[4].ToString();
                aLapd.TRXchannel0Subslot = aDataReaderInfo[5].ToString();
                aLapd.TRXchannel0Tsl = aDataReaderInfo[6].ToString();
                aLapd.TRXchannel0Type = aDataReaderInfo[7].ToString();
                aLapd.TRXchannel1AdminState = aDataReaderInfo[8].ToString();
                aLapd.TRXchannel1Pcm = aDataReaderInfo[9].ToString();
                aLapd.TRXchannel1Subslot = aDataReaderInfo[10].ToString();
                aLapd.TRXchannel1Tsl = aDataReaderInfo[11].ToString();
                aLapd.TRXchannel1Type = aDataReaderInfo[12].ToString();
                aLapd.TRXchannel2AdminState = aDataReaderInfo[13].ToString();
                aLapd.TRXchannel2Pcm = aDataReaderInfo[14].ToString();
                aLapd.TRXchannel2Subslot = aDataReaderInfo[15].ToString();
                aLapd.TRXchannel2Tsl = aDataReaderInfo[16].ToString();
                aLapd.TRXchannel2Type = aDataReaderInfo[17].ToString();
                aLapd.TRXchannel3AdminState = aDataReaderInfo[18].ToString();
                aLapd.TRXchannel3Pcm = aDataReaderInfo[19].ToString();
                aLapd.TRXchannel3Subslot = aDataReaderInfo[20].ToString();
                aLapd.TRXchannel3Tsl = aDataReaderInfo[21].ToString();
                aLapd.TRXchannel3Type = aDataReaderInfo[22].ToString();
                aLapd.TRXchannel4AdminState = aDataReaderInfo[23].ToString();
                aLapd.TRXchannel4Pcm = aDataReaderInfo[24].ToString();
                aLapd.TRXchannel4Subslot = aDataReaderInfo[25].ToString();
                aLapd.TRXchannel4Tsl = aDataReaderInfo[26].ToString();
                aLapd.TRXchannel4Type = aDataReaderInfo[27].ToString();
                aLapd.TRXchannel5AdminState = aDataReaderInfo[28].ToString();
                aLapd.TRXchannel5Pcm = aDataReaderInfo[29].ToString();
                aLapd.TRXchannel5Subslot = aDataReaderInfo[30].ToString();
                aLapd.TRXchannel5Tsl = aDataReaderInfo[31].ToString();
                aLapd.TRXchannel5Type = aDataReaderInfo[32].ToString();
                aLapd.TRXchannel6AdminState = aDataReaderInfo[33].ToString();
                aLapd.TRXchannel6Pcm = aDataReaderInfo[34].ToString();
                aLapd.TRXchannel6Subslot = aDataReaderInfo[35].ToString();
                aLapd.TRXchannel6Tsl = aDataReaderInfo[36].ToString();
                aLapd.TRXchannel6Type = aDataReaderInfo[37].ToString();
                aLapd.TRXchannel7AdminState = aDataReaderInfo[38].ToString();
                aLapd.TRXchannel7Pcm = aDataReaderInfo[39].ToString();
                aLapd.TRXchannel7Subslot = aDataReaderInfo[40].ToString();
                aLapd.TRXchannel7Tsl = aDataReaderInfo[41].ToString();
                aLapd.TRXchannel7Type = aDataReaderInfo[42].ToString();
                aLapd.TRXdaPool_ID = aDataReaderInfo[43].ToString();
                aLapd.TRXgprsEnabledTrx = aDataReaderInfo[44].ToString();
                aLapd.TRXhalfRateSupport = aDataReaderInfo[45].ToString();
                aLapd.TRXinitialFrequency = aDataReaderInfo[46].ToString();
                aLapd.TRXlapdLinkName = aDataReaderInfo[47].ToString();
                aLapd.TRXsubslotsForSignalling = aDataReaderInfo[48].ToString();
                aLapd.TRXtsc = aDataReaderInfo[49].ToString();
                aLapd.LAPDplmn = aDataReaderInfo[50].ToString();
                aLapd.LAPDbitRate = aDataReaderInfo[51].ToString();
                aLapd.LAPDabisSigChannelTimeSlotPcm = aDataReaderInfo[52].ToString();
                aLapd.LAPDabisSigChannelTimeSlotTsl = aDataReaderInfo[53].ToString();
                aLapd.LAPDabisSigChannelSubSlot = aDataReaderInfo[54].ToString();
                aLapd.LAPDadminState = aDataReaderInfo[55].ToString();
                aLapd.LAPDBSCId = aDataReaderInfo[56].ToString();
                aLapd.LAPDdChannelType = aDataReaderInfo[57].ToString();
                aLapd.LAPDname = aDataReaderInfo[58].ToString();
                aLapd.LAPDparameterSetNumber = aDataReaderInfo[59].ToString();
                aLapd.LAPDsapi = aDataReaderInfo[60].ToString();
                aLapd.LAPDtei = aDataReaderInfo[61].ToString();
                aLapd.LAPDlogicalBCSUAddress = aDataReaderInfo[62].ToString();
                aLapd.BSC = aDataReaderInfo[63].ToString();
                aLapd.BCF = aDataReaderInfo[64].ToString();
                aLapd.BTS = aDataReaderInfo[65].ToString();
                aLapd.TRX = aDataReaderInfo[66].ToString();
                aLapd.PreferedBcchTRX = aDataReaderInfo[67].ToString();




                aList.Add(aLapd);

               
            }

            aDataReaderInfo.Close();
            aSqlConnection.Close();
             return aList;

            
        }

        
    }
}
