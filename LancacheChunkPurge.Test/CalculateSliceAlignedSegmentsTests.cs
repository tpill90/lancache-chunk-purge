namespace LancacheChunkPurge.Test
{
    public sealed class CalculateSliceAlignedSegmentsTests
    {
        //TODO rename + explain
        [Fact]
        public void Test1()
        {
            var byteRange = new ByteRange
            {
                Lower = 0,
                Upper = 9993
            };
            var segments = LogInfo.CalculateSliceAlignedSegments(byteRange);

            //TODO explain
            segments.Should().ContainSingle();

            var segment = segments.Single();
            segment.Lower.Should().Be(0);
            // Should be rounded up to the next slice ending
            segment.Upper.Should().Be(1048575);
        }

        //TODO rename + explain
        [Fact]
        public void Test2()
        {
            var byteRange = new ByteRange
            {
                Lower = 1111,
                Upper = 9993
            };
            var segments = LogInfo.CalculateSliceAlignedSegments(byteRange);

            //TODO explain
            segments.Should().ContainSingle();

            var segment = segments.Single();
            segment.Lower.Should().Be(0);
            // Should be rounded up to the next slice ending
            segment.Upper.Should().Be(1048575);
        }

        //TODO rename + explain
        [Fact]
        public void Test3()
        {
            var byteRange = new ByteRange
            {
                Lower = 26_380_013,
                Upper = 32_837_297
            };
            var segments = LogInfo.CalculateSliceAlignedSegments(byteRange);

            //TODO explain
            segments.Should().HaveCount(7);

            //TODO write out rest of tests
        }

    }
}
