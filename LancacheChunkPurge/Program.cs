namespace LancacheChunkPurge
{
    //TODO how do permissions for the cache work exactly?  Will people need to run this inside of the monolithic container, or can they run it on their host machine with config.
    //TODO think of a better name for this project.  Maybe file purger?
    //TODO lancache prefill common needs to be a submodule.
    public static class Program
    {
        //public static string logFilePath = @"C:\Users\Tim\Dropbox\Programming\Lancache-Prefills\lancache-file-deleter\scripts\access.log";

       //TODO support json log format
        // log_format cachelog-json escape=json '{"timestamp":"$msec","time_local":"$time_local","cache_identifier":"$cacheidentifier","remote_addr":"$remote_addr","forwarded_for":"$http_x_forwarded_for","remote_user":"$remote_user","status":"$status","bytes_sent":$body_bytes_sent,"referer":"$http_referer","user_agent":"$http_user_agent","upstream_cache_status":"$upstream_cache_status","host":"$host","http_range":"$http_range","method":"$request_method","path":"$request_uri","proto":"$server_protocol","scheme":"$scheme"}';

        public static int Main()
        {
          

            // Return failed status code, since you can only get to this line if an exception was handled
            return 1;
        }

        private static void ActualLogic()
        {
            var inputLogLine = "[blizzard] 192.168.1.55 / - - - [12/Sep/2023:22:07:37 +0100] \"GET /tpr/zeus/data/c1/94/c1942d6badb10a911d3e617bac1e7be0 HTTP/1.1\" 206 6457285 \"-\" \"PostmanRuntime/7.32.3\" \"HIT\" \"level3.blizzard.com\" \"bytes=26380013-32837297\"";
            
            var parsedRequestLog = NginxLogParser.ParseRequestLog(inputLogLine).First();
            var md5Hashed = parsedRequestLog.ComputeOnDiskFileName();


            var processingTimer = Stopwatch.StartNew();

            //var rawLogs = File.ReadAllLines(logFilePath);
            //var parsedRequests = NginxLogParser.ParseRequestLogs2(rawLogs);

            

            Debugger.Break();
        }

        
    }
}