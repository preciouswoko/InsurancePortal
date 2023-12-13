using InsuranceCore.Interfaces;
using InsuranceCore.Models;
using InsuranceInfrastructure.Repositories;
using InsuranceInfrastructure.Util;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.Extensions.Options;
using SelectPdf;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
namespace InsuranceInfrastructure.Services
{
    public class UtilityService : IUtilityService
    {
        private AppSettings _appsettings;
        private readonly string _secretKey;
        private readonly string _iv;
        private readonly ILoggingService _logging;
        private readonly IHttpContextAccessor _hcontext;
        private string menu_string;
        private char sp = '"';
        public readonly IGenericRepository<AdminRoles> _adminRepo;
        private readonly IGenericRepository<EncryptionData> _encryptiondata;
        private readonly IHostingEnvironment _hostingEnvironment;

        public UtilityService(IOptions<AppSettings> ioptions, IHostingEnvironment hostingEnvironment, IGenericRepository<EncryptionData> encryptiondata, ILoggingService logging, IHttpContextAccessor hcontext, IGenericRepository<AdminRoles> adminRepo)
        {
            _appsettings = ioptions.Value;
            _secretKey = _appsettings.Key;
            _iv = _appsettings.Iv;
            _hcontext = hcontext;
            _adminRepo = adminRepo;
            _encryptiondata = encryptiondata;
            _logging = logging;
            _hostingEnvironment = hostingEnvironment;

        }
        public string Encrypt(string plaintext, CancellationToken cancellation)
        {
            try
            {
                // var keys = AesEncryptionRepository.ReadUserKey(username);
                //var getencryption = _encryptiondata.GetById(1);


                /// new LogHelper().Info(string.Format("Encrypting Plain Text"));
                using (Aes myAes = Aes.Create())
                {
                    //myAes.Key = Encoding.UTF8.GetBytes(getencryption.Key);
                    //myAes.IV = Encoding.UTF8.GetBytes(getencryption.Iv);

                    myAes.Key = Encoding.UTF8.GetBytes(_secretKey);
                    myAes.IV = Encoding.UTF8.GetBytes(_iv);

                    // Encrypt the string to an array of bytes.
                    byte[] encrypted = EncryptStringToBytes_Aes(plaintext, myAes.Key, myAes.IV);
                    string ciphertext = ByteArrayToString(encrypted);
                    //  new LogHelper().Info(string.Format("Encryption returned-{0}", ciphertext));
                    return ciphertext;
                }
            }
            catch (Exception ex)
            {
                _logging.LogError(ex.ToString(), "Encrypt");
                throw ex;
            }
        }
        private static byte[] EncryptStringToBytes_Aes(string plainText, byte[] Key, byte[] IV)
        {
            // Check arguments.
            if (plainText == null || plainText.Length <= 0)
                throw new ArgumentNullException("plainText");
            if (Key == null || Key.Length <= 0)
                throw new ArgumentNullException("Key");
            if (IV == null || IV.Length <= 0)
                throw new ArgumentNullException("IV");
            byte[] encrypted;
            // Create an Aes object
            // with the specified key and IV.
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key;
                aesAlg.IV = IV;
                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);
                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor,
                    CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(plainText);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }
        private static byte[] HexadecimalStringToByteArray(string input)
        {
            var outputLength = input.Length / 2;
            var output = new byte[outputLength];
            using (var sr = new StringReader(input))
            {
                for (var i = 0; i < outputLength; i++)
                    output[i] = Convert.ToByte(new string(new char[2] { (char)sr.Read(), (char)sr.Read() }), 16);
            }
            return output;
        }
        public string ByteArrayToString(byte[] ba)
        {
            StringBuilder hex = new StringBuilder(ba.Length * 2);
            foreach (byte b in ba)
                hex.AppendFormat("{0:x2}", b);
            return hex.ToString();
        }
        public decimal GetDebitValue(string Ofs)
        {

            string[] keyValuePairs = Ofs.Split(',');

            decimal debitAmount = 0;

            foreach (string pair in keyValuePairs)
            {
                if (pair.Contains("DEBIT.AMOUNT::="))
                {
                    debitAmount = Convert.ToDecimal(pair.Replace("DEBIT.AMOUNT::=", ""));
                    break;
                }
            }


            return debitAmount;

        }

        public async Task<string> GenerateFilePath(IFormFile uploadedFile)
        {
          
            try
            {
                if (uploadedFile == null || uploadedFile.Length <= 0)
                {
                    // Handle the case where the uploaded file is empty or null
                    return null;
                }

                // Ensure a unique filename to prevent overwriting
                var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(uploadedFile.FileName)}";

                // Combine the unique file name with the configured uploads folder to create the full file path
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

                // Ensure the uploads directory exists; if not, create it
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var fullFilePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(fullFilePath, FileMode.Create))
                {
                    await uploadedFile.CopyToAsync(stream);
                }

                // Return the full file path
                return fullFilePath;
            }
            catch (Exception ex)
            {
                _logging.LogError(ex.ToString(), "GenerateFilePath");
                throw;
            }
        }
        public IFormFile ConvertBase64ToIFormFile(string base64String, string contentType, string fileName)
        {
            if (string.IsNullOrEmpty(base64String))
            {
                return null; // Handle empty base64 string
            }

            byte[] bytes = Convert.FromBase64String(base64String);
            var stream = new MemoryStream(bytes);

            var file = new FormFile(stream, 0, bytes.Length, "file", fileName)
            {
                Headers = new HeaderDictionary(),
                ContentType = contentType
            };

            return file;
        }

        public string ConvertIFormFileToBase64(IFormFile uploadedFile)
        {
            if (uploadedFile == null || uploadedFile.Length == 0)
            {
                return null; // Handle empty or null file
            }

            using (var memoryStream = new MemoryStream())
            {
                uploadedFile.CopyTo(memoryStream);
                byte[] bytes = memoryStream.ToArray();
                return Convert.ToBase64String(bytes);
            }
        }

        //public bool DeleteFile(string fileName)
        //{
        //    try
        //    {
        //        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

        //        // Get a list of all files in the folder
        //        var files = Directory.GetFiles(uploadsFolder);

        //        // Search for the file with the provided filename (excluding the unique GUID prefix)
        //        var targetFile = files.FirstOrDefault(file => Path.GetFileName(file) == fileName);


        //        if (targetFile != null)
        //        {
        //            File.Delete(targetFile);
        //            return true;
        //        }

        //        return false;

        //    }
        //    catch (Exception ex)
        //    {
        //        _logging.LogError(ex.ToString(), "DeleteFile");
        //        throw;
        //    }
        //}
        public bool DeleteFile(string fileName)
        {
            try
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads");

                var matchingFiles = Directory.GetFiles(uploadsFolder, $"*_{fileName}");

                foreach (var file in matchingFiles)
                {
                    File.Delete(file);
                }

                return matchingFiles.Length > 0; 

            }
            catch (Exception ex)
            {
                _logging.LogError(ex.ToString(), "DeleteFile");
                throw;
            }
        }


        public decimal CalculateCommission(decimal premiumAmount , int percentage)
        {
            // Calculation logic
            var commission = premiumAmount * (percentage / 100);
            return commission;
        }

        public string GetContentType(string filePath)
        {
            // Map file extensions to MIME types (you can add more as needed)
            var contentTypes = new Dictionary<string, string>{
                { ".pdf", "application/pdf" },
                { ".jpg", "image/jpeg" },
                { ".png", "image/png" },
                // Add more mappings as needed
            };

            // Get the file's extension
            string extension = Path.GetExtension(filePath).ToLowerInvariant();

            // Try to find the content type for the extension
            if (contentTypes.TryGetValue(extension, out string contentType))
            {
                return contentType;
            }

            // If the extension is not recognized, return a generic binary content type
            return "application/octet-stream";
        }

        public string GenerateSHA512String(string inputString)
        {
            SHA512 sha512 = SHA512Managed.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(inputString);
            byte[] hash = sha512.ComputeHash(bytes);
            return GetStringFromHash(hash);
        }


        public string GenerateSHA256String(string inputString)
        {
            SHA256 sha256 = SHA256Managed.Create();
            byte[] bytes = Encoding.UTF8.GetBytes(inputString);
            byte[] hash = sha256.ComputeHash(bytes);
            return GetStringFromHash(hash);
        }


        private static string GetStringFromHash(byte[] hash)
        {
            StringBuilder result = new StringBuilder();

            for (int i = 0; i < hash.Length; i++)
            {
                result.Append(hash[i].ToString("X2"));
            }
            return result.ToString();
        }

        public  bool IsValidJson(string json)
        {
            if (string.IsNullOrEmpty(json)) return false;
            return json.Substring(0, 1) == "{" || json.Substring(0, 1) == "[";
        }
        public  byte[] ConvertToPdf(string mailMessage)
        {
            var _htmltoPdf = new HtmlToPdf();
            _htmltoPdf.Options.ExternalLinksEnabled = true;
            _htmltoPdf.Options.CssMediaType = HtmlToPdfCssMediaType.Print;
            _htmltoPdf.Options.DisplayCutText = true;
            _htmltoPdf.Options.InternalLinksEnabled = true;
            _htmltoPdf.Options.JavaScriptEnabled = true;
            _htmltoPdf.Options.PageBreaksEnhancedAlgorithm = true;
            _htmltoPdf.Options.EmbedFonts = true;
            _htmltoPdf.Options.KeepImagesTogether = true;
            _htmltoPdf.Options.PdfCompressionLevel = PdfCompressionLevel.NoCompression;
            _htmltoPdf.Options.MaxPageLoadTime = 60 * 60 * 5;
            _htmltoPdf.Options.MarginTop = 50;
            _htmltoPdf.Options.MarginBottom = 50;
            var document = _htmltoPdf.ConvertHtmlString(mailMessage);
            using (MemoryStream stream = new MemoryStream())
            {
                document.Save(stream);
                return stream.ToArray();
            };

        }
        public  string GetIpAddress(Microsoft.AspNetCore.Http.HttpRequest request)
        {
            return request.HttpContext.Connection.RemoteIpAddress.ToString();
        }
        public string FindBrowser(string BrowserText)
        {
            if (BrowserText.ToLower().IndexOf("edge") > -1) return "edge";
            if (BrowserText.ToLower().IndexOf("edg") > -1) return "edge dev/can";
            if (BrowserText.ToLower().IndexOf("opr") > -1) return "opera";
            if (BrowserText.ToLower().IndexOf("chrome") > -1) return "chrome";
            if (BrowserText.ToLower().IndexOf("trident") > -1) return "trident";
            if (BrowserText.ToLower().IndexOf("firefox") > -1) return "firefox";
            if (BrowserText.ToLower().IndexOf("safari") > -1) return "safari";
            return "other";
        }
        //public async Task<string> GeneratedMenuHtml(int RoleId)
        //{
        //    var request = _hcontext.HttpContext.Request;

        //    //var role = _context.AdminRoles.Include(r => r.Roles).Where(x => x.Id == RoleId && !x.IsDeleted).FirstOrDefault();
        //    //if (role == null) return "0";
        //    var role = await _adminRepo.GetWithIncludeAsync(x => x.Id == RoleId && !x.IsDeleted, r => r.Roles);

        //    if (role == null) return "0";

        //    var mainMenu = MenuItemRepository.Menus().Where(x => role.Roles.Any(y => y.MenuOption == x.Code));
        //    if (RoleId == 9999)
        //    {
        //        mainMenu = MenuItemRepository.Menus();
        //    }

        //    var menu_string = ""; // Initialize an empty string to store the menu HTML

        //    var MainMenuFiltered = mainMenu.OrderBy(t => t.Sequence).Select(x => x.Group).Distinct();

        //    foreach (var item in MainMenuFiltered)
        //    {
        //        menu_string += "<li class=\"active open\">\r\n";
        //        menu_string += "<a href=\"javascript:void(0);\" class=\"menu-toggle\"><i class=\"zmdi zmdi-apps\"></i><span>" + item + "</span></a>\r\n";
        //        menu_string += "<ul class=\"ml-menu\">\r\n";

        //        var _items = mainMenu.Where(x => x.Group == item).OrderBy(x => x.Sequence);
        //        foreach (var x in _items)
        //        {
        //            menu_string += "<li><a href=\"/Menu/ItemRun/" + x.Code.Trim() + "[]\">" + x.Description + "</a> </li>\r\n";
        //        }
        //        menu_string += "</ul>\r\n";
        //        menu_string += "</li>\r\n";
        //    }

        //    return menu_string;
        //}
        public  string ComputeSHA256Hash(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(input);
                byte[] hashBytes = sha256.ComputeHash(bytes);

                // Convert the byte array to a hexadecimal string
                StringBuilder stringBuilder = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    stringBuilder.Append(hashBytes[i].ToString("x2"));
                }

                return stringBuilder.ToString();
            }
        }
        public async Task<string> GeneratedMenuHtml(int RoleId, List<string> permissions)
        {
            var request = _hcontext.HttpContext.Request;

            var mainMenu = RoleId == 9999 ? MenuItemRepository.Menus() : MenuItemRepository.Menus();

            // Call MapMenuItemToPermission to map permissions to menu item codes
            var menuCodes = MapMenuItemToPermission(permissions);

            var MainMenuFiltered = mainMenu.OrderBy(t => t.Sequence).Select(x => x.Group).Distinct();

            foreach (var item in MainMenuFiltered)
            {
                var _items = mainMenu.Where(x => x.Group == item).OrderBy(x => x.Sequence);
                var hasSubItems = false;

                foreach (var x in _items)
                {
                    // Check if the menu code is in menuCodes (permissions)
                    if (menuCodes.Contains(x.Code))
                    {
                        if (!hasSubItems)
                        {
                            menu_string += $"<li class=\"active open\">\r\n";
                            menu_string += $"<a href=\"javascript:void(0);\" class=\"menu-toggle\"><i class=\"zmdi zmdi-apps\"></i><span style=\"color: black;\">{item}</span></a>\r\n";
                            menu_string += "<ul class=\"ml-menu\">\r\n";
                            hasSubItems = true;
                        }

                        menu_string += $"<li><a href=\"/Menu/ItemRun/{x.Code.Trim()}[]\" style=\"color: black;\">{x.Description}</a></li>\r\n";
                    }
                }

                if (hasSubItems)
                {
                    menu_string += "</ul>\r\n";
                    menu_string += "</li>\r\n";
                }
            }

            return menu_string;
        }


        private List<string> MapMenuItemToPermission(List<string> permissions)
        {
            var codeMap = new Dictionary<string, string>
            {
                { "INR", "A017" },
                { "AUR", "A05" },
                { "ANU", "A06" },
                { "UIC", "A08" },
                { "RIC", "A07" },
                { "ACI", "A09" },
                { "GAI", "A03" },
                { "LOB", "A012" },
                { "LOU", "A011" },
                { "LOI", "A013" },
                { "LIS", "A014" },
                 { "LBI", "A025" },
                { "LBS", "A026" },
                 { "GAR", "A027" },
            };

            var codes = new List<string>();

            foreach (var code in permissions)
            {
                if (codeMap.ContainsKey(code))
                {
                    codes.Add(codeMap[code]);
                }
            }

            return codes;
        }

        public string Decrypt(string ciphertext, CancellationToken cancellation)
        {
            try
            {
                // var keys = AesEncryptionRepository.ReadUserKey(username);
                var getencryption = _encryptiondata.GetById(1);

                /// new LogHelper().Info(string.Format("Decrypting Ciphertext"));
                using (Aes myAes = Aes.Create())
                {
                    myAes.Key = Encoding.UTF8.GetBytes(getencryption.Key);
                    myAes.IV = Encoding.UTF8.GetBytes(getencryption.Iv);

                    //myAes.Key = Encoding.UTF8.GetBytes(_secretKey);
                    //myAes.IV = Encoding.UTF8.GetBytes(_iv);

                    // Convert the ciphertext (hex string) back to byte array
                    byte[] encryptedBytes = StringToByteArray(ciphertext);

                    // Decrypt the byte array to plaintext
                    string plaintext = DecryptBytesToString_Aes(encryptedBytes, myAes.Key, myAes.IV);

                    // new LogHelper().Info(string.Format("Decryption returned-{0}", plaintext));
                    return plaintext;
                }
            }
            catch (Exception ex)
            {
                _logging.LogError(ex.ToString(), "Decrypt");
                throw ex;
            }
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

        public byte[] StringToByteArray(string hex)
        {
            int numberChars = hex.Length;
            byte[] bytes = new byte[numberChars / 2];
            for (int i = 0; i < numberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }
            return bytes;
        }
        public string BuildEmailTemplate(string name, string title, string msg)
        {

            string body = string.Empty;
            try
            {
                string webRootPath = _hostingEnvironment.WebRootPath;
                string filePath = Path.Combine(webRootPath, "Resource/Template/Email/emailTemplate.html");

                if (File.Exists(filePath))
                {
                    string fileContent = System.IO.File.ReadAllText(filePath);

                    fileContent = fileContent.Replace("{msg}", msg);
                    fileContent = fileContent.Replace("{title}", title);
                    fileContent = fileContent.Replace("{name}", name);
                    return fileContent;

                }

                //var appDomain = System.AppDomain.CurrentDomain;
                //var basePath = appDomain.BaseDirectory;
                //string path = Path.Combine(basePath, "Resource\\Template\\Email", "emailTemplate.html");

                //StringBuilder sb = new StringBuilder();
                //StreamReader sr = new StreamReader(path);
                //string line = sr.ReadToEnd();
                //sb.Append(line);
                //sb.Replace("{msg}", msg);
                //sb.Replace("{title}", title);
                //sb.Replace("{name}", name);
                
                
                //body = sb.ToString();
            }
            catch (Exception ex)
            {
                _logging.LogInformation(ex.ToString(), "BuildOtpTempEmail");
            }
            return body;
        }
        public string ConvertDateToString(string date)
        {
            try
            {
                if (DateTime.TryParseExact(date, "MM/dd/yyyy HH:mm:ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime parsedDateTime))
                {
                    string formattedDateTime = parsedDateTime.ToString("MMM dd, yyyy HH:mm:ss");
                    return formattedDateTime;
                }
                else
                {
                    return "Invalid date and time format";
                }
            }
            catch (Exception ex)
            {
                _logging.LogInformation(ex.Message.ToString(), "ConvertDateToString");
                return "Error occurred while converting date to string";
            }
        }
    }
}
