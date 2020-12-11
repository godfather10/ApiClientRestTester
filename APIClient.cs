using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
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

        private List<string> _commandLst = new List<string>();
        
        private List<string> _configurationLst = new List<string>();

        private HttpClient _client = null;

        private string _webSocketPath = "";

        private string _restPath = "";

        public APIClient(ref LogInformation log, ref LogInformation logTasks)
        {
            _log = log;
            _logTasks = logTasks;            
            LoadConfiguration();
            SSLClient();
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
                _commandLst.Clear();
                _commandLst = File.ReadAllLines("CommandsConfiguration.cfg").ToList();
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
                foreach(string val in _commandLst)
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
                _logTasks.LogMsgOnly(" ");
                _logTasks.LogMsgOnly("@ @ @ @ @ @ @ @ @ @ @ @ EXECUTE GET REST @ @ @ @ @ @ @ @ @ @ @ @");
                _logTasks.LogMsgOnly(string.Format("Path:{0}",_restPath));
                _logTasks.LogMsgOnly(string.Format("Agent:{0}","User-Agent"));
                _logTasks.LogMsgOnly(string.Format("With:{0}",".NET Foundation Repository Reporter"));
                _logTasks.LogMsgOnly(string.Format("Command:{0}",_commandLst[line]));

                _client.BaseAddress = new Uri(_restPath);

                // Add an Accept header for JSON format.  
                //_client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // Add an Accept header for HTML format.  
                _client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("text/html"));
                HttpResponseMessage response = _client.GetAsync(_commandLst[line]).Result;  // Blocking call!  

                if (response.IsSuccessStatusCode)
                {
                    _logTasks.LogMsgOnly("Request Message Information:- \n\n" + response.RequestMessage + "\n");
                    _logTasks.LogMsgOnly("Response Message Header \n\n" + response.Content.Headers + "\n");
                }
                else
                {
                    _logTasks.LogMsgOnly("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
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

        private async Task ExecutePOSTCommandAsync(int line )
        {
            try
            {
                string[] dataLineAll = _commandLst[line].Split(' ');
                string url = dataLineAll[0];
                string paramters = dataLineAll[1];

                _logTasks.LogMsgOnly(" ");
                _logTasks.LogMsgOnly("@ @ @ @ @ @ @ @ @ @ @ @ EXECUTE POST REST @ @ @ @ @ @ @ @ @ @ @ @");
                _logTasks.LogMsgOnly(string.Format("Path:{0}",_restPath));
                _logTasks.LogMsgOnly(string.Format("Agent:{0}","User-Agent"));
                _logTasks.LogMsgOnly(string.Format("With:{0}",".NET Foundation Repository Reporter"));
                _logTasks.LogMsgOnly(string.Format("Command:{0}",_commandLst[line]));

                _client.BaseAddress = new Uri(_restPath);
                System.Net.Http.HttpContent content = new StringContent(paramters, UTF8Encoding.UTF8, "application/json");
                var response = _client.PostAsync(url, content).Result;                      
                
                if (response.IsSuccessStatusCode)
                {
                    _logTasks.LogMsgOnly("Request Message Information:- \n\n" + response.RequestMessage + "\n");
                    _logTasks.LogMsgOnly("Response Message Header \n\n" + response.Content.Headers + "\n");
                }
                else
                {
                    _logTasks.LogMsgOnly("{0} ({1})", (int)response.StatusCode, response.ReasonPhrase);
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