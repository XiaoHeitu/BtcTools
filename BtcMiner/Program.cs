using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace BtcMiner
{
    class Program
    {
        static object lockobj = new object();
        static void Main(string[] args)
        {
            StratumMiner m = new StratumMiner("ss.antpool.com",3333,"ywj6792341BTC.001","x");

            //var bits = MinerUilts.HexToBytes("1903a30c");
            //MinerUilts.BitsToTarget(bits);

            while (Console.ReadLine().ToUpper() != "EXIT")
            {

            }
            //Miner m = new Miner();

            //Stopwatch sw = Stopwatch.StartNew();
            //long count = 0;
            //Task[] tasks = new Task[8];
            //for (int ti = 0; ti < tasks.Length; ti++)
            //{
            //    tasks[ti] = Task.Run(() =>
            //    {
            //        while (true)
            //        {
            //            var block = m.GetNewBlock(
            //        "02000000",
            //        "69054f28012b4474caa9e821102655cc74037c415ad2bba70200000000000000",
            //        "2ecfc74ceb512c5055bcff7e57735f7323c32f8bbb48f5e96307e5268c001cc9",
            //        "3a09be52",
            //        "0ca30319",
            //        "88261c37");
            //            lock (lockobj)
            //            {
            //                count++;
            //                var pcount = 1000000;
            //                if (count % pcount == 0)
            //                {
            //                    var time = sw.Elapsed.TotalSeconds;
            //                    Console.WriteLine($"数量：{pcount}，用时{time:f2}秒，速度：{ pcount / time / 1024:f3} KH/s");
            //                    sw.Restart();
            //                }
            //            }
            //        }
            //    });
            //}

            //Task.WaitAll(tasks);
            //while (true)
            //{
            //    var block = m.GetNewBlock(
            //        "02000000",
            //        "69054f28012b4474caa9e821102655cc74037c415ad2bba70200000000000000",
            //        "2ecfc74ceb512c5055bcff7e57735f7323c32f8bbb48f5e96307e5268c001cc9",
            //        "3a09be52",
            //        "0ca30319",
            //        "88261c37");
            //    count++;
            //    var pcount = 1000000;
            //    if (count % pcount == 0)
            //    {
            //        var time = sw.Elapsed.TotalSeconds;
            //        Console.WriteLine($"数量：{pcount}，用时{time:f2}秒，速度：{ pcount / time / 1024:f3} KH/s");
            //        sw.Restart();
            //    }
            //}

            //Console.WriteLine(BitConverter.ToString(block.Reverse().ToArray()).Replace("-", "").ToLower());
        }
    }
}
