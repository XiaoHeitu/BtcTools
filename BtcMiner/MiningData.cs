using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BtcMiner
{
    public struct MiningData
    {
        public static MiningData empty = new MiningData();

        //连接参数
        public byte[] extraNonce1;
        public uint extraNonce2Size;

        //任务通知
        public string taskID;
        public byte[] prevHash;
        public byte[] extraNonce2;
        public byte[] merkleRoot;
        public byte[] version;
        public byte[] bits;
        public byte[] time;

        //验证难度
        public byte[] target;

        //块数据
        public byte[] nonce;
        public byte[] newBlock;
    }
}
