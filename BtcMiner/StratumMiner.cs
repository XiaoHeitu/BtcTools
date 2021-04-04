using socket.core.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json;
using System.IO;

namespace BtcMiner
{
    public class StratumMiner : Miner
    {
        TcpPushClient client;



        //矿池参数
        string host;
        int port;
        string user;
        string password;

        //连接参数
        byte[] extraNonce1;
        uint extraNonce2Size;

        //任务通知
        string taskID;
        byte[] prevHash;
        byte[] merkleRoot;
        byte[] version;
        byte[] bits;
        byte[] time;

        //验证难度
        byte[] target;

        /// <summary>
        /// Stratum矿工
        /// </summary>
        /// <param name="host">主机</param>
        /// <param name="port">端口</param>
        /// <param name="user">用户</param>
        /// <param name="password">密码</param>
        public StratumMiner(string host, int port, string user, string password)
        {
            this.host = host;
            this.port = port;
            this.user = user;
            this.password = password;

            this.client = new TcpPushClient(40960);
            this.client.OnReceive += this.Client_OnReceive;
            this.client.OnClose += this.Client_OnClose;
            this.client.OnConnect += this.Client_OnConnect;

            this.Connect();
        }

        private void Client_OnReceive(byte[] data)
        {
            this.AnalysisReceive(data);
        }

        private void Client_OnConnect(bool success)
        {
            if (success)
            {
                MessageHelper.WriteLine(ConsoleColor.Yellow, "连接矿池成功");
                this.Subscribe();
            }
            else
            {
                this.Reconnect();
            }
        }

        private void Client_OnClose()
        {
            this.Reconnect();
        }

        /// <summary>
        /// 获取Coinbase
        /// </summary>
        /// <param name="coin1"></param>
        /// <param name="extranonce1"></param>
        /// <param name="extranonce2"></param>
        /// <param name="coin2"></param>
        /// <returns></returns>
        private byte[] GetCoinbase(byte[] coin1, byte[] extranonce1, byte[] coin2)
        {
            //Coinbase=Coinb1 + Extranonce1 + Extranonce2 + Coinb2
            byte[] result = null;
            using (MemoryStream mem = new MemoryStream())
            {
                mem.Write(coin1);
                mem.Write(extraNonce1);
                mem.Write(new byte[] { 0, 0, 0, 0 });
                mem.Write(coin2);

                result = mem.ToArray();
                mem.Close();
            }

            return result;
        }

        private void Subscribe()
        {
            var data = Encoding.UTF8.GetBytes("{\"id\":1,\"method\":\"mining.subscribe\",\"params\":[\"BtcMiner/1.0_bate\"]}\n");
            this.client.Send(data, 0, data.Length);
        }

        private void Authorize()
        {
            var data = Encoding.UTF8.GetBytes($"{{\"id\":2,\"method\":\"mining.authorize\",\"params\":[\"{this.user}\",\"{this.password}\"]}}\n");
            this.client.Send(data, 0, data.Length);
        }

        /// <summary>
        /// 重连矿池
        /// </summary>
        private void Reconnect()
        {
            //this.logger.LogError($"矿池掉线，3秒后重连！");
            MessageHelper.WriteLine(ConsoleColor.Red, "矿池掉线3秒后重连");
            Task.Run(() =>
            {
                Thread.Sleep(3000);
                this.Connect();
            });
        }

        /// <summary>
        /// 连接矿池
        /// </summary>
        private void Connect()
        {
            MessageHelper.WriteLine(ConsoleColor.Yellow, "开始连接矿池");
            this.client.Connect(this.host, this.port);
        }

        /// <summary>
        /// 分析接收数据
        /// </summary>
        /// <param name="data"></param>
        private void AnalysisReceive(byte[] data)
        {
            var dataStr = Encoding.UTF8.GetString(data);
            var jsons = dataStr.Split('\n');
            for (int i = 0; i < jsons.Length; i++)
            {
                var json = jsons[i];

                using (JsonDocument doc = JsonDocument.Parse(json))
                {
                    var ok = doc.RootElement.TryGetProperty("id", out var id);
                    if (ok && id.GetInt32() == 1)
                    {
                        this.AnalysisSubscribeResult(doc);
                        continue;
                    }

                    ok = doc.RootElement.TryGetProperty("method", out var method);
                    if (ok && method.GetString() == "mining.notify")
                    {
                        this.AnalysisNotify(doc);
                    }
                }
            }
        }
        private void AnalysisSubscribeResult(JsonDocument doc)
        {
            var ok = doc.RootElement.TryGetProperty("result", out var result);
            if (ok)
            {
                this.extraNonce1 = this.HexToBytes(result[1].GetString());
                this.extraNonce2Size = result[2].GetUInt32();
            }

            this.Authorize();
        }
        private void AnalysisNotify(JsonDocument doc)
        {
            var ok = doc.RootElement.TryGetProperty("params", out var @params);
            if (ok)
            {
                this.taskID = @params[0].GetString();
                this.prevHash = this.HexToBytes(@params[1].GetString());
                var coinbase1 = this.HexToBytes(@params[2].GetString());
                var coinbase2 = this.HexToBytes(@params[3].GetString());
                var coinbase = this.GetCoinbase(coinbase1, this.extraNonce1, coinbase2);
                var merkles = @params[4].EnumerateArray().Select(d => this.HexToBytes(d.GetString())).ToArray();
                this.merkleRoot = this.GetMerkleRoot(coinbase, merkles);
                this.version = this.HexToBytes(@params[5].GetString());
                this.bits = this.HexToBytes(@params[6].GetString());
                this.time = this.HexToBytes(@params[7].GetString());
            }
        }



        /// <summary>
        /// Hex字符串转字节数组
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        private byte[] HexToBytes(string hex)
        {
            var length = hex.Length / 2;
            byte[] result = new byte[length];
            for (var i = 0; i < length; i++)
            {
                result[i] = byte.Parse(hex.Substring(i * 2, 2), System.Globalization.NumberStyles.HexNumber);
            }

            return result;
        }
    }
}
