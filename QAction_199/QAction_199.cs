using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
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
			string operationID = "-1";
			string name = Convert.ToString(values[1]);
			string program = Convert.ToString(values[2]);
			string operatorName = Convert.ToString(values[3]);
			string ip = Convert.ToString(values[4]);
			int primaryKey = Convert.ToInt32(values[5]);

			Scte35Event scte = Scte35Event.FromHex(hexString);
			ScteExportToElastic.FillScteIntoElastic(protocol, scte, operationID, name, program, operatorName, ip, primaryKey, string.Empty);
		}
		catch (Exception ex)
		{
			protocol.Log("QA" + protocol.QActionID + "|" + protocol.GetTriggerParameter() + "|Run|Exception thrown:" + Environment.NewLine + ex, LogType.Error, LogLevel.NoLogging);
		}
	}
}