using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

using Newtonsoft.Json;

using Sewer56.BitStream;
using Sewer56.BitStream.ByteStreams;
using Skyline.DataMiner.Net.VirtualFunctions;
using Skyline.DataMiner.Scripting;
using Skyline.Protocol.SCTE35;

/// <summary>
/// DataMiner QAction Class: Fake SCTE Event.
/// </summary>
public static class QAction
{
	/// <summary>
	/// The QAction entry point.
	/// </summary>
	/// <param name="protocol">Link with SLProtocol process.</param>
	public static void Run(SLProtocol protocol)
	{
		try
		{
			object[] values = (object[])protocol.GetParameters(new uint[]{Parameter.fakesctehexvalue,Parameter.fakesctestream,Parameter.fakescteprogram,Parameter.fakescteoperationname,Parameter.fakescteipaddress,Parameter.lastprimarykey});

			string hexString = Convert.ToString(values[0]);
			string operationID = "-1";
			string name = Convert.ToString(values[1]);
			string program = Convert.ToString(values[2]);
			string operatorName = Convert.ToString(values[3]);
			string ip = Convert.ToString(values[4]);
			int primaryKey = Convert.ToInt32(values[5]);

			List<object[]> rows = new List<object[]>();

			Scte35Event scte = Scte35Event.FromHex(hexString);
			foreach (SpliceDescriptor operation in scte.Operations)
			{
				if (ip.Contains("."))
				{
					Regex regex = new Regex(@"\d{1,3}(\.\d{1,3}){3}");
					MatchCollection matches = regex.Matches(ip);
					ip = matches[0].Value;
				}
				else
				{
					ip = String.Empty;
				}

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
					Scte_fld1_8000013 = "N/A",
				};
				primaryKey++;
				rows.Add(elasticRow.ToObjectArray());
			}

			protocol.SetParameter(Parameter.lastprimarykey, primaryKey);
			protocol.FillArray(Parameter.Scte.tablePid, rows, NotifyProtocol.SaveOption.Partial);
		}
		catch (Exception ex)
		{
			protocol.Log("QA" + protocol.QActionID + "|" + protocol.GetTriggerParameter() + "|Run|Exception thrown:" + Environment.NewLine + ex, LogType.Error, LogLevel.NoLogging);
		}
	}
}