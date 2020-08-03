using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace DistSysACWClient
{
    #region Task 10 and beyond
    class Client
    {
        private const string url = "http://localhost:52555";
        private static HttpClient client = new HttpClient();

        static string mUser, mKey;
        static Guid guid;
        const string loading = "Please wait while it loads...";

        static byte[] rsakey;

        public Client()
        {
            client.BaseAddress = new Uri(url);
        }

        #region Tasks

        #region simple stuff
        static async Task Clear()
        {
            var response = client.GetAsync("api/Other/Clear");
            Console.WriteLine(loading);
            var res = await response.Result.Content.ReadAsStringAsync();
            Console.WriteLine(res);
        }

        static async Task Hello()
        {

            var response = client.GetAsync("api/talkback/Hello");
            Console.WriteLine(loading);
            var res = await response.Result.Content.ReadAsStringAsync();
            Console.WriteLine(res);

        }

        static async Task Sort(int[] ints)
        {
            var str = "integers=";
            var contain = new string[5];
            string strings = null;
            for (int i = 0; i < ints.Length; i++)
            {
                if (ints[i] != ints.Length + 1)
                {
                    contain[i] = str + ints[i] + "&";
                }
                else
                {
                    contain[i] = str + ints[i];

                }
                strings += contain[i];
            }
            var response = client.GetAsync("api/talkback/Sort?" + strings);
            Console.WriteLine(loading);
            var res = await response.Result.Content.ReadAsStringAsync();
            Console.WriteLine(res);
        }

        static async Task PublicKey()
        {
            var response = await client.GetAsync("api/protected/getpublickey");
            Console.WriteLine(loading);
            rsakey = await response.Content.ReadAsByteArrayAsync();

            if (response.IsSuccessStatusCode)
            {
                Console.WriteLine("Got Public Key");
            }
            else
            {
                Console.WriteLine("Couldnt Get the Public Key");
            }

        }

        #endregion

        #region database interaction
        static async Task GetUser(string pUser)
        {
            var response = client.GetAsync("api/user/new?username=" + pUser);
            Console.WriteLine(loading);
            var res = await response.Result.Content.ReadAsStringAsync();
            Console.WriteLine(res);
        }

        static async Task PostUser(string pUser)
        {
            var json = JsonConvert.SerializeObject(pUser);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            var response = client.PostAsync("api/user/new", data);
            Console.WriteLine(loading);
            var res = await response.Result.Content.ReadAsStringAsync();
            if (response.Result.IsSuccessStatusCode)
            {
                mKey = res;
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                client.DefaultRequestHeaders.Add("ApiKey", mKey);
            }
            else
            {
                Console.WriteLine(res);
            }
        }

        static async Task Create(string pUser)
        {
            mUser = pUser;
            guid = Guid.NewGuid();
            mKey = guid.ToString();
            var values = new Dictionary<string, string>
            {
                {mUser,mKey}
            };
        }

        static async Task Delete(string pUser)
        {
            var response = client.DeleteAsync("api/user/removeuser?username=" + pUser);
            Console.WriteLine(loading);
            var res = await response.Result.Content.ReadAsStringAsync();
            Console.WriteLine(res);
        }

        static async Task Update(string pUser, string pRole)
        {
            var values = new Dictionary<string, string>
            {
                { "username", pUser},
                { "role", pRole }
            };
            var json = JsonConvert.SerializeObject(values);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            var response = client.PostAsync("api/user/changerole", data);
            Console.WriteLine(loading);
            var res = await response.Result.Content.ReadAsStringAsync();
            Console.WriteLine(res);
        }
        #endregion

        #region Secured SHA stuff

        static async Task SecureHello()
        {
            var response = await client.GetAsync("api/protected/hello");
            Console.WriteLine(loading);
            var res = await response.Content.ReadAsStringAsync();
            Console.WriteLine(res);

        }

        static async Task SecureSHA1(string message)
        {
            var response = client.GetAsync("api/protected/sha1?message=" + message);
            Console.WriteLine(loading);
            var res = await response.Result.Content.ReadAsStringAsync();
            Console.WriteLine(res);
        }

        static async Task SecureSHA256(string message)
        {
            var response = client.GetAsync("api/protected/sha256?message=" + message);
            Console.WriteLine(loading);
            var res = await response.Result.Content.ReadAsStringAsync();
            Console.WriteLine(res);
        }

        #endregion

        static async Task Sign(string message)
        {
            byte[] original = Encoding.ASCII.GetBytes(message);
            byte[] signed;

            var response = client.GetAsync("api/protected/protected/sign?message=" + message);
            var res = await response.Result.Content.ReadAsStringAsync();
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.ImportCspBlob(rsakey);
            bool veirfy = rsa.VerifyData(original, new SHA1CryptoServiceProvider(), rsakey);
            Console.WriteLine(loading);
            Console.WriteLine(res);
            if (veirfy)
            {
                Console.WriteLine("Message was signed");
            }
            else if (!veirfy)
            {
                Console.WriteLine("Message wasn't signed");
            }
            else if (res == null)
            {
                Console.WriteLine("Unkown public key");
            }
        }

        //Uses this to allow MainAsync to run as the Main method
        public static void Main()
        {
            MainAsync().GetAwaiter().GetResult();
        }

        private static async Task MainAsync()
        {
            client.BaseAddress = new Uri(url);
            string input = null;

            Start:
            Console.WriteLine("What to do?\n" +
                "1.Hello\n" +
                "2.Sort\n" +
                "3.Get a User\n" +
                "4.Add a New User\n" +
                "5.Create User\n" +
                "6.Delete a User\n" +
                "7.Update a User's Role\n" +
                "8.Secure Hello\n" +
                "9.Secure SHA1 Message\n" +
                "10.Secure SHA256 Message\n" +
                "11.Secure Public Key Retrieval\n" +
                "12.Sign\n" +
                "13.Exit"
                );

            try
            {
                string read = Console.ReadLine();
                switch (read)
                {
                    case "1":
                        Hello();
                        Clear();
                        break;
                    case "2":
                        Console.WriteLine("What 5 numbers do you want to sort?");
                        int[] array = new int[5];
                        int x;
                        for (int i = 0; i < array.Length; i++)
                        {
                            input = Console.ReadLine();
                            if (int.TryParse(input, out x))
                            {
                                array[i] = x;
                            }
                            else
                            {
                                i--;
                                Console.WriteLine("Only enter whole integers. Try again.");
                            }
                        }
                        Sort(array);
                        break;
                    case "3":
                        Console.WriteLine("Enter a username to look up.");
                        var s = Console.ReadLine();
                        GetUser(s);
                        break;
                    case "4":
                        Console.WriteLine("Add a new user.");
                        if (mUser != null)
                        {
                            PostUser(mUser);
                        }
                        else
                        {
                            PostUser(Console.ReadLine());
                        }
                        break;
                    case "5":
                        Console.WriteLine("Create User");
                        Create(Console.ReadLine());
                        break;
                    case "6":
                        Console.WriteLine("Enter the username of the user you wish to delete.");
                        string q = Console.ReadLine();
                        Delete(q);
                        break;
                    case "7":
                        Console.WriteLine("Enter username to change role.");
                        string user = Console.ReadLine();
                        Console.WriteLine("What role do you want to assign?");
                        string role = Console.ReadLine();
                        Update(user, role);
                        break;

                    case "8":
                        await SecureHello();
                        break;
                    case "9":
                        Console.WriteLine("What would you like to say?");
                        string goIn = Console.ReadLine();
                        SecureSHA1(goIn);
                        break;
                    case "10":
                        Console.WriteLine("What would you like to say?");
                        string inputs = Console.ReadLine();
                        SecureSHA256(inputs);
                        break;
                    case "11":
                        await PublicKey();
                        break;
                    case "12":
                        Console.WriteLine("What would you like to say?");
                        input = Console.ReadLine();
                        Sign(input);
                        break;
                    case "13":
                        Environment.Exit(-1);
                        return;
                    default:
                        Console.Clear();
                        Console.WriteLine("Please select from the list.");
                        goto Start;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("An error occured: " + e);
            }

            Restart:
            Console.WriteLine("Restart? y/n");
            string end = Console.ReadLine();

            if (end.ToLower() == "y")
            {
                Console.Clear();
                goto Start;
            }
            else if (end.ToLower() == "n")
            {
                Environment.Exit(-1);
                return;
            }
            else if (end != "y" || end != "n")
            {
                Console.Clear();
                goto Restart;
            }
        }

        #endregion
    }
    #endregion
}
