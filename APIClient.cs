using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
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

        //private ApiConnectorClient _apiConnectorClient = null;

        private static readonly HttpClient client = new HttpClient();

        string _webSocketPath = "";

        string _restPath = "";

        public APIClient(ref LogInformation log, ref LogInformation logTasks)
        {
            _log = log;
            _logTasks = logTasks;
            LoadConfiguration();
        }

        public void LoadConfiguration()
        {
            try
            {
                //List<string> configurationLst = new List<string>();
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
                //List<string> configurationLst = new List<string>();
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
                Console.WriteLine(" ");
                Console.WriteLine("ret -> Return to main menu.");   
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

                Console.WriteLine(" ");
                Console.WriteLine("ret -> Return to main menu.");    
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
                Console.WriteLine("EX#N -> Execute command number N");
                Console.WriteLine("exit -> Terminate Program.");            
                Console.WriteLine("OPTIONS: ");
            }
            catch (Exception ex)
            {
                _log.Error(ex.Message);
            }
        }

        private async Task ExecuteCommandAsync(int line)
        {
            try
            {
                _logTasks.LogMsgOnly(" ");
                _logTasks.LogMsgOnly("@ @ @ @ @ @ @ @ @ @ @ @ EXECUTE REST @ @ @ @ @ @ @ @ @ @ @ @");
                _logTasks.LogMsgOnly(string.Format("Path:{0}",_restPath));
                _logTasks.LogMsgOnly(string.Format("Agent:{0}","User-Agent"));
                _logTasks.LogMsgOnly(string.Format("With:{0}",".NET Foundation Repository Reporter"));
                _logTasks.LogMsgOnly(string.Format("Command:{0}",_commandLst[line]));

                client.DefaultRequestHeaders.Accept.Clear();                
                client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(_commandLst[line]));
                var stringTask = client.GetStringAsync(_restPath);

                var msg = await stringTask;
                _logTasks.LogMsgOnly(string.Format("RESULT:{0}",msg));

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
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(
                        new MediaTypeWithQualityHeaderValue("application/vnd.github.v3+json"));
                    client.DefaultRequestHeaders.Add("User-Agent", ".NET Foundation Repository Reporter");

                    var stringTask = client.GetStringAsync("https://api.github.com/orgs/dotnet/repos");

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
                else if(commandStr.ToLower().Contains("ex#"))
                {
                    string[] dataLineAll = commandStr.Split('#');

                    await ExecuteCommandAsync(int.Parse(dataLineAll[1]));
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