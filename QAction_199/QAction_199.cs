using System;
using Skyline.DataMiner.Scripting;
using Skyline.DataMiner.Utils.Scte.HexParsing;
using Skyline.DataMiner.Utils.Scte.IndexDatabaseExport;

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
			uint[] fakeSctParameterPids = new uint[]
			{
				Parameter.fakesctehexvalue,
				Parameter.fakesctestream,
				Parameter.fakescteprogram,
				Parameter.fakescteoperationname,
				Parameter.fakescteipaddress,
			};

			object[] values = (object[])protocol.GetParameters(fakeSctParameterPids);

			string hexString = Convert.ToString(values[0]);
			var scteCells = new ScteExporter.ScteMetadata(Convert.ToString(values[1]), Convert.ToString(values[2]), Convert.ToString(values[4]),default,Convert.ToString(values[3]));

			Scte35Event scte = Scte35Event.FromHex(hexString);
			ScteExporter.OffloadToIndexingDatabase(protocol, scte, scteCells);
		}
		catch (Exception ex)
		{
			protocol.Log("QA" + protocol.QActionID + "|" + protocol.GetTriggerParameter() + "|Run|Exception thrown:" + Environment.NewLine + ex, LogType.Error, LogLevel.NoLogging);
		}
	}
}