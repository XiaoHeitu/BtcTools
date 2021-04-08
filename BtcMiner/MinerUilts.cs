using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BtcMiner
{
    public class MinerUilts
    {
        /// <summary>
        /// 获取新块
        /// </summary>
        /// <param name="version">版本号(4字节)</param>
        /// <param name="prevHash">前块HASH(32字节)</param>
        /// <param name="merkleRoot">交易根(32字节)</param>
        /// <param name="time">封包时间戳(UTC)(4字节)</param>
        /// <param name="targetBits">难度目标(4字节)</param>
        /// <param name="nonce">随机数(4字节)</param>
        /// <returns>新块</returns>
        public static byte[] GetNewBlock(byte[] version, byte[] prevHash, byte[] merkleRoot, byte[] time, byte[] targetBits, byte[] nonce)
        {
            byte[] blockHeader = new byte[80];
            version.CopyTo(blockHeader, 0);
            prevHash.CopyTo(blockHeader, 4);
            merkleRoot.CopyTo(blockHeader, 36);
            time.CopyTo(blockHeader, 68);
            targetBits.CopyTo(blockHeader, 72);
            nonce.CopyTo(blockHeader, 76);

            return SHA256Managed.HashData(SHA256Managed.HashData(blockHeader));
        }

        /// <summary>
        /// 获取交易树
        /// </summary>
        /// <param name="merkles">交易列表(每交易64字节)</param>
        /// <returns>交易树(32字节)</returns>
        public static byte[] GetMerkleRoot(byte[] coinbase, IList<byte[]> merkles)
        {
            List<byte[]> ordered = merkles.OrderBy(d => BitConverter.ToUInt64(d)).ToList();

            if (coinbase != null)
            {
                ordered.Insert(0, coinbase);
            }

            var result = GetMerkleRootHashs(ordered);
            return result[0];
        }

        private static IList<byte[]> GetMerkleRootHashs(IList<byte[]> merkleHashs)
        {
            if (merkleHashs.Count % 2 == 1)
            {
                merkleHashs.Add(merkleHashs.Last());
            }

            IList<byte[]> results = new List<byte[]>();
            for (int i = 0; i < merkleHashs.Count; i += 2)
            {
                using (MemoryStream mem = new MemoryStream())
                {
                    mem.Write(merkleHashs[i]);
                    mem.Write(merkleHashs[i + 1]);

                    results.Add(SHA256Managed.HashData(mem.ToArray()));
                }
            }

            if (results.Count > 1)
            {
                results = GetMerkleRootHashs(results);
            }

            return results;
        }

        /// <summary>
        /// 获取4字节随机数
        /// </summary>
        /// <returns>4字节随机数</returns>
        private byte[] GetNonce()
        {
            return null;
        }

        /// <summary>
        /// Bits转目标难度
        /// </summary>
        /// <param name="bits">Bits(8字节)</param>
        /// <returns>目标难度</returns>
        public static byte[] BitsToTarget(byte[] bits)
        {
            //以十六进制表示，总共有8位，前2位为指数，后6位为系数
            //难度值（target） = 系数 * 2^(8 * (指数 - 3))
            var intbits = BitConverter.ToInt32(bits.Reverse().ToArray());

            //解析bits
            var factor = intbits & 0x00FFFFFF;
            var power = intbits >> 24;

            power = 8 * (power - 3);

            //var target = factor * Math.Pow(2, power);

            var bigInteger = factor * BigInteger.Pow(2, power);
            //var str= BitConverter.ToString(bigInteger.ToByteArray(false,true)).Replace("-", "");
            return bigInteger.ToByteArray(true,false);
        }

        /// <summary>
        /// Hex字符串转字节数组
        /// </summary>
        /// <param name="hex"></param>
        /// <returns></returns>
        public static byte[] HexToBytes(string hex)
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
