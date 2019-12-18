using System;
using System.Buffers;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace JT808.Gateway.Test
{
    public class PipeTest
    {
        [Fact]
        public void Test1()
        {
            var reader = new ReadOnlySequence<byte>(new byte[] { 0x7E, 0, 1, 2, 0x7E});
            SequenceReader<byte> seqReader = new SequenceReader<byte>(reader);
            int index = 0;
            byte mark = 0;
            long totalConsumed = 0;
            List<byte[]> packages = new List<byte[]>();
            while (!seqReader.End)
            {
                if (seqReader.IsNext(0x7E, advancePast: true))
                {
                    if (mark == 1)
                    {
                        var package = seqReader.Sequence.Slice(totalConsumed, seqReader.Consumed - totalConsumed).ToArray();
                        packages.Add(package);
                        totalConsumed += (seqReader.Consumed - totalConsumed);
                        index++;
                        if (seqReader.End) break;
                        seqReader.Advance(1);
                        mark = 0;
                    }
                    mark++;
                }
                else
                {
                    seqReader.Advance(1);
                }
                index++;
            }
            Assert.Equal(5, index);
            Assert.Single(packages);
            Assert.Equal(5, seqReader.Consumed);
        }

        [Fact]
        public void Test2()
        {
            var reader = new ReadOnlySequence<byte>(new byte[] { 0x7E, 0, 1, 2, 0x7E, 0x7E, 0, 1, 0x7E, 0x7E, 2, 2, 2 });
            SequenceReader<byte> seqReader = new SequenceReader<byte>(reader);
            int index = 0;
            byte mark = 0;
            long totalConsumed = 0;
            List<byte[]> packages = new List<byte[]>();
            while (!seqReader.End)
            {
                if (seqReader.IsNext(0x7E, advancePast: true))
                {
                    if (mark == 1)
                    {
                        var package = seqReader.Sequence.Slice(totalConsumed, seqReader.Consumed - totalConsumed).ToArray();
                        packages.Add(package);
                        totalConsumed += (seqReader.Consumed - totalConsumed);
                        index++;
                        if (seqReader.End) break;
                        seqReader.Advance(1);
                        mark = 0;
                    }
                    mark++;
                }
                else
                {
                    seqReader.Advance(1);
                }
                index++;
            }
            Assert.Equal(13, index);
            Assert.Equal(2,packages.Count);
            Assert.Equal(9, totalConsumed);
            Assert.Equal(13, seqReader.Consumed);
        }

        [Fact]
        public void Test3()
        {
            Assert.Throws<Exception>(() => 
            {
                var reader = new ReadOnlySequence<byte>(new byte[] { 0, 1, 2, 0x7E });
                SequenceReader<byte> seqReader = new SequenceReader<byte>(reader);
                if (seqReader.TryPeek(out byte beginMark))
                {
                    if (beginMark != 0x7E) throw new ArgumentException("not 808 packages");
                }
            });
        }
    }
}
