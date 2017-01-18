using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Appointments.EventHandlers;
using EventSource;

namespace Appointments.EventHandlers
{
	public class InMemeoryStorage<T> : IReadModelStorage<T> where T: IReadModel
	{
		private Dictionary<Guid, T> storage = new Dictionary<Guid, T>();

		public Task Get(Guid id)
		{
			T result;
			if (storage.TryGetValue(id, out result))
			{
				return Task.FromResult(result);
			}
			else
			{
				return null;
			}
		}

		public Task<IEnumerable<T>> GetAll(int pageIndex, int pageSize)
		{
			throw new NotImplementedException();
		}

		public Task Save(T model)
		{
			T result;
			if (storage.TryGetValue(model.Id, out result))
			{
				throw new InvalidOperationException(String.Format("read model with same ID {0} is not allowed", model.Id));
			}
			else
			{
				storage[model.Id] = model;
				return Task.FromResult(true);
			}
		}
	}
}