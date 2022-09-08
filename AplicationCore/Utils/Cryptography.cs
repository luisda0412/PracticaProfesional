using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Utils
{
    class Cryptography
    {
        const string KEY = "~!@#$%^&*()_+{}:>?<`1234567890-=[]\'.,/|";

        /// <summary>
        /// http://www.aspsnippets.com/Articles/AES-Encryption-Decryption-Cryptography-Tutorial-with-example-in-ASPNet-using-C-and-VBNet.aspx
        /// </summary>
        /// <param name="pCadena"></param>
        /// <returns></returns>
        static public string EncrypthAES(string pCadena)
        {
            byte[] clearBytes = Encoding.Unicode.GetBytes(pCadena);

            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(KEY, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(clearBytes, 0, clearBytes.Length);
                        cs.Close();
                    }
                    pCadena = Convert.ToBase64String(ms.ToArray());
                }
            }
            return pCadena;
        }

        static public string DecrypthAES(string pCadena)
        {

            byte[] cipherBytes = Convert.FromBase64String(pCadena);
            using (Aes encryptor = Aes.Create())
            {
                Rfc2898DeriveBytes pdb = new Rfc2898DeriveBytes(KEY, new byte[] { 0x49, 0x76, 0x61, 0x6e, 0x20, 0x4d, 0x65, 0x64, 0x76, 0x65, 0x64, 0x65, 0x76 });
                encryptor.Key = pdb.GetBytes(32);
                encryptor.IV = pdb.GetBytes(16);
                using (MemoryStream ms = new MemoryStream())
                {
                    using (CryptoStream cs = new CryptoStream(ms, encryptor.CreateDecryptor(), CryptoStreamMode.Write))
                    {
                        cs.Write(cipherBytes, 0, cipherBytes.Length);
                        cs.Close();
                    }
                    pCadena = Encoding.Unicode.GetString(ms.ToArray());
                }
            }
            return pCadena;
        }


    }
}

