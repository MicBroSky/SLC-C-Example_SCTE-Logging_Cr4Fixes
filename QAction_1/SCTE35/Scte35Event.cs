namespace Skyline.Protocol.SCTE35
{
	using System;
	using System.Collections.Generic;
	using System.Globalization;
	using Sewer56.BitStream;
	using Sewer56.BitStream.ByteStreams;

	public class Scte35Event
	{
		#region Constructors

		private Scte35Event()
		{
		}

		private Scte35Event(SpliceInfoSection spi, List<SpliceDescriptor> descriptors, TimeSignal timeSignal)
		{
			Operations = descriptors.ToArray();
			Pts = timeSignal.Pts;
			SpliceCommand = spi.SpliceCommand;
			TableId = Convert.ToInt32(spi.TableId);
			ProtocolVersion = spi.ProtocolVersion;
		}

		#endregion

		#region Public properties

		public SpliceDescriptor[] Operations { get; private set; }

		public double Pts { get; private set; }

		public string SpliceCommand { get; private set; }

		public int TableId { get; private set; }

		public int ProtocolVersion { get; private set; }

		#endregion

		#region Methods

		/// <summary>
		/// Parses a SCTE Hexadecimal string to get its values.
		/// </summary>
		/// <param name="hex">The SCTE hexadecimal string.</param>
		/// <returns>The SCTE 35 Event values.</returns>
		public static Scte35Event FromHex(string hex)
		{
			var reader = new BitStream<ArrayByteStream>(new ArrayByteStream(HexStringToByteArray(hex)));

			var spi = new SpliceInfoSection(reader);

			if (!spi.IsOk)
			{
				return new Scte35Event();
			}

			if (!"time_signal".Equals(spi.SpliceCommand))
			{
				return new Scte35Event();
			}

			var timeSignal = new TimeSignal(spi.Reader);

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
			var descriptors = new List<SpliceDescriptor>();
			var updatedReader = timeSignal.Reader;

			while (descriptor_length > 10)
			{
				var spd = new SpliceDescriptor(updatedReader);

				descriptors.Add(spd);
				descriptor_length -= spd.Length;
				updatedReader = spd.Reader;
			}

			return new Scte35Event(spi,descriptors,timeSignal);
		}

		/// <summary>
		/// Convert a SCTE Hexadecimals string into a byte array.
		/// </summary>
		/// <param name="hexString">The SCTE hexadecimal string.</param>
		/// <returns> an array of bytes based on the hexadecimal string.</returns>
		private static byte[] HexStringToByteArray(string hexString)
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