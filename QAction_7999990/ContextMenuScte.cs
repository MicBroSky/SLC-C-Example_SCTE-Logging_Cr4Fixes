namespace Skyline.Protocol.SCTE.ContextMenu
{
	using Skyline.DataMiner.Net;
	using Skyline.DataMiner.Scripting;
	using Skyline.DataMiner.Utils.Table.ContextMenu;

	internal enum Action
	{
		ClearTable = 1,
		DeleteSelectedEvents = 2,
	}

	internal class ContextMenuScte : ContextMenu<Action>
	{
		public ContextMenuScte(SLProtocol protocol, object contextMenuData, int tablePid)
			: base(protocol, contextMenuData, tablePid)
		{
		}

		public override void ProcessContextMenuAction()
		{
			switch (Action)
			{
				case Action.ClearTable:
					string[] primaryKeys = Protocol.GetKeys(Parameter.Scte.tablePid, NotifyProtocol.KeyType.Index);
					if (primaryKeys.Length > 0)
					{
						Protocol.DeleteRow(Parameter.Scte.tablePid, primaryKeys);
					}

					break;

				case Action.DeleteSelectedEvents:
					Protocol.DeleteRow(Parameter.Scte.tablePid, this.Data);
					break;

				default:
					Protocol.Log(
						$"QA{Protocol.QActionID}|ContextMenuscte|Process|Unexpected ContextMenu value '{ActionRaw}'",
						LogType.Error,
						LogLevel.NoLogging);
					break;
			}
		}
	}
}