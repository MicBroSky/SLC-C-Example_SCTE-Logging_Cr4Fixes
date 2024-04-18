using System;
using QAction_1.SCTE35;
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
			object[] values = (object[])protocol.GetParameters(new uint[] { Parameter.fakesctehexvalue, Parameter.fakesctestream, Parameter.fakescteprogram, Parameter.fakescteoperationname, Parameter.fakescteipaddress, Parameter.lastprimarykey });

			string hexString = Convert.ToString(values[0]);
			var scteCells = new ScteExporter.ScteMetadata
			{
				Name = Convert.ToString(values[1]),
				Program = Convert.ToString(values[2]),
				OperatorName = Convert.ToString(values[3]),
				IP = Convert.ToString(values[4]),
				PrimaryKey = Convert.ToInt64(values[5]),
			};

			Scte35Event scte = Scte35Event.FromHex(hexString);
			ScteExporter.OffloadToIndexingDatabase(protocol, scte, scteCells);
		}
		catch (Exception ex)
		{
			protocol.Log("QA" + protocol.QActionID + "|" + protocol.GetTriggerParameter() + "|Run|Exception thrown:" + Environment.NewLine + ex, LogType.Error, LogLevel.NoLogging);
		}
	}
}