using System.Text;

class Program
{
    private const string WindowTitle = "WARP-Plus-Generator";
    private const string ApiUrl = "https://api.cloudflareclient.com/v0a{0}/reg";

    private static readonly HttpClient HttpClient = new();

    static async Task Main()
    {
        Console.Title = WindowTitle;
        Console.Clear();

        string referrer = GetReferrer();

        int g = 0, b = 0;

        while (true)
        {
            Console.Clear();
            Console.WriteLine($"\r[+] Sending request...   [----------] 0%");

            int result = await SendRequestAsync(referrer);

            if (result == 200)
            {
                g++;
                ShowProgressBar();
                Console.WriteLine($"\n[-] WORK ON ID: {referrer}");
                Console.WriteLine($"[:)] {g} GB has been successfully added to your account.");
                Console.WriteLine($"[#] Total: {g} Good {b} Bad");

                await WaitSeconds(18);
            }
            else
            {
                b++;
                Console.WriteLine("[:(] Error when connecting to the server.");
                Console.WriteLine($"[#] Total: {g} Good {b} Bad");

                await WaitSeconds(10);
            }
        }
    }

    static string GetReferrer()
    {
        Console.Write("[#] Enter the User ID: ");
        string? referrer = Console.ReadLine();

        if (string.IsNullOrEmpty(referrer))
        {
            Console.WriteLine("User ID cannot be empty.");
            Environment.Exit(1);
        }

        return referrer;
    }

    static async Task<int> SendRequestAsync(string referrer)
    {
        try
        {
            string installId = GenerateRandomString(22);
            string key = GenerateRandomString(43) + "=";
            string fcmToken = $"{installId}:APA91b{GenerateRandomString(134)}";
            string tos = $"{DateTime.Now:yyyy-MM-ddTHH:mm:sszzz}";
            string requestBody = $@"
            {{
                ""key"": ""{key}"",
                ""install_id"": ""{installId}"",
                ""fcm_token"": ""{fcmToken}"",
                ""referrer"": ""{referrer}"",
                ""warp_enabled"": false,
                ""tos"": ""{tos}"",
                ""type"": ""Android"",
                ""locale"": ""es_ES""
            }}";

            string apiUrl = string.Format(ApiUrl, GenerateDigitString(3));
            byte[] data = Encoding.UTF8.GetBytes(requestBody);

            var requestMessage = new HttpRequestMessage(HttpMethod.Post, apiUrl);
            requestMessage.Headers.Host = "api.cloudflareclient.com";
            requestMessage.Headers.Add("Accept-Encoding", "gzip");
            requestMessage.Headers.Add("User-Agent", "okhttp/3.12.1");

            using var requestContent = new ByteArrayContent(data);
            requestMessage.Content = requestContent;

            var response = await HttpClient.SendAsync(requestMessage);
            return (int)response.StatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine();
            Console.WriteLine(ex.Message);
            return 0;
        }
    }


    static void ShowProgressBar()
    {
        string[] animation = new string[]
        {
            "[----------]", "[x---------]", "[xx--------]", "[xxx-------]",
            "[xxxx------]", "[xxxxx-----]", "[xxxxxx----]", "[xxxxxxx---]",
            "[xxxxxxxx--]", "[xxxxxxxxx-]"
        };

        int progressAnim = 0;
        int percent = 0;

        while (percent < 100)
        {
            for (int i = 0; i < 10 && percent < 100; i++)
            {
                percent++;
                Console.Write($"\r[+] Waiting response...  {animation[progressAnim % animation.Length]} {percent}%");
                Thread.Sleep(75);
            }

            progressAnim++;
        }

        Console.Write("\r[+] Request completed... [xxxxxxxxxx] 100%");
    }

    static async Task WaitSeconds(int seconds)
    {
        for (int i = seconds; i > 0; i--)
        {
            Console.Write($"\r[*] After {i} seconds, a new request will be sent.");
            await Task.Delay(1000);
        }
    }

    static string GenerateRandomString(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        Random random = new();
        StringBuilder sb = new();

        for (int i = 0; i < length; i++)
        {
            sb.Append(chars[random.Next(chars.Length)]);
        }

        return sb.ToString();
    }

    static string GenerateDigitString(int length)
    {
        const string digits = "0123456789";
        Random random = new();
        StringBuilder sb = new();

        for (int i = 0; i < length; i++)
        {
            sb.Append(digits[random.Next(digits.Length)]);
        }

        return sb.ToString();
    }
}
