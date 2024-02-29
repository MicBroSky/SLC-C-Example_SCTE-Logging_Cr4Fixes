namespace Skyline.Protocol.SCTE35
{
	using System;
	using System.Runtime.Serialization;

	[Serializable]
	public class SpliceCommandNotSupportedException : Exception
	{
		public SpliceCommandNotSupportedException()
		{
			throw new InvalidTimeSignalException();
		}

		public SpliceCommandNotSupportedException(string spliceCommand)
			: base(spliceCommand)
		{
			throw new SpliceCommandNotSupportedException(spliceCommand);
		}

		public SpliceCommandNotSupportedException(string message, Exception innerException)
			: base(message, innerException)
		{
			// Add any type-specific logic for inner exceptions.
		}

		protected SpliceCommandNotSupportedException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			// here for the proper iSerializaiton pattern
		}
	}
}