namespace LancacheChunkPurge
{
    //TODO need to test this with things like windows update, steam, epic, etc
    //TODO document
    public static class NginxLogParser
    {
        public static List<LogInfo> ParseRequestLog(string requestLine)
        {
            var asArray = new List<string> { requestLine };
            return ParseRequestLogs2(asArray);
        }

        public static List<LogInfo> ParseRequestLogs2(List<string> rawRequests)
        {
            List<LogInfo> logInfos = new List<LogInfo>();

            foreach (var log in rawRequests)
            {
                // These can't be deleted, so they can't be processed in any way
                if (log.Contains("lancache-heartbeat"))
                {
                    continue;
                }

                // Matching requests that have a byte range
                //TODO document this
                string pattern = @"\[(?<cachekey>[^\]]+)\] .+ ""GET (?<url>[^ ]+) .+"" (?<status>\d+) (?<body_bytes_sent>\d+) .+ ""(?<http_range>-|bytes=[^""]+)""";
                Match match = Regex.Match(log, pattern);
                if (match.Success)
                {
                    LogInfo logInfo = new LogInfo
                    {
                        CacheKey = match.Groups["cachekey"].Value,
                        Url = match.Groups["url"].Value,
                        ResponseSizeBytes = uint.Parse(match.Groups["body_bytes_sent"].Value)
                    };

                    var byteRangeRequested = match.Groups["http_range"].Value;
                    if (byteRangeRequested == "-")
                    {
                        logInfo.RequestedWholeFile = true;
                    }
                    else
                    {
                        logInfo.RangeRequested = byteRangeRequested;
                    }

                    logInfos.Add(logInfo);
                }
            }
            return logInfos;
        }
    }
}