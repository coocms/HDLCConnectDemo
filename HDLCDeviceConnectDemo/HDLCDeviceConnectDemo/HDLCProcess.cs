using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HDLCDeviceConnectDemo
{
    public class HDLCProcess
    {
        public static List<(int, byte[])> ProtocalHDLCProcess(List<(int, byte[])> datas)
        {
            for(int i = 0; i < datas.Count; i++)
            {
                var data = datas[i];
                List<byte> item = new List<byte>();
                item.AddRange(new List<byte> {0x7E, 0xA0, 0x1E, 0x00, 0x20, 0xFE, 0xE1, 0xCD, 0x13, 0xC4, 0x7F, 0xE6, 0xE6, 0x00 });
                item.AddRange(data.Item2);
                var crc = CheckProcess.Crc20(item.Skip(1).Take(item.Count - 1).ToArray());
                item.Add(crc[1]);
                item.Add(crc[0]);
                item.Add(0x7E);
                
                datas[i] = (data.Item1, item.ToArray());

            }
            datas.ForEach(data =>
            {
                Console.WriteLine(DataConvert.byteToHexStr(data.Item2));
            });


            return datas;
        }
        

    }
}
