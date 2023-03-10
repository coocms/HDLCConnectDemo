using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HDLCDeviceConnectDemo
{
    public class Protocal376_2
    {
        public static byte[] Protocal376_2Process((int, byte[]) data,int frameCount, int totalCountStr)
        {
            List<byte> res = new List<byte>();
            res.AddRange(new List<byte> { 0x68, 0x9e, 0x00, 0x41, 0x08, 0x00});//固定部分
            if (data.Item1 > 0xffffffff)
            {
                Console.WriteLine("Protocal376_2Process");
            }
            res.AddRange(IntToBytes(frameCount + 1, 4));
            res.AddRange(new List<byte> { 0x15, 0x01, 0x00, 0x03, 0x00, 0x00 });//固定部分
            res.AddRange(IntToBytesWithReverse(totalCountStr, 2));//总段数
            res.AddRange(IntToBytesWithReverse(frameCount, 4));//第i段标识
            res.AddRange(IntToBytesWithReverse(132, 2));
            //自定义数据地址
            res.AddRange(IntToBytesWithReverse(data.Item1 * 128, 4));
            res.AddRange(data.Item2);
            var k = res.Skip(3).Take(res.Count() - 3).ToArray();
            res.Add(CheckProcess.FrameCheck(k.ToArray()));
            res.Add(0x16);
            
            return res.ToArray();
        }

        static byte[] IntToBytes(int value, int size)
        {
            string hexString = value.ToString("X" + size * 2);
            var t1 = DataConvert.strToToHexByte(hexString);
            Stack<byte> tempStack = new Stack<byte>();
            t1.ToList().ForEach(x => tempStack.Push(x));
            List<byte> bytes = new List<byte>();
            for (int i = 0; i < t1.Length; i++)
                bytes.Add(tempStack.Pop());

            return bytes.ToArray();

        }


        static byte[] IntToBytesWithReverse(int value, int size)
        {
            string hexString = value.ToString("X" + size * 2);
            var t1 = DataConvert.strToToHexByte(hexString);
            Stack<byte> tempStack = new Stack<byte>();
            t1.ToList().ForEach(x => tempStack.Push(x));
            List<byte> bytes = new List<byte>();
            for (int i = 0; i < t1.Length; i++)
                bytes.Add(tempStack.Pop());

            return bytes.ToArray();
         
        }

    }
}
