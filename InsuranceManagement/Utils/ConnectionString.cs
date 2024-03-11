using InsuranceCore.Interfaces;
using InsuranceInfrastructure.Data;
using InsuranceInfrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceManagement.Utils
{
    public static class ConnectionString
    {
        public static IServiceCollection GetConnectionString(this IServiceCollection services, IConfiguration configuration)
        {

            var connectionString1 = configuration.GetConnectionString("DefaultConnection");
            string[] splitValues = connectionString1.Split(';');
            string password = null;
            string id = null;
            foreach (string item in splitValues)
            {
                if (item.Contains("User Id="))
                {
                    id = item.Substring(item.IndexOf('=') + 1);
                    continue;

                }
                else if (item.Contains("Password="))
                {
                    password = item.Substring(item.IndexOf('=') + 1);
                    continue;
                }

                //string[] keyValue = item.Split('=');
                //if (/*keyValue.Length == 2 && */keyValue[0].Trim() == "Password")
                //{
                //    password = keyValue[1].Trim();
                //    continue;
                //}
                //if (/*keyValue.Length == 2 &&*/ keyValue[0].Trim() == "User Id")
                //{
                //    id = keyValue[1].Trim();
                //    continue;
                //}
            }
           

            var Decryptedpassword1 =  Decrypt(password, configuration);
            var DecryptedUserId1 =  Decrypt(id, configuration);
            var DecryptedUserId = "User Id=" + DecryptedUserId1;
            var Decryptedpassword = "Password=" + Decryptedpassword1;
            var DecryptedconnectionString = splitValues[0]+";" + splitValues[1] + ";" + splitValues[2] + ";" + DecryptedUserId + ";" + Decryptedpassword + ";" + splitValues[5];

          //  var connectionString = Decrypt(configuration.GetConnectionString("DefaultConnection"), configuration);

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(DecryptedconnectionString));
            return services;
        }
        private  static string Decrypt(string ciphertext, IConfiguration configuration)
        {
            try
            {


                /// new LogHelper().Info(string.Format("Decrypting Ciphertext"));
                using (Aes myAes = Aes.Create())
                {

                    string _secretKey = configuration["AppSettings:Key"];
                    string _iv = configuration["AppSettings:Iv"];

                    myAes.Key = Encoding.UTF8.GetBytes(_secretKey);
                     myAes.IV = Encoding.UTF8.GetBytes(_iv);
                    // myAes.IV = Convert.FromBase64String(_iv);

                    // Convert the ciphertext (hex string) back to byte array
                    //byte[] encryptedBytes = StringToByteArray(ciphertext);
                   // byte[] encryptedBytes = StringToByteArray(ciphertext);
                    byte[] encryptedBytes = Convert.FromBase64String(ciphertext);
                    // Decrypt the byte array to plaintext
                    string plaintext = DecryptBytesToString_Aes(encryptedBytes, myAes.Key, myAes.IV);

                    // new LogHelper().Info(string.Format("Decryption returned-{0}", plaintext));
                    return plaintext;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        private static  byte[] StringToByteArray(string hex)
        {
            int numberChars = hex.Length;
            byte[] bytes = new byte[numberChars / 2];
            for (int i = 0; i < numberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }
            return bytes;
        }
        private static string DecryptBytesToString_Aes(byte[] cipherText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");

            string plaintext = null;
            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor,
                        CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            return plaintext;
        }
    }
}
