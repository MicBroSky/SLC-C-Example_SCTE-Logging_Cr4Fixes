namespace Skyline.Protocol.SCTE35
{
	using System;
	using System.Collections.Generic;
	using System.Text;

	using Sewer56.BitStream;
	using Sewer56.BitStream.ByteStreams;

	public class SpliceDescriptor : ScteBase
	{
		private const string STR_ArchiveAllowedFlag = "archive_allowed_flag";
		private const string STR_Components = "components";
		private const string STR_DeliveryNotRestrictedFlag = "delivery_not_restricted_flag";
		private const string STR_DescriptorLength = "descriptor_length";
		private const string STR_DeviceRestrictions = "device_restrictions";
		private const string STR_Identifier = "identifier";
		private const string STR_NoRegionalBlackoutFlag = "no_regional_blackout_flag";
		private const string STR_ProgramSegmentationFlag = "program_segmentation_flag";
		private const string STR_SegmentationDuration = "segmentation_duration";
		private const string STR_SegmentationDurationFlag = "segmentation_duration_flag";
		private const string STR_SegmentationEventCancelIndicator = "segmentation_event_cancel_indicator";
		private const string STR_SegmentationEventId = "segmentation_event_id";
		private const string STR_SegmentationExpected = "segmentation_expected";
		private const string STR_SegmentationNum = "segmentation_num";
		private const string STR_SegmentationTypeId = "segmentation_type_id";
		private const string STR_SegmentationUpid = "segmentation_upid";
		private const string STR_SegmentationUpidLength = "segmentation_upid_length";
		private const string STR_SegmentationUpidType = "segmenation_upid_type";
		private const string STR_SpliceDescriptorTag = "splice_descriptor_tag";
		private const string STR_SubSegmentNum = "sub_segment_num";
		private const string STR_SubSegmentsExpected = "sub_segments_expected";
		private const string STR_WebDeliveryAllowedFlag = "web_delivery_allowed_flag";

		private static readonly Dictionary<byte, string> SegmentationNameMapping = new Dictionary<byte, string>
		{
			{ 0, "Not Indicated" },
			{ 1, "Content Identification" },
			{ 16, "Program Start" },
			{ 17, "Program End" },
			{ 18, "Program Early Termination" },
			{ 19, "Program Breakaway" },
			{ 20, "Program Resumption" },
			{ 21, "Program Runover Planned" },
			{ 22, "Program Runover Unplanned" },
			{ 23, "Program Overlap Start" },
			{ 24, "Program Blackout Override" },
			{ 25, "Program Join" },
			{ 32, "Chapter Start" },
			{ 33, "Chapter End" },
			{ 34, "Break Start" },
			{ 35, "Break End" },
			{ 36, "Opening Credit Start" },
			{ 37, "Opening Credit End" },
			{ 38, "Closing Credit Start" },
			{ 39, "Closing Credit End" },
			{ 48, "Provider Advertisement Start" },
			{ 49, "Provider Advertisement End" },
			{ 50, "Distributor Advertisement Start" },
			{ 51, "Distributor Advertisement End" },
			{ 52, "Provider Placement Opportunity Start" },
			{ 53, "Provider Placement Opportunity End" },
			{ 54, "Distributor Placement Opportunity Start" },
			{ 55, "Distributor Placement Opportunity End" },
			{ 56, "Provider Overlay Placement Opportunity Start" },
			{ 57, "Provider Overlay Placement Opportunity End" },
			{ 58, "Distributor Overlay Placement Opportunity Start" },
			{ 59, "Distributor Overlay Placement Opportunity End" },
			{ 60, "Provider Promo Start" },
			{ 61, "Provider Promo End" },
			{ 62, "Distributor Promo Start" },
			{ 63, "Distributor Promo End" },
			{ 64, "Unscheduled Event Start" },
			{ 65, "Unscheduled Event End" },
			{ 66, "Alternate Content Opportunity Start" },
			{ 67, "Alternate Content Opportunity End" },
			{ 68, "Provider Ad Block Start" },
			{ 69, "Provider Ad Block End" },
			{ 70, "Distributor Ad Block Start" },
			{ 71, "Distributor Ad Block End" },
			{ 80, "Network Start" },
			{ 81, "Network End" },
		};

		#region Constructors

		public SpliceDescriptor(BitStream<ArrayByteStream> reader)
		{
			Reader = reader;
			Initialize();
		}

		#endregion

		#region Public properties

		public bool ArchiveAllowedFlag => GetValueFromField<bool>(STR_ArchiveAllowedFlag);

		public bool DeliveryNotRestrictedFlag => GetValueFromField<bool>(STR_DeliveryNotRestrictedFlag);

		public int DeviceRestrictions => GetValueFromField<int>(STR_DeviceRestrictions);

		public int EventID => GetValueFromField<int>(STR_SegmentationEventId);

		public int Length => GetValueFromField<int>(STR_DescriptorLength);

		public bool NoRegionalBlackoutFlag => GetValueFromField<bool>(STR_NoRegionalBlackoutFlag);

		public long SegmentationDuration => GetValueFromField<long>(STR_SegmentationDuration);

		public bool SegmentationEventCancelIndicator => GetValueFromField<bool>(STR_SegmentationEventCancelIndicator);

		public int SegmentationEventId => GetValueFromField<int>(STR_SegmentationEventId);

		public int SegmentationExpected => GetValueFromField<int>(STR_SegmentationExpected);

		public int SegmentationNum => GetValueFromField<int>(STR_SegmentationNum);

		public byte SegmentationType => GetValueFromField<byte>(STR_SegmentationTypeId);

		public string SegmentationTypeName
		{
			get
			{
				if (SegmentationNameMapping.TryGetValue(SegmentationType, out string name))
				{
					return name;
				}

				return $"Unknown type {SegmentationType}";
			}
		}

		public string SegmentationUpid => GetValueFromField<string>(STR_SegmentationUpid);

		public int SegmentationUpidType => GetValueFromField<byte>(STR_SegmentationUpidType);

		public bool WebDeliveryAllowedFlag => GetValueFromField<bool>(STR_WebDeliveryAllowedFlag);

		#endregion

		#region Methods

		internal override void Initialize()
		{
			var readerTest = Reader;

			Fields = new Dictionary<string, object>
			{
				[STR_SpliceDescriptorTag] = readerTest.Read<byte>(),
				[STR_DescriptorLength] = readerTest.Read<int>(8),
				[STR_Identifier] = readerTest.Read<int>(), // This should be a string with 4 characters and value CUEI
				[STR_SegmentationEventId] = readerTest.Read<int>(),
				[STR_SegmentationEventCancelIndicator] = Convert.ToBoolean(readerTest.ReadBit()),
			};

			readerTest.SeekRelative(0, 7);

			// We should have all the fields loaded up to here; otherwise we got a reader error
			IsLoadedCheck("Couldn't read the header for a splice descriptor");

			if (!GetValueFromField<bool>(STR_SegmentationEventCancelIndicator))
			{
				Fields[STR_ProgramSegmentationFlag] = Convert.ToBoolean(readerTest.ReadBit());
				Fields[STR_SegmentationDurationFlag] = Convert.ToBoolean(readerTest.ReadBit());
				Fields[STR_DeliveryNotRestrictedFlag] = Convert.ToBoolean(readerTest.ReadBit());
				IsLoadedCheck("Couldn't read the segmentation flags");

				if (!GetValueFromField<bool>(STR_DeliveryNotRestrictedFlag))
				{
					Fields[STR_WebDeliveryAllowedFlag] = Convert.ToBoolean(readerTest.ReadBit());
					Fields[STR_NoRegionalBlackoutFlag] = Convert.ToBoolean(readerTest.ReadBit());
					Fields[STR_ArchiveAllowedFlag] = Convert.ToBoolean(readerTest.ReadBit());
					Fields[STR_DeviceRestrictions] = readerTest.Read<int>(2);
				}
				else
				{
					readerTest.SeekRelative(0, 5);
				}

				if (!GetValueFromField<bool>(STR_ProgramSegmentationFlag))
				{
					int compCount = readerTest.Read<int>(8);
					if (compCount < 0)
					{
						throw new FieldReadException("Couldn't read component_count from reader");
					}

					var components = new List<Tuple<int, long>>();
					for (int i = 0; i < compCount; i++)
					{
						int compTag = readerTest.Read<int>(8);

						readerTest.SeekRelative(0, 7);
						long compPTS = readerTest.Read<long>(33);
						components.Add(new Tuple<int, long>(compTag, compPTS));
					}

					Fields[STR_Components] = components.ToArray();
				}

				if (GetValueFromField<bool>(STR_SegmentationDurationFlag))
				{
					Fields[STR_SegmentationDuration] = readerTest.Read<long>(40);
				}

				Fields[STR_SegmentationUpidType] = readerTest.Read<byte>();
				Fields[STR_SegmentationUpidLength] = readerTest.Read<int>(8);
				IsLoadedCheck("Couldn't read the components or the UPIUD header");

				int count = GetValueFromField<int>(STR_SegmentationUpidLength);
				if (count > 0)
				{
					var bytes = new List<byte>();

					for (int i = 0; i < count; i++)
					{
						byte byteValue = (byte)readerTest.Read<long>(8);
						bytes.Add(byteValue);
					}

					byte[] content = bytes.ToArray();

					Fields[STR_SegmentationUpid] = Encoding.UTF8.GetString(content);
				}
				else
				{
					Fields[STR_SegmentationUpid] = String.Empty;
				}

				Fields[STR_SegmentationTypeId] = (byte)readerTest.Read<long>(8);
				Fields[STR_SegmentationNum] = readerTest.Read<int>(8);
				Fields[STR_SegmentationExpected] = readerTest.Read<int>(8);
				IsLoadedCheck("Couldn't read the segmentation type information");

				byte segType = GetValueFromField<byte>(STR_SegmentationTypeId);
				if (segType == 0x34 || segType == 0x36 || segType == 0x38 || segType == 0x3a)
				{
					Fields[STR_SubSegmentNum] = readerTest.Read<int>(8);
					Fields[STR_SubSegmentsExpected] = readerTest.Read<int>(8);
				}
			}

			Reader = readerTest;
		}

		private void IsLoadedCheck(string message)
		{
			if (!IsLoaded)
			{
				throw new FieldReadException(message);
			}
		}

		#endregion
	}
}