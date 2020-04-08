using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DRMAPI.Data
{
    public static class DataUtils
    {

        public static byte[] HexStringToByteArray(string hexString)
        {
            if (hexString.Length % 2 != 0)
            {
                throw new ArgumentException(
                    String.Format(CultureInfo.InvariantCulture, 
                    "The binary key cannot have an odd number of digits: {0}",
                    hexString));
            }

            // Have to do some weird offsetting because of the leading escape character
            byte[] data = new byte[(hexString.Length / 2) - 1];
            for (int index = 0; index < data.Length; index++)
            {
                string byteValue = hexString.Substring((index + 1) * 2, 2);
                data[index] = byte.Parse(byteValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture);
            }

            return data;
        }
    }
}
