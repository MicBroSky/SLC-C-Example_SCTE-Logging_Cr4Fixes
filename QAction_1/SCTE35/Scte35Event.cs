namespace Skyline.Protocol.SCTE35
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;

	using Sewer56.BitStream;
	using Sewer56.BitStream.ByteStreams;

	public class Scte35Event
	{
		#region Fields
		private static SpliceInfoSection spi;
		private static List<SpliceDescriptor> descriptors;
		private static TimeSignal timeSignal;
		#endregion

		#region Constructors
		private Scte35Event()
		{
		}
		#endregion

		#region Public properties
		public SpliceDescriptor[] Operations => descriptors.ToArray();

		public double Pts => timeSignal.Pts;

		public string SpliceCommand => spi.SpliceCommand;

		public int TableId => Convert.ToInt32(spi.TableId);

		public int ProtocolVersion => spi.ProtocolVersion;
		#endregion

		#region Methods
		public static Scte35Event FromBytes(byte[] bytes)
		{
			var reader = new BitStream<ArrayByteStream>(new ArrayByteStream(bytes));

			spi = new SpliceInfoSection(reader);

			if (!spi.IsOk)
			{
				return new Scte35Event();
			}

			if (!"time_signal".Equals(spi.SpliceCommand))
			{
				return new Scte35Event();
			}

			timeSignal = new TimeSignal(spi.Reader);

			if (!timeSignal.IsLoaded)
			{
				return new Scte35Event();
			}

			reader = timeSignal.Reader;
			int descriptor_length = reader.Read<int>(16);
			timeSignal.Reader = reader;

			if (descriptor_length < 0)
			{
				return new Scte35Event();
			}

			// Loop to read the descriptors in this packet
			descriptors = new List<SpliceDescriptor>();
			var updatedReader = timeSignal.Reader;

			while (descriptor_length > 10)
			{
				var spd = new SpliceDescriptor(updatedReader);

				descriptors.Add(spd);
				descriptor_length -= spd.Length;
				updatedReader = spd.Reader;
			}

			return new Scte35Event();
		}

		public static Scte35Event FromHex(string hexString)
		{
			var bytes = HexStringToByteArray(hexString);
			return FromBytes(bytes);
		}

		public static byte[] HexStringToByteArray(string hexString)
		{
			byte[] bytes = new byte[(hexString.Length - 2) / 2];

			for (int i = 2; i < hexString.Length; i += 2)
			{
				string hexByte = hexString.Substring(i, 2);
				bytes[(i / 2) - 1] = Byte.Parse(hexByte, NumberStyles.HexNumber);
			}

			return bytes;
		}
		#endregion
	}
}