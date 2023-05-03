using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace iEdisonCore
{
    class Program
    {
        static void Main(string[] args)
        {
            string str = TestSettings(args);
            Console.WriteLine(str);
            Console.WriteLine("");
            Console.WriteLine("Press any key to continue");
            Console.ReadKey(true);
        }

        static string INVENTION_SEARCH = "/iedison/api/v1/inventions/search";
        public static string TestSettings(string[] args)
        {
            string certfileandpath = @"C:\temp\certificate.pfx";
            if (args.Length > 0)
                certfileandpath = args[0];
            string passcode = "";
            if (args.Length > 1)
                passcode = args[1];
            string baseurl = "https://api-iedison.nist.gov";
            if (args.Length > 2)
                baseurl = args[2];
            string searchurl = (baseurl.EndsWith('/') ? baseurl.TrimEnd('/') : baseurl) + INVENTION_SEARCH;

            Console.WriteLine("certificate file: " + certfileandpath);
            Console.WriteLine("certificate passcode: " + new string('*', passcode.Length));
            Console.WriteLine("search url: " + searchurl);

            //https://docs.microsoft.com/en-us/dotnet/api/system.net.http.httpclienthandler.servercertificatecustomvalidationcallback?view=net-6.0#system-net-http-httpclienthandler-servercertificatecustomvalidationcallback
            try
            {
                Console.WriteLine("creating certificate");
                using (X509Certificate2 cert = new X509Certificate2(certfileandpath, passcode))
                {
                    //Console.WriteLine("certificate created" + Environment.NewLine);
                    Console.WriteLine("Creating clientHandler");
                    HttpClientHandler clientHandler = new HttpClientHandler()
                    {
                        UseDefaultCredentials = true,
                        ClientCertificates = { cert },
                        ServerCertificateCustomValidationCallback = ServerCertificateCustomValidation,
                    };
                    var httpContent = new MultipartFormDataContent
                    {
                        { new StringContent("{ \"inventionTitle\": \"test\", \"limit\": 1 }"), "inventionSearchCriteria" }
                    };
                    //Console.WriteLine("clientHandler created" + Environment.NewLine);

                    Console.WriteLine("creating httpClient");
                    HttpClient httpClient = new HttpClient(clientHandler) { BaseAddress = new Uri(baseurl) };
                    httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    //Console.WriteLine("httpClient created" + Environment.NewLine);

                    Console.WriteLine("sending request");
                    var response = httpClient.PostAsync(INVENTION_SEARCH, httpContent).Result;
                    try
                    {
                        string str = response.Content.ReadAsStringAsync().Result;
                        response.EnsureSuccessStatusCode();
                    }
                    catch (HttpRequestException re)
                    {
                        return re.Message;
                    }
                    catch (Exception e)
                    {
                        return e.StackTrace;
                    }
                    Console.WriteLine("request complete" + Environment.NewLine);
                    string content = response.Content.ReadAsStringAsync().Result;
                    return content;
                }
            }
            catch (Exception ex)
            {
                return ex.Message + Environment.NewLine + ex.HResult + Environment.NewLine + ex.InnerException + Environment.NewLine + ex.StackTrace + Environment.NewLine + ex.HResult;
            }
        }

        private static bool ServerCertificateCustomValidation(HttpRequestMessage requestMessage, X509Certificate2 certificate, X509Chain chain, SslPolicyErrors sslErrors)
        {
            // It is possible inpect the certificate provided by server
            Console.WriteLine($"Requested URI: {requestMessage.RequestUri}");
            Console.WriteLine($"Effective date: {certificate.GetEffectiveDateString()}");
            Console.WriteLine($"Exp date: {certificate.GetExpirationDateString()}");
            Console.WriteLine($"Issuer: {certificate.Issuer}");
            Console.WriteLine($"Subject: {certificate.Subject}");

            // Based on the custom logic it is possible to decide whether the client considers certificate valid or not
            Console.WriteLine($"Errors: {sslErrors}");
            Console.WriteLine("");
            return sslErrors == SslPolicyErrors.None;
        }
    }
}
