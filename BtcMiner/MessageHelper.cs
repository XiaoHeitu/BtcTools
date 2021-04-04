using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BtcMiner
{
    public class MessageHelper
    {
        private static object lockobj = new object();

        /// <summary>
        /// 写入消息
        /// </summary>
        /// <param name="color">消息颜色</param>
        /// <param name="message">消息内容</param>
        public static void Write(ConsoleColor color, string message)
        {
            lock (lockobj)
            {
                Console.ForegroundColor = color;
                Console.Write(message);
                Console.ResetColor();
            }
        }

        /// <summary>
        /// 写入一行消息
        /// </summary>
        /// <param name="color">消息颜色</param>
        /// <param name="message">消息内容</param>
        public static void WriteLine(ConsoleColor color, string message)
        {
            lock (lockobj)
            {
                Console.ForegroundColor = color;
                Console.WriteLine(message);
                Console.ResetColor();
            }
        }
    }
}
