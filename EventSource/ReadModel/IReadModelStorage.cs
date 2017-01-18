using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventSource;

namespace Appointments.EventHandlers
{
	public interface IReadModelStorage<T> where T : IReadModel
	{
		Task<IEnumerable<T>> GetAll(int pageIndex, int pageSize);
		Task Save(T model);
		Task Get(Guid id);
	}

}