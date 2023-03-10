using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace HDLCDeviceConnectDemo
{
    public class FileProcess
    {
        public static byte[] LoadBinaryFile(string Fliepath)
        {
            int length = 0;
            
            try
            {
                FileStream fs = File.Open(Fliepath, FileMode.Open, FileAccess.Read, FileShare.None);
                length = (int)fs.Length;
                byte[] readbuf = new byte[length];
                fs.Seek(0, SeekOrigin.Begin);
                fs.Read(readbuf, 0, length);

                fs.Close();
                return readbuf;
            }
            catch (System.Exception ex)
            {
                return new byte[0];   
            }
        }
    }
}
