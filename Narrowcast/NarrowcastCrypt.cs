using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;

/*
 EXPRESSION:
        VALUE.KEY.relaydns.com

 HELPER FUNCTIONS:
        SECOND_WINDOW := FLOOR(UNIX_TIMESTAMP / FREQUENCY)

 LOGIC:
        KEY := DNS_LABELIZE(BASE64(AES_ECB(SHA256(INCREMENT + SECOND_WINDOW() + FREQUENCY + CHANNEL), SHA256(CHANNEL))))
        VALUE := DNS_LABELIZE(BASE64(AES_ECB(32_BYTES_OF_DATA_MAX, SHA256(CHANNEL))))

 NOTES:
        since ECB is used for our mode (as predictability is required), we must not have predictable data.
 */

namespace Narrowcast
{
    class NarrowcastCrypt
    {
        private const CipherMode cipherMode = CipherMode.ECB;
        private const PaddingMode paddingMode = PaddingMode.Zeros;
        private byte[] key = null;

        public NarrowcastCrypt(byte[] key) {
            this.key = key;
        }

        public string Encrypt(byte[] enc) {
            try {
                RijndaelManaged rm = new RijndaelManaged();
                rm.Key = this.key;
                rm.Mode = NarrowcastCrypt.cipherMode;
                rm.Padding = NarrowcastCrypt.paddingMode;
                ICryptoTransform cTransform = rm.CreateEncryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(enc, 0, enc.Length);
                StringBuilder sb = new StringBuilder(Convert.ToBase64String(resultArray, 0, resultArray.Length));
                for (int i = 0; i < NarrowcastNative.BASE64_REMAP.Length; i += 2)
                    sb.Replace(NarrowcastNative.BASE64_REMAP[i], NarrowcastNative.BASE64_REMAP[i + 1]);
                if (sb.ToString().StartsWith("-"))
                    sb.Insert(0, NarrowcastNative.BASE64_LPAD);
                return (sb.ToString());
            }
            catch { return (String.Empty); }
        }

        public string Encrypt(string enc) {
            try {
                byte[] encByteArray = Encoding.ASCII.GetBytes(enc);
                return (this.Encrypt(encByteArray));
            }
            catch { return (String.Empty); }
        }

        public string Decrypt(string dec) {
            try {
                StringBuilder sb = new StringBuilder(dec);
                for (int i = 0; i < NarrowcastNative.BASE64_REMAP.Length; i += 2)
                    sb.Replace(NarrowcastNative.BASE64_REMAP[i + 1], NarrowcastNative.BASE64_REMAP[i]);
                if (sb.ToString().StartsWith(NarrowcastNative.BASE64_LPAD))
                    sb.Remove(0, NarrowcastNative.BASE64_LPAD.Length);
                byte[] toEncryptArray = Convert.FromBase64String(sb.ToString());
                RijndaelManaged rm = new RijndaelManaged();
                rm.Key = this.key;
                rm.Mode = NarrowcastCrypt.cipherMode;
                rm.Padding = NarrowcastCrypt.paddingMode;
                ICryptoTransform cTransform = rm.CreateDecryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
                return (Encoding.ASCII.GetString(resultArray));
            }
            catch { return (String.Empty); }
        }

        public static byte[] SHA256(string str) {
            SHA256Managed crypt = new SHA256Managed();
            System.Text.StringBuilder hash = new System.Text.StringBuilder();
            return (crypt.ComputeHash(Encoding.ASCII.GetBytes(str), 0, Encoding.ASCII.GetByteCount(str)));
        }

    }
}
