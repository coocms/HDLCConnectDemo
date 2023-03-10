
using System.Collections.Concurrent;
using System.Data;
using System.IO.Ports;
using System.Text;
using HDLCDeviceConnectDemo;


string portName = "";
while (true)
{
    Console.WriteLine("串口列表:");
    string[] ports = SerialPort.GetPortNames();//获取电脑上可用串口号
    ports.ToList().ForEach(port => Console.WriteLine(port));
    Console.Write("输入串口名 回车刷新>>");
    var cmd = Console.ReadLine();
    if (cmd.Contains("COM"))
    {
        portName = cmd;
        break;
    }


}




#region 二进制文件加载

byte[] updateFileBytes;
while (true)
{
    try
    {
        Console.Write("请输入升级包路径>>");
        var filePath = Console.ReadLine();
        updateFileBytes = FileProcess.LoadBinaryFile(filePath);
        if (updateFileBytes.Length == 0)
        {
            Console.WriteLine("路径错误");
            continue;
        }
        break;
    }
    catch (Exception ex) 
    {
        Console.WriteLine(ex.Message);
        continue;
    }

    
}




int count = 0;
Dictionary<int, byte[]> toBeSendDatas = new Dictionary<int, byte[]>();

while (true)
{
    var res = updateFileBytes.Skip(count * 128).Take(128).ToArray();
    
    if(res.Length > 0)
        toBeSendDatas.Add(count++, res);
    if (res.Length < 128)
    {
        break;
    }
}
#region 测试跳过逻辑
//toBeSendDatas[100] = new byte[128];
//for (int i = 0; i < 128; i++)
//{
//    toBeSendDatas[100][i] = 255;
//}
#endregion

#region 全为FF 跳过逻辑
toBeSendDatas = toBeSendDatas.Where(o => o.Value.Count(x => x == byte.MaxValue) != 128).ToDictionary(o=>o.Key, o=>o.Value);
#endregion

#region 376.2协议处理
int totalCount = toBeSendDatas.Count;

List<(int, byte[])> frame376_2 = new List<(int, byte[])>();
int frameCount = 0;
toBeSendDatas.ToList().ForEach(x =>
{
    frame376_2.Add((x.Key, Protocal376_2.Protocal376_2Process((x.Key, x.Value),frameCount++, totalCount)));
});
#endregion 

#region HDLC 协议处理
//frame376_2 = HDLCProcess.ProtocalHDLCProcess(frame376_2);


#endregion

#endregion

#region 帧校验
//var r = CheckProcess.FrameCheck(DataConvert.strToToHexByte("41 08 00 00 00 00 0115 01 00 03 00 00 B9 01      00 00 00 00       80 00 C4 87 00 00 EC 60 20 00 00 00 00 00 03 04 05 07 03 00 00 00 EC B7 EF 00 33 00 44 00 55 00 66 00 11 00 FF FF EC E8 EF 00 EC 18 F0 00 EC 08 E9 00 00 00 00 00 00 00 00 00 00 00 AD 98 06 00 00 00 39 71 0B 00 14 04 00 00 FF FF FF FF EC 3E F2 00 EC DB F1 00 FF FF FF FF FF FF FF FF FF FF FF FF EC 57 F5 00 CB F8 20 FE 36 34 FD FC C5 1E 00 41 00 36 10 20 34 C4 F5 EF 05 11 8B 99 A7 A5 17 44"));
#endregion

#region CRC 校验DEMO

//var crcRes = CheckProcess.Crc20(DataConvert.strToToHexByte("A01B2DCD13"));
//var crcResStr = DataConvert.byteToHexStr(crcRes);
#endregion

#region 模块透传

// See https://aka.ms/new-console-template for more information

//485---------------------------------------------------------------
var _serialPort = new SerialPort();
_serialPort.PortName = portName;
_serialPort.BaudRate = 9600;
_serialPort.DataBits = 8;
_serialPort.StopBits = StopBits.One;
_serialPort.DtrEnable = true;
_serialPort.RtsEnable = true;

_serialPort.Parity = Parity.Even;
//_serialPort.Handshake = Handshake.None;
_serialPort.Encoding = System.Text.Encoding.UTF8;



using SerialProcess serialProcess = new SerialProcess(_serialPort);
serialProcess.Start();

Console.WriteLine($"开始升级 总包数:{frame376_2.Count}");
for (int i = 0; i < frame376_2.Count; i++)
{
    Console.WriteLine($"开始发送第{i}包");
    serialProcess.WriteData(frame376_2[i].Item2);
    var bError = false;
    try
    {
        while (true)
        {
            //0xFF, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01, 0x16, 0x17 读版本错误的报文
            var response = serialProcess.ReceivedData();
            var resBytes = DataConvert.strToToHexByte(response);
            if (resBytes.Count() < 14)
            {
                Console.WriteLine("数据长度异常，尝试重新接收");
                continue;
            }
                
            switch (resBytes[13])
            {
                case 2: 
                    Console.WriteLine("flash写错误");
                    bError = true;
                    break;
                case 3:
                    Console.WriteLine("升级版本号错误");
                    bError = true;
                    break;
                case 5:
                    Console.WriteLine("长度错误");
                    bError = true;
                    break;
                case 6:
                    Console.WriteLine("flash擦除失败");
                    bError = true;
                    break;
                case 7:
                    Console.WriteLine("数据包非法");
                    bError = true;
                    break;
                case 9:
                    Console.WriteLine("不支持的命令");
                    bError = true;
                    break;
                case 19:
                    Console.WriteLine("外部flash错误");
                    bError = true;
                    break;
                default:
                    break;
            }
            if (bError)
                break;
        }
        if (bError)
        {
            Console.WriteLine("发生错误 升级中断");
            break;
        }
    }
    catch (Exception ex)
    {

    }
    
}
Console.WriteLine("升级结束");
Console.ReadLine();
return;

//485----------------------------------------------------------------------------------------------------
serialProcess.WriteData("7EA00703CD9336417E");
serialProcess.ReceivedData();
serialProcess.WriteData("7EA01903CD13DDA8E6E600C001C000010000600100FF020018F57E");
serialProcess.ReceivedData();
serialProcess.WriteData("7EA01903CD13DDA8E6E600C001C000010000600100FF020018F57E");
serialProcess.ReceivedData();
//serialProcess.WriteData("7E A0 1E 00 20 0401 CD  13 E0DD E6 E6 00 FF 06 9F 50 00 00 FE AA AA AA  AA AA AA 00 EF E3E5 7E");

serialProcess.WriteData("7EA01E0020FEE1CD13C47FE6E600 FF069F500000FEAAAAAAAAAAAA00EF E3E57E");
//7E A0 1E 00 20 FE E1 CD  13 C4 7F E6 E6 00 FF 06 9F 50 00 00 FE AA AA AA  AA AA AA 00 EF E3 E5 7E
var version = serialProcess.ReceivedData();
var versionFrameBytes = DataConvert.strToToHexByte(version);
var lenth = versionFrameBytes.Skip(23).ToArray().Take(1).First();
var versionBytes = versionFrameBytes.Skip(24).Take(lenth).ToArray();


string str = Encoding.Default.GetString(versionBytes);
Console.WriteLine("Now Version = " + str);
//485----------------------------------------------------------------------------------------------------

//serialProcess.ReceivedData();


#endregion






Console.ReadLine();


