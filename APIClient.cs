using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.WebSockets;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace APIClientPackage
{
    /// <summary>
    /// Class definition for the API Client run
    /// </summary>
    public class APIClient
    {
        private LogInformation _log = null;

        private LogInformation _logTasks = null;

        private List<string> _commandGETLst = new List<string>();

        private List<string> _commandPOSTLst = new List<string>();
        
        private List<string> _configurationLst = new List<string>();

        private HttpClient _client = null;

        private ClientWebSocket _ws = null;

        private CancellationTokenSource _wsCts = null;

        private bool _initWs = false;

        private string _webSocketPath = "";

        private string _restPath = "";

        private string certificateFileName = "";

        public APIClient(ref LogInformation log, ref LogInformation logTasks)
        {
            _log = log;
            _logTasks = logTasks;            
            LoadConfiguration();            
            SSLClient();
        }

        private async Task<int> WebSocketInitAsync()
        {
            try
            {
                _ws = new ClientWebSocket();
                Uri uri = new Uri(_webSocketPath);
                _wsCts = new CancellationTokenSource();
                 await _ws.ConnectAsync(uri, _wsCts.Token);
                return 0;
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                return -1;
            }
        }

        private int SSLClient()
        {            
            try
            {
                HttpClientHandler clientHandler = new HttpClientHandler();
                //clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
                clientHandler.ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator;
                //clientHandler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate,
                //clientHandler.CookieContainer = container;
                
                // Pass the handler to httpclient(from you are calling api)
                _client = new HttpClient(clientHandler);

                //X509Certificate2 cert = new X509Certificate2(certificateFileName, "", X509KeyStorageFlags.MachineKeySet);
                //_ws.SslConfiguration.ClientCertificateSelectionCallback =
                //(sender,targethost,localCertificates, remoteCertificate,acceptableIssuers) =>
                //{
                //    return cert;
                //};

                return 0;
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                return -1;
            }
        }

        public void LoadConfiguration()
        {
            try
            {
                _configurationLst.Clear();
                _configurationLst = File.ReadAllLines("GeneralConfiguration.cfg").ToList();
                _webSocketPath = _configurationLst[0];
                _restPath = _configurationLst[1];
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
            }
        }

        public void LoadCommands()
        {
            try
            {
                _commandGETLst.Clear();
                _commandGETLst = File.ReadAllLines("CommandsGETConfiguration.cfg").ToList();

                _commandPOSTLst.Clear();
                _commandPOSTLst = File.ReadAllLines("CommandsPOSTConfiguration.cfg").ToList();
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
            }
        }

        private void PrintConfigurationInfo()
        {
            try
            {
                Console.WriteLine(" ");
                Console.WriteLine(" ");
                Console.WriteLine("# # # # # # # # # # # # # # # # # # # # # # # # #");
                Console.WriteLine("# # # MENU CONFIGURATION");
                int counter = 0;
                foreach(string val in _configurationLst)
                {
                    Console.WriteLine(string.Format("{0}->{1}",counter++,val));
                }
                Console.WriteLine("ret -> Return to main menu.");   
                Console.WriteLine(" ");
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
            }         
        }

        private void PrintCommandsInfo()
        {
            try
            {
                Console.WriteLine(" ");
                Console.WriteLine(" ");
                Console.WriteLine("# # # # # # # # # # # # # # # # # # # # # # # # #");
                Console.WriteLine("# # # MENU COMMANDS");
                int counter = 0;
                foreach(string val in _commandGETLst)
                {
                    Console.WriteLine(string.Format("GET#{0}->{1}",counter++,val));
                }

                counter = 0;
                foreach(string val in _commandPOSTLst)
                {
                    Console.WriteLine(string.Format("POST#{0}->{1}",counter++,val));
                }

                Console.WriteLine("ret -> Return to main menu.");    
                Console.WriteLine(" ");
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
            }        
        }

        private void PrintMainMenu()
        {
            try
            {
                Console.WriteLine(" ");
                Console.WriteLine(" ");
                Console.WriteLine("# # # # # # # # # # # # # # # # # # # # # # # # #");
                Console.WriteLine("# # # FriendUp Core API Tester");
                Console.WriteLine("1 -> See general configuration (file GeneralConfiguration.cfg).");
                Console.WriteLine("2 -> See commands calls configuration (file CommandsConfiguration.cfg).");
                Console.WriteLine("get#N -> Execute command number N (Type GET)");
                Console.WriteLine("post#N -> Execute command number N (Type POST)");
                Console.WriteLine("exit -> Terminate Program.");            
                Console.WriteLine(" ");
                Console.WriteLine("OPTIONS: ");
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
            }
        }

        private async Task ExecuteGETCommandAsync(int line)
        {
            try
            {
                if((line < 0)||(line >= _commandGETLst.Count))
                {
                    _log.Error("Command index out of range:{0}",line);
                    return;
                }

                _log.Info("ExecuteGETCommandAsync START cmd:{0}",_commandGETLst[line]);
                Console.WriteLine("Executing cmd:{0}",_commandGETLst[line]);

                _logTasks.LogMsgOnly(" ");
                _logTasks.LogMsgOnly("@ @ @ @ @ @ @ @ @ @ @ @ EXECUTE GET REST @ @ @ @ @ @ @ @ @ @ @ @");
                _logTasks.LogMsgOnly(string.Format("Path:{0}",_restPath));
                _logTasks.LogMsgOnly(string.Format("Agent:{0}","User-Agent"));
                _logTasks.LogMsgOnly(string.Format("With:{0}",".NET Foundation Repository Reporter"));
                _logTasks.LogMsgOnly(string.Format("Command:{0}",_commandGETLst[line]));

                _client.BaseAddress = new Uri(_restPath);
                _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));
                HttpResponseMessage response = _client.GetAsync(_commandGETLst[line]).Result;  // Blocking call!  

                if (response.IsSuccessStatusCode)
                {
                    _logTasks.LogMsgOnly("Request Message Information:- \n\n" + response.RequestMessage + "\n");
                    _logTasks.LogMsgOnly("Response Message Header \n\n" + response.Content.Headers + "\n");
                    _log.Info("Request Message Information:- \n\n" + response.RequestMessage + "\n");
                    _log.Info("Response Message Header \n\n" + response.Content.Headers + "\n");
                    Console.WriteLine("Command executed with SUCCESS!");
                }
                else
                {
                    _logTasks.LogMsgOnly("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
                    _log.Info("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
                    Console.WriteLine("ERROR executing Command!");
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                if(ex.HelpLink!=null)
                {
                    _log.Error(ex.HelpLink);
                }
                if(ex.StackTrace!=null)
                {
                    _log.Error(ex.StackTrace);
                }
                if(ex.TargetSite!=null)
                {
                    _log.Error(ex.TargetSite.ToString());                
                }
            }
        }

        private async Task ExecutePOSTCommandAsync(int line)
        {
            try
            {
                if((line < 0)||(line >= _commandPOSTLst.Count))
                {
                    _log.Error("Command index out of range:{0}",line);
                    return;
                }

                _log.Info("ExecuteGETCommandAsync START cmd:{0}",_commandPOSTLst[line]);                
                Console.WriteLine("Executing cmd:{0}",_commandPOSTLst[line]);

                string[] dataLineAll = _commandPOSTLst[line].Split(' ');
                string url = dataLineAll[0];
                string paramters = dataLineAll[1];

                _logTasks.LogMsgOnly(" ");
                _logTasks.LogMsgOnly("@ @ @ @ @ @ @ @ @ @ @ @ EXECUTE POST REST @ @ @ @ @ @ @ @ @ @ @ @");
                _logTasks.LogMsgOnly(string.Format("Path:{0}",_restPath));
                _logTasks.LogMsgOnly(string.Format("Agent:{0}","User-Agent"));
                _logTasks.LogMsgOnly(string.Format("With:{0}",".NET Foundation Repository Reporter"));
                _logTasks.LogMsgOnly(string.Format("Command:{0}",_commandPOSTLst[line]));

                _client.BaseAddress = new Uri(_restPath);
                System.Net.Http.HttpContent content = new StringContent(paramters, UTF8Encoding.UTF8, "application/json");
                var response = _client.PostAsync(url, content).Result;                      

                if (response.IsSuccessStatusCode)
                {
                    _logTasks.LogMsgOnly("Request Message Information:- \n\n" + response.RequestMessage + "\n");
                    _logTasks.LogMsgOnly("Response Message Header \n\n" + response.Content.Headers + "\n");
                    _log.Info("Request Message Information:- \n\n" + response.RequestMessage + "\n");
                    _log.Info("Response Message Header \n\n" + response.Content.Headers + "\n");
                    Console.WriteLine("Command executed with SUCCESS!");
                }
                else
                {
                    _logTasks.LogMsgOnly("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
                    _log.Info("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
                    Console.WriteLine("ERROR executing Command!");
                }
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
                if(ex.HelpLink!=null)
                {
                    _log.Error(ex.HelpLink);
                }
                if(ex.StackTrace!=null)
                {
                    _log.Error(ex.StackTrace);
                }
                if(ex.TargetSite!=null)
                {
                    _log.Error(ex.TargetSite.ToString());                
                }
            }
        }

        public async Task RunTaskTest()
        {
                try
                {
                    _client.DefaultRequestHeaders.Accept.Clear();
                    _client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
                    _client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");

                    var stringTask = _client.GetStringAsync("https://api.github.com/orgs/dotnet/repos");

                    var msg = await stringTask;
                    Console.Write(msg);
                }
                catch (Exception ex)
                {
                    _log.Error(ex.Message);
                }
        }

        public async Task RunAsync()
        {
            string commandStr = "";
            bool running=true;
            int menuToPrint = 0;

            LoadCommands();
            LoadConfiguration();

            while(running)
            {
                if(menuToPrint == 0)
                {
                    PrintMainMenu();
                }
                else if(menuToPrint == 1)
                {
                    PrintConfigurationInfo();
                }
                else if(menuToPrint == 2)
                {
                    PrintCommandsInfo();
                }
                else
                {
                    PrintMainMenu();
                }

                Console.WriteLine("OPÇÃO -> ");
                commandStr = Console.ReadLine();
             
                if(commandStr.ToLower() == "exit")
                {
                    running=false;
                }
                else if(commandStr.ToLower().Contains("get#"))
                {
                    string[] dataLineAll = commandStr.Split('#');

                    await ExecuteGETCommandAsync(int.Parse(dataLineAll[1]));
                }   
                else if(commandStr.ToLower().Contains("post#"))
                {
                    string[] dataLineAll = commandStr.Split('#');

                    await ExecutePOSTCommandAsync(int.Parse(dataLineAll[1]));
                }                      
                else if(menuToPrint == 1) // Configuration Info
                {
                    if(commandStr.ToLower() == "ret")
                    {
                        menuToPrint = 0;   
                    } 
                }
                else if(menuToPrint == 2) // Commands Info
                {
                    if(commandStr.ToLower() == "ret")
                    {
                        menuToPrint = 0;   
                    } 
                }
                else// Main menu
                {
                    if(commandStr.ToLower() == "1")
                    {
                        menuToPrint = 1;
                    }  
                    else if(commandStr.ToLower() == "2")
                    {
                        menuToPrint = 2;
                    }    
                }

                LoadCommands();
                LoadConfiguration();
            }
        }
    }
}