using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace DistSysACW.CoreExtensions
{
    public class RSAInternal
    {

        static string str;

        //Encryption
        static public byte[] RSAEncrypt(byte[] content, RSAParameters publickey)
        {

            byte[] encryptedBytes;
            try
            {
                using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
                {
                    rsa.PersistKeyInCsp = true;
                    rsa.ImportParameters(publickey);
                    encryptedBytes = rsa.Encrypt(content, true);

                }
                return encryptedBytes;
            }
            catch (CryptographicException e)
            {
                return null;
            }

        }

        //Decryption
        public static byte[] RSADecrypt(byte[] ciphertext, RSAParameters rsaParams)
        {
            try
            {
                byte[] decryptData;
                using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
                {
                    RSA.ImportParameters(rsaParams);
                    decryptData = RSA.Decrypt(ciphertext, true);
                }
                return decryptData;
            }
            catch (CryptographicException e)
            {
                return null;
            }
        }

        public static byte[] HashAndSignBytes(byte[] DataToSign, RSAParameters Key)
        {
            try
            {
                RSACryptoServiceProvider RSAalg = new RSACryptoServiceProvider();
                RSAalg.ImportParameters(Key);
                return RSAalg.SignData(DataToSign, new SHA1CryptoServiceProvider());
            }
            catch (CryptographicException e)
            {
                return null;
            }
        }

        public static bool VerifySignedHash(byte[] DataToVerify, byte[] SignedData, RSAParameters Key)
        {
            try
            {
                RSACryptoServiceProvider RSAalg = new RSACryptoServiceProvider();
                RSAalg.ImportParameters(Key);
                return RSAalg.VerifyData(DataToVerify, new SHA1CryptoServiceProvider(), SignedData);
            }
            catch (CryptographicException e)
            {
                return false;
            }
        }

    }
}
