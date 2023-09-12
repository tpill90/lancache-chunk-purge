namespace LancacheChunkPurge.Test
{
    public sealed class HashCalculatorTests
    {
        //TODO rename + explain
        [Fact]
        public void WholeFile_RangeRequested()
        {
            var inputLogLine = "[blizzard] 192.168.1.55 / - - - [12/Sep/2023:17:25:25 +0100] \"GET /tpr/sc1live/data/c9/7e/c97e6071294fb69f542c57874d8433c5 HTTP/1.1\" 206 99700 \"-\" \"-\" \"MISS\" \"cdn.blizzard.com\" \"bytes=0-99699\"";
            var parsedLog = NginxLogParser.ParseRequestLog(inputLogLine).First();

            var calculatedFilePath = parsedLog.ComputeOnDiskFileName().First();

            var expected = "5f/6a/32fc9098be7edf9ae705aeb659836a5f";
            Assert.Equal(expected: expected, calculatedFilePath);
        }

        //TODO rename + explain
        [Fact]
        public void WholeFile()
        {
            var inputLogLine = "[steam] 192.168.1.55 / - - - [12/Sep/2023:18:36:35 +0100] \"GET /depot/945361/chunk/79c3227473e1293d952744d09774c68d8d41b150 HTTP/1.1\" 200 840016 \"-\" \"Valve/Steam HTTP Client 1.0\" \"MISS\" \"cache10-iad1.steamcontent.com\" \"-\"";
            var parsedLog = NginxLogParser.ParseRequestLog(inputLogLine).First();

            var calculatedFilePath = parsedLog.ComputeOnDiskFileName().First();

            var expected = "39/79/73519632edaddcb5827be4b39d7b7939";
            Assert.Equal(expected: expected, calculatedFilePath);
        }

        //TODO rename + explain
        [Fact]
        public void WholeFile_SpansTwoSlices()
        {
            var inputLogLine = "[steam] 192.168.1.55 / - - - [12/Sep/2023:19:05:37 +0100] \"GET /depot/264712/chunk/f852a6f3b3c10b47d70dad30224a5b0845358294 HTTP/1.1\" 200 1054000 \"-\" \"Valve/Steam HTTP Client 1.0\" \"MISS\" \"cache6-iad1.steamcontent.com\" \"-\"";
            var parsedLog = NginxLogParser.ParseRequestLog(inputLogLine).First();

            //TODO I'm not entirely sure that this test case is correct.  Steam seems that it should span two slices, but for whatever reason only actually uses 1
            var calculatedFilePath = parsedLog.ComputeOnDiskFileName().First();

            var expected = "42/39/b93c31fd91ddb439f9c4d0ba04673942";
            Assert.Equal(expected: expected, calculatedFilePath);
        }

        //TODO rename + explain
        [Fact]
        public void RangeRequest_SpansMultipleSlices()
        {
            var inputLogLine = "[blizzard] 192.168.1.55 / - - - [12/Sep/2023:22:07:37 +0100] \"GET /tpr/zeus/data/c1/94/c1942d6badb10a911d3e617bac1e7be0 HTTP/1.1\" 206 6457285 \"-\" \"PostmanRuntime/7.32.3\" \"HIT\" \"level3.blizzard.com\" \"bytes=26380013-32837297\"";
            var parsedLog = NginxLogParser.ParseRequestLog(inputLogLine).First();

            var calculatedFilePaths = parsedLog.ComputeOnDiskFileName();
            calculatedFilePaths.Should().HaveCount(7);

            calculatedFilePaths[0].Should().Be("0d/0e/6ace2ce71c04d335965ee2dc70550e0d");
            calculatedFilePaths[1].Should().Be("69/c2/4b91e8e526be6e04d040aa199df9c269");
            calculatedFilePaths[2].Should().Be("e0/4d/a9b3583eee3d4111af9d7c54fbf74de0");
            calculatedFilePaths[3].Should().Be("5f/0b/d095893c2cdbb4ba37d00d9b12520b5f");
            calculatedFilePaths[4].Should().Be("3c/c6/2cfb5b84a9fad3e5c0f62fbaee57c63c");
            calculatedFilePaths[5].Should().Be("21/b7/90dd1966e01fa49a9d49f9200551b721");
            calculatedFilePaths[6].Should().Be("90/a7/da189ae006772318633d6ebe04f1a790");

        }
    }
}
