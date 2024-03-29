namespace QAction_1.SCTE35
{
	using System;
	using System.Collections.Generic;
	using System.Text.RegularExpressions;
	using Newtonsoft.Json;
	using Skyline.DataMiner.Scripting;
	using Skyline.Protocol.SCTE35;
	using Formatting = Newtonsoft.Json.Formatting;

	public class ScteExporter
	{

		public class AdditionalScteCells
		{
			public string OperationID { get; set; } = "-1";

			public string Name { get; set; }

			public string Program { get; set; }

			public string OperatorName { get; set; }

			public string IP { get; set; }

			public long PrimaryKey { get; set; }

			public string Field1 { get; set; } = string.Empty;
		}

		/// <summary>
		/// Fills the SCTE information into the elastic database.
		/// </summary>
		/// <param name="protocol">The protocol.</param>
		/// <param name="scte">The scte.</param>
		/// <param name="scteCells">A class that storse all the additional SCTE cells that can be filled in for the table.</param>
		public static void OffloadingToIndexingDatabase(SLProtocol protocol, Scte35Event scte, AdditionalScteCells scteCells)
		{
			scteCells.IP = IpAddressFormatTest(scteCells.IP);
			List<object[]> rows = new List<object[]>(scte.Operations.Length);

			foreach (SpliceDescriptor operation in scte.Operations)
			{
				ScteQActionRow elasticRow = new ScteQActionRow
				{
					Scte_key_8000001 = scteCells.PrimaryKey,
					Scte_ts_8000002 = scte.Pts,
					Scte_opid_8000003 = scteCells.OperationID,
					Scte_opname_8000004 = scteCells.OperatorName,
					Scte_src_8000005 = operation.SegmentationUpid.Split('_')[0] + ":" + scteCells.IP,
					Scte_str_8000006 = scteCells.Name,
					Scte_pgm_8000007 = scteCells.Program,
					Scte_obj_8000008 = JsonConvert.SerializeObject(scte, Formatting.None),
					Scte_segevntid_8000009 = operation.EventID,
					Scte_segupid_8000010 = operation.SegmentationUpid,
					Scte_segtypeid_8000011 = operation.SegmentationType,
					Scte_segtypename_8000012 = operation.SegmentationTypeName,
					Scte_fld1_8000013 = scteCells.Field1,
				};
				scteCells.PrimaryKey++;
				rows.Add(elasticRow.ToObjectArray());
			}

			protocol.SetParameter(Parameter.lastprimarykey, scteCells.PrimaryKey);
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
