namespace Skyline.Protocol.SCTE35
{
	using System;
	using System.Runtime.Serialization;

	[Serializable]
	public class InvalidSpliceSectionException : Exception
	{
		public InvalidSpliceSectionException()
		{
			throw new InvalidSpliceSectionException();
		}

		public InvalidSpliceSectionException(string message)
			: base(message)
		{
			throw new InvalidSpliceSectionException(message);
		}

		public InvalidSpliceSectionException(string message, Exception innerException)
			: base(message, innerException)
		{
			// Add any type-specific logic for inner exceptions.
		}

		protected InvalidSpliceSectionException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			// here for the proper iSerializaiton pattern
		}
	}
}