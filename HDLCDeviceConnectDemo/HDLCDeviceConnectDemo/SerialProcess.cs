using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HDLCDeviceConnectDemo
{
    public class SerialProcess:IDisposable
    {
        SerialPort _serialPort;
        ConcurrentQueue<string> receivedData = new ConcurrentQueue<string>();
        public SerialProcess(SerialPort serialPort) 
        {
            _serialPort = serialPort;
            //Thread thread = new Thread(receivedThread);
            //thread.IsBackground = true;
            //thread.Start();


        }
        public string ReceivedData() 
        {
            while (true)
            {
                try 
                {
                    if (receivedData.TryDequeue(out string result))
                    {
                        Console.WriteLine("Receive : " + result);
                        return result;   

                    }
                        
                    Thread.Sleep(500);

                } 
                catch 
                { 

                }
            }
        }

        public void Dispose()
        {
            _serialPort.Close();
            _serialPort.Dispose();
        }

        public bool Start()
        {
            try
            {
                if (_serialPort == null) return false;


                _serialPort.Open();
                _serialPort.DataReceived += (s, o) => DataReceived(s, o);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("SerialProcess Error : " + ex.ToString());
                return false;
            }
            
            
        }
        public bool WriteData(string datas) 
        {
            try
            {
                var sendBytes = DataConvert.strToToHexByte(datas);
                _serialPort.Write(sendBytes, 0, sendBytes.Length);
                Console.WriteLine("Send :    " + datas);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("WriteData Error : " + ex.Message);
                return false;
            }
        }
        public bool WriteData(byte[] sendBytes)
        {
            try
            {
                
                _serialPort.Write(sendBytes, 0, sendBytes.Length);
                Console.WriteLine("Send :    " + DataConvert.byteToHexStr(sendBytes));
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine("WriteData Error : " + ex.Message);
                return false;
            }
        }
        void DataReceived(object sender, SerialDataReceivedEventArgs args)
        {
            try
            {
                Thread.Sleep(200);
                string res = "";
                while (true)
                {
                    var count = _serialPort.BytesToRead;
                    if (count == 0)
                        break;

                    byte[] bytes = new byte[count];
                    _serialPort.Read(bytes, 0, count);
                    res += DataConvert.byteToHexStr(bytes.ToArray());
                    
                }
                receivedData.Enqueue(res);


            }
            catch (Exception)
            {


            }

        }

    }
}
