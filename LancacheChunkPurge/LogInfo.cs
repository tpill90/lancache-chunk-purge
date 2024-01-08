namespace LancacheChunkPurge
{
    //TODO document
    public sealed class LogInfo
    {
        public static readonly uint SliceSize = (uint)ByteSize.FromMebiBytes(1).Bytes;

        public string CacheKey { get; set; }

        public string Url { get; set; }
        public uint ResponseSizeBytes { get; set; }

        public bool RequestedWholeFile { get; set; }

        public string RangeRequested { get; set; }

        //TODO remove the 'range=' text from ever being selected in the first place
        public ulong LowerRange => UInt64.Parse(RangeRequested.Replace("bytes=", "").Split('-')[0]);
        public ulong UpperRange => UInt64.Parse(RangeRequested.Replace("bytes=", "").Split('-')[1]);

        public override string ToString()
        {
            return $"Cache Key: {CacheKey}, Url: {Url}, Response Size: {ResponseSizeBytes}, Byte Range Requested: {RangeRequested}";
        }

        //TODO comment
        public List<string> ComputeOnDiskFileName()
        {
            if (RequestedWholeFile)
            {
                var results = new List<string>();
                //TODO comment
                if (ResponseSizeBytes == 0)
                {
                    string requestKey = $"{CacheKey}{Url}bytes=0-1048575";
                    var hashed = CalculateMd5Hash(requestKey);

                    var filePath = $"{hashed.Substring(hashed.Length - 2)}/{hashed.Substring(hashed.Length - 4, 2)}/{hashed}";
                    results.Add(filePath);
                    return results;
                }

                var ranges = CalculateRanges(ResponseSizeBytes);
                foreach (var range in ranges)
                {
                    string requestKey = $"{CacheKey}{Url}bytes={range.Lower}-{range.Upper}";
                    var hashed = CalculateMd5Hash(requestKey);

                    var filePath = $"{hashed.Substring(hashed.Length - 2)}/{hashed.Substring(hashed.Length - 4, 2)}/{hashed}";
                    results.Add(filePath);
                }

                return results;
            }
            else
            {
                var calculatedHashes = new List<string>();
                var ranges = CalculateSliceAlignedSegments(new ByteRange { Lower = LowerRange, Upper = UpperRange });

                foreach (var range in ranges)
                {
                    string requestKey = $"{CacheKey}{Url}bytes={range.Lower}-{range.Upper}";
                    var hashed = CalculateMd5Hash(requestKey);
                    //TODO comment
                    var filePath = $"{hashed.Substring(hashed.Length - 2)}/{hashed.Substring(hashed.Length - 4, 2)}/{hashed}";

                    calculatedHashes.Add(filePath);
                }

                return calculatedHashes;
            }
        }

        public static List<ByteRange> CalculateRanges(uint totalSize)
        {
            var ranges = new List<ByteRange>();

            for (uint start = 0; start < totalSize; start += SliceSize)
            {
                uint end = start + SliceSize - 1;
                if (end >= totalSize)
                {
                    end = start + SliceSize - 1;
                }
                ranges.Add(new ByteRange
                {
                    Lower = start,
                    Upper = end
                });
            }

            return ranges;
        }

        public static List<ByteRange> CalculateSliceAlignedSegments(ByteRange range)
        {
            var segments = new List<ByteRange>();

            ulong start = range.Lower;
            ulong end = range.Upper;

            // Align the start up to the next slice start
            start = start / SliceSize * SliceSize;

            while (start < end)
            {
                ulong segmentEnd = start + SliceSize - 1;

                segments.Add(new ByteRange { Lower = start, Upper = segmentEnd });
                start = segmentEnd + 1;
            }

            return segments;
        }

        private static string CalculateMd5Hash(string combined)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(combined);
            return ManagedMD5.Calculate(inputBytes);
        }
    }

    public sealed class ByteRange
    {
        public ulong Lower { get; set; }
        public ulong Upper { get; set; }

        public override string ToString()
        {
            return $"{Lower}-{Upper}";
        }
    }
}
