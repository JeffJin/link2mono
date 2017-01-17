using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventSource;

namespace Appointments.EventHandlers
{
	public interface IReadModelStoragte<T> where T : IReadModel
	{
		Task<IEnumerable<T>> GetAll(int pageIndex, int pageSize);
		Task Save(IReadModel model);
		Task Get(Guid id);
	}
}