using APIClientPackage;

namespace APIClientTest
{
    class Program
    {
        static async System.Threading.Tasks.Task Main(string[] args)
        {
            LogInformation _log = new LogInformation(@"MainLog.log");
            LogInformation _logTasks = new LogInformation(@"TasksLog.log");
            _log.Info("Program Start");

            APIClient apiClient = new APIClient(ref _log, ref _logTasks);

            //TEST
            //await apiClient.RunTaskTest();

            await apiClient.RunAsync();
        }
    }
}
