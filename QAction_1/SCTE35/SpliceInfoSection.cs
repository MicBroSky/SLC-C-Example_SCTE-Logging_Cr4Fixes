namespace Skyline.Protocol.SCTE35
{
	using System;
	using System.Collections.Generic;

	using Sewer56.BitStream;
	using Sewer56.BitStream.ByteStreams;

	public class SpliceInfoSection : ScteBase
	{
		private const string STR_CommandType = "splice_command_type";
		private const string STR_CWIndex = "cw_index";
		private const string STR_EncryptedPacket = "encrypted_packet";
		private const string STR_EncryptionAlgorithm = "encryption_algorithm";
		private const string STR_PrivateIndicator = "private_indicator";
		private const string STR_ProtocolVersion = "protocol_version";
		private const string STR_PTSAdjustment = "pts_adjustment";
		private const string STR_SAPType = "sap_type";
		private const string STR_SectionLength = "section_length";
		private const string STR_SectionSyntaxIndicator = "section_syntax_indicator";
		private const string STR_SpliceCommandLength = "splice_command_length";
		private const string STR_TableId = "table_id";
		private const string STR_Tier = "tier";

		private static readonly Dictionary<byte, string> SpliceCommandTypes = new Dictionary<byte, string>
		{
			{ 0x00, "splice_null" },
			{ 0x01, "reserved" },
			{ 0x02, "reserved" },
			{ 0x03, "reserved" },
			{ 0x04, "splice_schedule" },
			{ 0x05, "splice_insert" },
			{ 0x06, "time_signal" },
			{ 0x07, "bandwidth_reservation" },
			{ 0xff, "private_command" },
		};

		public SpliceInfoSection(BitStream<ArrayByteStream> reader)
		{
			Reader = reader;
			Initialize();
		}

		public bool IsOk
		{
			get
			{
				byte header = (byte)Fields[STR_TableId];

				return IsLoaded && header == 0xfc;
			}
		}

		public int ProtocolVersion => GetValueFromField<int>(STR_ProtocolVersion);

		public long PtsAdjustment
		{
			get
			{
				if (IsOk)
				{
					long pts = (long)Fields[STR_PTSAdjustment];

					return pts;
				}

				return Int64.MinValue;
			}
		}

		public string SpliceCommand
		{
			get
			{
				if (IsOk && SpliceCommandTypes.TryGetValue((byte)Fields[STR_CommandType], out string command))
				{
					return command;
				}

				return String.Empty;
			}
		}

		public int SpliceCommandLength
		{
			get
			{
				if (IsOk)
				{
					int length = (int)Fields[STR_SpliceCommandLength];

					return length;
				}

				return Int32.MinValue;
			}
		}

		public byte TablePid => GetValueFromField<byte>(STR_TableId);

		internal override void Initialize()
		{
			var readerTest = Reader;
			Fields = new Dictionary<string, object>
			{
				[STR_TableId] = readerTest.Read<byte>(),
				[STR_SectionSyntaxIndicator] = Convert.ToBoolean(readerTest.ReadBit()),
				[STR_PrivateIndicator] = Convert.ToBoolean(readerTest.ReadBit()),
				[STR_SAPType] = readerTest.Read<int>(2),
				[STR_SectionLength] = readerTest.Read<int>(12),
				[STR_ProtocolVersion] = readerTest.Read<int>(8),
				[STR_EncryptedPacket] = Convert.ToBoolean(readerTest.ReadBit()),
				[STR_EncryptionAlgorithm] = readerTest.Read<int>(6),
				[STR_PTSAdjustment] = readerTest.Read<long>(33),
				[STR_CWIndex] = readerTest.Read<int>(8),
				[STR_Tier] = readerTest.Read<int>(12),
				[STR_SpliceCommandLength] = readerTest.Read<int>(12),
				[STR_CommandType] = readerTest.Read<byte>(),
			};

			Reader = readerTest;
		}
	}
}