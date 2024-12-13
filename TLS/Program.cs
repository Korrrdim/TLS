using System;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;

class Program
{
    static void Main(string[] args)
    {
        string url = "https://elearning.kubg.edu.ua";

        try
        {
            Uri uri = new Uri(url);
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);
            
            request.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(ValidateServerCertificate);
            
            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            {
                Console.WriteLine($"З'єднання з сервером {url} встановлено успішно.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Помилка під час з'єднання: {ex.Message}");
        }
    }
    
    private static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
    {
        if (sslPolicyErrors == SslPolicyErrors.None)
        {
            Console.WriteLine("Сертифікат валідний.");
            Console.WriteLine($"Тема сертифіката: {certificate.Subject}");
            Console.WriteLine($"Видавець сертифіката: {certificate.Issuer}");
            Console.WriteLine($"Термін дії сертифіката: {certificate.GetExpirationDateString()}");
            
            X509Certificate2 cert = new X509Certificate2(certificate);
            RSA rsa = cert.GetRSAPublicKey();
            if (rsa != null)
            {
                byte[] publicKeyBytes = rsa.ExportRSAPublicKey();
                using (SHA256 sha256 = SHA256.Create())
                {
                    byte[] hash = sha256.ComputeHash(publicKeyBytes);
                    Console.WriteLine("Хеш публічного ключа сертифіката:");
                    Console.WriteLine(BitConverter.ToString(hash).Replace("-", "").ToLower());
                }
            }
            else
            {
                Console.WriteLine("Не вдалося отримати публічний ключ.");
            }

            return true;
        }
        else
        {
            Console.WriteLine($"Помилка SSL: {sslPolicyErrors}");
            return false;
        }
    }
}
