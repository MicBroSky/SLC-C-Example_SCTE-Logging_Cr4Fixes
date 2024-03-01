using System;
using System.Collections.Generic;

using Sewer56.BitStream;
using Sewer56.BitStream.ByteStreams;

using Skyline.Protocol.SCTE35;

public class TimeSignal : ScteBase
{
	private const string STR_PTS = "pts_time";
	private bool hasPts;

	public TimeSignal(BitStream<ArrayByteStream> reader)
	{
		this.Reader = reader;
		Initialize();
	}

	public bool HasPts => hasPts;

	public double Pts
	{
		get
		{
			if (hasPts)
			{
				long pts = (long)Fields[STR_PTS];
				return Convert.ToDouble(pts);
			}

			return long.MinValue;
		}
	}

	internal override void Initialize()
	{
		Fields = new Dictionary<string, object>();
		var readerTest = Reader;
		bool timeflag = Convert.ToBoolean(readerTest.ReadBit());

		if (timeflag)
		{
			hasPts = true;
			readerTest.SeekRelative(0, 6);
			Fields[STR_PTS] = readerTest.Read<long>(33) / 90000;
		}
		else
		{
			hasPts = false;
			readerTest.SeekRelative(0, 7);
		}

		Reader = readerTest;
	}
}
