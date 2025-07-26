using System;

namespace Backend.Helpers
{
    public class SimpleEncryptionHelper
    {
        public string MapingCharactersForEncryption(string strChar)
        {
            if (string.IsNullOrEmpty(strChar)) return string.Empty;

            char ch = strChar[0];
            int ascii = (int)ch;

            if (ascii >= 65 && ascii <= 90)
                return ((char)(90 - (ascii - 65))).ToString();
            else if (ascii >= 53 && ascii <= 57)
                return ((char)(57 - (ascii - 48))).ToString();
            else if (ascii >= 48 && ascii < 53)
                return ((char)(57 - ascii + 48)).ToString();
            else if (ascii >= 97 && ascii <= 122)
                return ((char)(122 - (ascii - 97))).ToString();
            else
                return strChar;
        }

        public string MapingStringForEncryption(string strEncryption)
        {
            string result = string.Empty;
            for (int i = 0; i < strEncryption.Length; i++)
                result += MapingCharactersForEncryption(strEncryption.Substring(i, 1));
            return result;
        }
    }
}
