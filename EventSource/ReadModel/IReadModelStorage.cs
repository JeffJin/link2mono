using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EventSource;

namespace EventSource
{
	public interface IReadModelStorage<T> where T : IReadModel
	{
		Task<List<T>> GetAll(int pageIndex, int pageSize);
		Task Save(T model);
		Task<T> Get(Guid id);
	}

}