namespace Core.Data.Contracts
{
    /// <summary>
    /// Provides method signatures for a seeding and checking if a data store exists
    /// </summary>
    public interface IContext
    {
        /// <summary>
        /// When implemented, seeds the target data store
        /// </summary>
        void Seed();
        /// <summary>
        /// When implemented, checks to make sure that the database actually does exist
        /// </summary>
        /// <returns>True if the database exists. Otherwise, false</returns>
        bool DatabaseExists();
    }
}
