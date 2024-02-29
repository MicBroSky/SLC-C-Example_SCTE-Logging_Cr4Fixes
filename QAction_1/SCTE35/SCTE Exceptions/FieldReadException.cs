namespace Skyline.Protocol.SCTE35
{
	using System;
	using System.Runtime.Serialization;

	[Serializable]
	public class FieldReadException : Exception
	{
		public FieldReadException()
		{
			throw new FieldReadException();
		}

		public FieldReadException(string fieldName)
			: base(fieldName)
		{
			throw new FieldReadException(fieldName);
		}

		public FieldReadException(string message, Exception innerException)
			: base(message, innerException)
		{
			// Add any type-specific logic for inner exceptions.
		}

		protected FieldReadException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
			// here for the proper iSerializaiton pattern
		}
	}
}