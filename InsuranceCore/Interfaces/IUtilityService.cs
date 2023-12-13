using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace InsuranceCore.Interfaces
{
    public interface IUtilityService
    {
        string BuildEmailTemplate(string name, string title, string msg);
        string Encrypt(string plaintext, CancellationToken cancellation);
        string Decrypt(string ciphertext, CancellationToken cancellation);
        string GenerateSHA512String(string inputString);
        string GenerateSHA256String(string inputString);
        bool IsValidJson(string json);
        byte[] ConvertToPdf(string mailMessage);
        string GetIpAddress(Microsoft.AspNetCore.Http.HttpRequest request);
        Task<string> GenerateFilePath(IFormFile uploadedFile);
        string GetContentType(string filePath);
        decimal CalculateCommission(decimal premiumAmount, int percentage);
        string FindBrowser(string BrowserText);
        Task<string> GeneratedMenuHtml(int RoleId, List<string> permissions);
        string ComputeSHA256Hash(string input);
        bool DeleteFile(string fileName);
        string ConvertDateToString(string date);
        IFormFile ConvertBase64ToIFormFile(string base64String, string contentType, string fileName);
        string ConvertIFormFileToBase64(IFormFile uploadedFile);
        decimal GetDebitValue(string Ofs);
    }
}
