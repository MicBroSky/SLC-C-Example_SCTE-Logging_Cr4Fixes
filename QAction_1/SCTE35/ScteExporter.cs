namespace QAction_1.SCTE35
{
	using System;
	using System.Collections.Generic;
	using System.Net;
	using System.Text.RegularExpressions;
	using Newtonsoft.Json;
	using Skyline.DataMiner.Scripting;
	using Skyline.Protocol.SCTE35;
	using Formatting = Newtonsoft.Json.Formatting;

	public class ScteExporter
	{
		public class ScteMetadata
		{
			/// <summary>
			/// The value of the opid (operation ID) field on the SCTE object's structure. This is sometimes not given in a device's SCTE implementation.
			/// </summary>
			public string OperationID { get; set; } = "-1";

			/// <summary>
			/// A representation of the source that the SCTE came from.
			/// </summary>
			public string Name { get; set; }

			/// <summary>
			/// A representation of the device that the SCTE came from. It is usually the DataMiner Element's name.
			/// </summary>
			public string Program { get; set; }

			/// <summary>
			/// A text representation of the operation ID.
			/// </summary>
			public string OperatorName { get; set; }

			/// <summary>
			/// The IP address of the device where the SCTE command came from. It is usually the device IP Address of the element.
			/// </summary>
			public string IP { get; set; }

			/// <summary>
			/// The auto incremented primary key that will be used for the next row of the SCTE table.
			/// </summary>
			public long PrimaryKey { get; set; }

			/// <summary>
			/// An extra field to fill in any additional information.
			/// </summary>
			public string Field1 { get; set; } = string.Empty;
		}

		/// <summary>
		/// Fills the SCTE information into the elastic database.
		/// </summary>
		/// <param name="protocol">The protocol.</param>
		/// <param name="scte">The SCTE information parsed from a hexadecimal string.</param>
		/// <param name="scteMetadata">A class that stores all the additional SCTE cells that can be filled in for the table. </param>
		public static void OffloadToIndexingDatabase(SLProtocol protocol, Scte35Event scte, ScteMetadata scteMetadata)
		{
			scteMetadata.IP = IpAddressFormatTest(scteMetadata.IP);
			List<object[]> rows = new List<object[]>(scte.Operations.Length);

			foreach (SpliceDescriptor operation in scte.Operations)
			{
				ScteQActionRow elasticRow = new ScteQActionRow
				{
					Scte_key_8000001 = scteMetadata.PrimaryKey,
					Scte_ts_8000002 = scte.Pts,
					Scte_opid_8000003 = scteMetadata.OperationID,
					Scte_opname_8000004 = scteMetadata.OperatorName,
					Scte_src_8000005 = operation.SegmentationUpid.Split('_')[0] + ":" + scteMetadata.IP,
					Scte_str_8000006 = scteMetadata.Name,
					Scte_pgm_8000007 = scteMetadata.Program,
					Scte_obj_8000008 = JsonConvert.SerializeObject(scte, Formatting.None),
					Scte_segevntid_8000009 = operation.EventID,
					Scte_segupid_8000010 = operation.SegmentationUpid,
					Scte_segtypeid_8000011 = operation.SegmentationType,
					Scte_segtypename_8000012 = operation.SegmentationTypeName,
					Scte_fld1_8000013 = scteMetadata.Field1,
				};
				scteMetadata.PrimaryKey++;
				rows.Add(elasticRow.ToObjectArray());
			}

			protocol.SetParameter(Parameter.lastprimarykey, scteMetadata.PrimaryKey);
			protocol.FillArray(Parameter.Scte.tablePid, rows);
		}

		/// <summary>
		/// Tests the IP Address to make sure it's in the right format. Designed this way to get the right IP address if you're using a parameter with type IP.
		/// </summary>
		/// <param name="ip">The IP value</param>
		/// <returns>Either the proper IP Address or an empty string.</returns>
		private static string IpAddressFormatTest(string ip)
		{
			if (ip.Contains("."))
			{
				Regex regex = new Regex(@"\d{1,3}(\.\d{1,3}){3}");
				MatchCollection matches = regex.Matches(ip);
				return matches[0].Value;
			}
			else
			{
				return String.Empty;
			}
		}
	}
}
