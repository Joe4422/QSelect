using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace LibQuakePackageManager.Providers
{
    public interface IProvider<T>
    {
        /// <summary>
        /// Contains all items provided by the provider.
        /// </summary>
        List<T> Items { get; }

        /// <summary>
        /// Refreshes the data provided by the provider.
        /// </summary>
        Task RefreshAsync();

        /// <summary>
        /// Fetches an item using the specified ID.
        /// </summary>
        /// <param name="id">ID of the item to fetch.</param>
        /// <returns>The item matching the ID, or null if none exists.</returns>
        T GetItem(string id);
    }
}
