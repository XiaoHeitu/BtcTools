using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BtcMiner
{
    public class MinerCore
    {
        MiningData curData = MiningData.empty;
        MiningData newData = MiningData.empty;
        Thread workThread = null;
        Random random = new Random();
        uint minerID;

        public MinerCore(MiningData data, uint minerID)
        {
            this.workThread = new Thread(this.Mining);
            this.minerID = minerID;
            this.Update(data, true);
        }

        public void Start()
        {
            this.workThread.Start();
        }

        public void Update(MiningData data, bool stop)
        {
            newData = data;
            if (stop)
            {
                curData = data;
            }
        }

        public event Action<MiningData> OnMiningSucceeded;

        public void Mining()
        {
            Stopwatch sw = Stopwatch.StartNew();
            uint count = 0;
            while (true)
            {
                if (this.curData.Equals(MiningData.empty))
                {
                    continue;
                }
                this.curData.nonce = new byte[4];
                random.NextBytes(this.curData.nonce);
                this.curData.newBlock = MinerUilts.GetNewBlock(this.curData.version, this.curData.prevHash, this.curData.merkleRoot, this.curData.time, this.curData.bits, this.curData.nonce);
                if (new BigInteger(this.curData.newBlock, true, false) < new BigInteger(this.curData.target, true, false))
                {
                    //挖矿成功

                    var succeeded = this.curData;
                    Task.Run(() =>
                    {
                        if (this.OnMiningSucceeded != null)
                        {
                            this.OnMiningSucceeded(succeeded);
                        }
                    });

                    //this.Update(this.newData, true);
                }

                if (this.curData.taskID != this.newData.taskID)
                {
                    this.curData = this.newData;
                }

                count++;
                var time = sw.Elapsed.TotalSeconds;

                if (time >= 10)
                {
                    MessageHelper.WriteLine(ConsoleColor.Cyan, $"Miner{this.minerID} {count / sw.Elapsed.TotalSeconds / 1024:f3}KH/S");
                    count = 0;
                    sw.Restart();
                }
            }
        }
    }
}
