namespace HDLCDeviceConnectDemo
{
    public class DataConvert
    {

        public static string RemoveZero(string dValue)
        {
            string sResult = dValue.ToString();

            int iIndex = sResult.Length - 1;
            for (int i = sResult.Length - 1; i >= 0; i--)
            {
                if (sResult.Substring(i, 1) != "0")
                {
                    iIndex = i;
                    break;
                }
            }
            sResult = sResult.Substring(0, iIndex + 1);
            if (sResult.EndsWith("."))
                sResult = sResult.Substring(0, sResult.Length - 1);
            return sResult;
        }

        public static byte[] strToToHexByte(string hexString)
        {
            hexString = hexString.Replace(" ", "");
            hexString = hexString.Replace(" ", "");


            if (hexString.Length % 2 != 0)
                hexString += " ";
            byte[] returnBytes = new byte[hexString.Length / 2];
            for (int i = 0; i < returnBytes.Length; i++)
            {

                returnBytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2), 16);

            }

            return returnBytes;
        }
        public static string byteToHexStr(byte[] bytes)
        {
            string returnStr = "";
            if (bytes != null)
            {
                for (int i = 0; i < bytes.Length; i++)
                {
                    returnStr += bytes[i].ToString("X2");
                }
            }
            return returnStr;
        }
    }
}
