using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shift.DAO
{
    public class TRX
    {
        string query="select trx.plmn as trxPlmn, trx.name as trxName, trx.adminState as trxAdminState,trx.channel0AdminState,trx.channel0Pcm,trx.channel0Subslot,trx.channel0Tsl,trx.channel0Type,trx.channel1AdminState,trx.channel1Pcm,trx.channel1Subslot,trx.channel1Tsl,trx.channel1Type,trx.channel2AdminState,trx.channel2Pcm,trx.channel2Subslot,trx.channel2Tsl,trx.channel2Type,trx.channel3AdminState,trx.channel3Pcm,trx.channel3Subslot,trx.channel3Tsl,trx.channel3Type,trx.channel4AdminState,trx.channel4Pcm,trx.channel4Subslot,trx.channel4Tsl,trx.channel4Type,trx.channel5AdminState,trx.channel5Pcm,trx.channel5Subslot,trx.channel5Tsl,trx.channel5Type,trx.channel6AdminState,trx.channel6Pcm,trx.channel6Subslot,trx.channel6Tsl,trx.channel6Type,trx.channel7AdminState,trx.channel7Pcm,trx.channel7Subslot,trx.channel7Tsl,trx.channel7Type,trx.daPool_ID,trx.gprsEnabledTrx,trx.halfRateSupport,trx.initialFrequency,trx.lapdLinkName,trx.subslotsForSignalling,trx.tsc,lapd.plmn as lapdPlmn,lapd.bitRate,lapd.abisSigChannelTimeSlotPcm,lapd.abisSigChannelTimeSlotTsl,lapd.abisSigChannelSubSlot,lapd.adminState  as lapdAdminState,lapd.bsc,lapd.dChannelType,lapd.name as lapdName,lapd.parameterSetNumber,lapd.sapi,lapd.tei,lapd.logicalBCSUAddress from trx,lapd where ((trx.bsc=865556 and trx.bcf=1 and trx.trx=3) or (trx.bsc=865556 and trx.bcf=1 and trx.trx=6) or (trx.bsc=865556 and trx.bcf=1 and trx.trx=7) or (trx.bsc=865556 and trx.bcf=1 and trx.trx=10) or (trx.bsc=865556 and trx.bcf=1 and trx.trx=11) or (trx.bsc=865556 and trx.bcf=1 and trx.trx=9) or (trx.bsc=865556 and trx.bcf=1 and trx.trx=5)) and trx.bsc=lapd.bsc and trx.lapdLinkName=lapd.name;";
    }
}
