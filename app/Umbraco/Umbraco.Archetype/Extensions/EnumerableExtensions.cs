using System.Collections.Generic;
using System.Linq;

namespace Archetype.Extensions
{
	public static class EnumerableExtensions
	{
		public static IEnumerable<IEnumerable<T>> InGroupsOf<T>(this IEnumerable<T> items, int numberOfItemsPerGroup)
		{
			return items.Select((e, i) => new { Item = e, Grouping = (i / numberOfItemsPerGroup) })
				.GroupBy(e => e.Grouping)
				.Select(g => g.Select(x => x.Item));
		}
	}
}
