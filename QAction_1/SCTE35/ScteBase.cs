namespace Skyline.Protocol.SCTE35
{
	using System.Collections.Generic;

	using Sewer56.BitStream;
	using Sewer56.BitStream.ByteStreams;

	public abstract class ScteBase
	{
		public bool IsLoaded
		{
			get
			{
				foreach (string key in Fields.Keys)
				{
					object obj = Fields[key];

					bool isUnexpectedType = !(obj is bool || obj is byte || obj is int || obj is long || obj is string);
					if (isUnexpectedType)
					{
						return false;
					}
				}

				return true;
			}
		}

		internal BitStream<ArrayByteStream> Reader { get; set; }

		internal Dictionary<string, object> Fields { get; set; }

		public T GetValueFromField<T>(string fieldName)
		{
			if (Fields.TryGetValue(fieldName, out object obj) && !(obj is null))
			{
				return (T)obj;
			}

			return default;
		}

		public bool TryGetValueFromField<T>(string fieldName, out T result)
		{
			if (Fields.TryGetValue(fieldName, out object obj) && !(obj is null))
			{
				result = (T)obj;

				return true;
			}

			result = default;

			return false;
		}

		internal abstract void Initialize();
	}
}