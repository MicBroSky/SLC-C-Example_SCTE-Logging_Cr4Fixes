namespace QAction_1.SCTE35
{
	using System;
	using System.Collections.Generic;
	using System.Text.RegularExpressions;
	using Newtonsoft.Json;
	using Skyline.DataMiner.Scripting;
	using Skyline.Protocol.SCTE35;
	using Formatting = Newtonsoft.Json.Formatting;

	public class ScteExportToElastic
	{
		/// <summary>
		/// Fills the SCTE information into the elastic database.
		/// </summary>
		/// <param name="protocol">The protocol.</param>
		/// <param name="scte">The scte.</param>
		/// <param name="operationID">The operation identifier.</param>
		/// <param name="name">The name.</param>
		/// <param name="program">The program.</param>
		/// <param name="operatorName">Name of the operator.</param>
		/// <param name="ip">The ip.</param>
		/// <param name="primaryKey">The primary key.</param>
		/// <param name="field1">The field1.</param>
		public static void FillScteIntoElastic(SLProtocol protocol, Scte35Event scte, string operationID, string name, string program, string operatorName, string ip, int primaryKey, string field1)
		{
			ip = IpAddressFormatTest(ip);
			List<object[]> rows = new List<object[]>(scte.Operations.Length);

			foreach (SpliceDescriptor operation in scte.Operations)
			{
				ScteQActionRow elasticRow = new ScteQActionRow
				{
					Scte_key_8000001 = primaryKey,
					Scte_ts_8000002 = scte.Pts,
					Scte_opid_8000003 = operationID,
					Scte_opname_8000004 = operatorName,
					Scte_src_8000005 = operation.SegmentationUpid.Split('_')[0] + ":" + ip,
					Scte_str_8000006 = name,
					Scte_pgm_8000007 = program,
					Scte_obj_8000008 = JsonConvert.SerializeObject(scte, Formatting.None),
					Scte_segevntid_8000009 = operation.EventID,
					Scte_segupid_8000010 = operation.SegmentationUpid,
					Scte_segtypeid_8000011 = operation.SegmentationType,
					Scte_segtypename_8000012 = operation.SegmentationTypeName,
					Scte_fld1_8000013 = field1,
				};
				primaryKey++;
				rows.Add(elasticRow.ToObjectArray());
			}

			protocol.SetParameter(Parameter.lastprimarykey, primaryKey);
			protocol.FillArray(Parameter.Scte.tablePid, rows, NotifyProtocol.SaveOption.Partial);
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
