using System;
using System.Threading.Tasks;
using Common.PersistentManager;

namespace Sample
{
	public class PersistentManager : PersistentManagerBase
	{
		public static string Key = "sample";

		public override string PersistentKey => Key;

		public override Task<bool> Download<T>(T data)
		{
			throw new NotImplementedException();
		}
	}
}