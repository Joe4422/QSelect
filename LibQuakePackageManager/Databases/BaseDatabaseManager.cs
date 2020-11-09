using LibQuakePackageManager.Providers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace LibQuakePackageManager.Databases
{
    public abstract class BaseDatabaseManager<item>
        where item : class, IProviderItem
    {
        #region Variables
        protected string dbFilePath;
        protected List<IProvider<item>> providers;
        #endregion

        #region Properties
        /// <summary>
        /// The items currently present in the database.
        /// </summary>
        public List<item> Items { get; protected set; } = new List<item>();

        /// <summary>
        /// Indicates if the database data has been loaded.
        /// </summary>
        public bool IsLoaded { get; protected set; } = false;
        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new BaseDatabaseManager using the specified database file and providers.
        /// </summary>
        /// <param name="dbFilePath">Path of the database file.</param>
        /// <param name="providers">Providers to read item data from.</param>
        public BaseDatabaseManager(string dbFilePath, List<IProvider<item>> providers)
        {
            // Initialise and perform null argument check
            this.dbFilePath = dbFilePath ?? throw new ArgumentNullException(nameof(dbFilePath));
            this.providers = providers ?? throw new ArgumentNullException(nameof(providers));
        }
        #endregion

        #region Methods
        public item this[string id]
        {
            get
            {
                try
                {
                    return Items.First(x => x.Id == id);
                }
                catch (Exception)
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// Performs an initial load of the database, either from the database file or by refreshing provider data.
        /// </summary>
        public async Task LoadDatabaseAsync()
        {
            if (!File.Exists(dbFilePath))
            {
                await RefreshDatabaseAsync();
            }
            else
            {
                await DeserialiseDatabaseAsync();
            }

            IsLoaded = true;
        }

        /// <summary>
        /// Saves the database to the database file.
        /// </summary>
        public async Task SaveDatabaseAsync()
        {
            await SerialiseDatabaseAsync();
        }

        /// <summary>
        /// Refreshes the data in Items by refreshing data in each provider and merging.
        /// </summary>
        public async Task RefreshDatabaseAsync()
        {
            Items.Clear();

            // Run all provider fetch tasks simultaneously
            List<Task> providerRefreshTasks = new List<Task>();
            foreach (IProvider<item> provider in providers)
            {
                providerRefreshTasks.Add(provider.RefreshAsync());
            }
            await Task.WhenAll(providerRefreshTasks);

            // Merge items together to populate Items.
            MergeItemLists();

            // Serialise resulting list
            await SerialiseDatabaseAsync();
        }

        /// <summary>
        /// Merges all the item lists from the providers together and stores the result in Items.
        /// </summary>
        protected void MergeItemLists()
        {
            // Copy first provider's items into Items
            Items = providers[0].Items.ToList();

            // Merge each subsequent provider's items into Items
            for (int i = 1; i < providers.Count; i++)
            {
                IProvider<item> provider = providers[i];

                foreach (item superior in provider.Items)
                {
                    bool wasMerged = false;

                    for (int j = 0; j < Items.Count; j++)
                    {
                        item inferior = Items[j];

                        if (superior.Id == inferior.Id)
                        {
                            Items[j] = MergeItems(superior, inferior);

                            wasMerged = true;
                        }
                    }

                    if (!wasMerged)
                    {
                        Items.Add(superior);
                    }
                }
            }
        }

        /// <summary>
        /// Merges two items together, with values from superior overriding those of inferior unless the value in superior is null.
        /// </summary>
        /// <param name="superior">The item whose values take precedence.</param>
        /// <param name="inferior">The item whose values are overwritten, unless the corresponding value in superior is null.</param>
        /// <returns>The result of merging the two items.</returns>
        protected abstract item MergeItems(item superior, item inferior);

        /// <summary>
        /// Serialises the database into a JSON file.
        /// </summary>
        protected async Task SerialiseDatabaseAsync()
        {
            await Task.Run(() => File.WriteAllText(dbFilePath, JsonConvert.SerializeObject(Items, Formatting.Indented)));
        }

        /// <summary>
        /// Deserialises the database from a JSON file and stores the result in Items.
        /// </summary>
        protected async Task DeserialiseDatabaseAsync()
        {
            Items = await Task.Run(() => JsonConvert.DeserializeObject<List<item>>(File.ReadAllText(dbFilePath)));
        }
        #endregion
    }
}
