namespace Skyline.Protocol.SCTE35
{
	using System;
	using System.Runtime.Serialization;

	[Serializable]
	public class InvalidTimeSignalException : Exception
	{
		public InvalidTimeSignalException()
		{
			throw new InvalidTimeSignalException();
		}

		public InvalidTimeSignalException(string message)
			: base(message)
		{
			throw new InvalidTimeSignalException(message);
		}

		public InvalidTimeSignalException(string message, Exception innerException)
			: base(message, innerException)
		{
			// Add any type-specific logic for inner exceptions.
		}

		protected InvalidTimeSignalException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			// here for the proper iSerializaiton pattern
		}
	}
}