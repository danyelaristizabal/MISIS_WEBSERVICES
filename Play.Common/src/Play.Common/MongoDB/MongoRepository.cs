using MongoDB.Driver;
using Play.Common.Service.Entities;
using Play.Common.Service.Repositories;
using System.Linq.Expressions;

namespace Play.Common.MongoDb
{
    public class MongoRepository<T> : IRepository<T> where T : IEntity
    {
        private readonly IMongoCollection<T> dbCollection;
        private readonly FilterDefinitionBuilder<T> filterBuilder = Builders<T>.Filter;
        public MongoRepository(IMongoDatabase mongoDatabase, string collectionName)
        {
            dbCollection = mongoDatabase.GetCollection<T>(collectionName);
        }
        public async Task<IReadOnlyCollection<T>> GetAllListAsync() => await dbCollection.Find(filterBuilder.Empty).ToListAsync();
        public async Task<IReadOnlyCollection<T>> GetAllListAsync(Expression<Func<T, bool>> filter) => await dbCollection.Find(filter).ToListAsync();
        public async Task<T> GetAsync(Guid id)
        {
            FilterDefinition<T> filterDefinition = filterBuilder.Eq(entity => entity.Id, id);
            return await dbCollection.Find(filterDefinition).FirstOrDefaultAsync();
        }
        public async Task<T> GetAsync(Expression<Func<T, bool>> filter) => await dbCollection.Find(filter).FirstOrDefaultAsync();
        public async Task CreateAsync(T entity)
        {
            if (entity is null) throw new ArgumentNullException(nameof(entity));
            await dbCollection.InsertOneAsync(entity);
        }
        public async Task UpdateAsync(T entity)
        {
            if (entity is null) throw new ArgumentNullException(nameof(entity));
            FilterDefinition<T> filterDefinition = filterBuilder.Eq(existingEntity => existingEntity.Id, entity.Id);
            await dbCollection.ReplaceOneAsync(filterDefinition, entity);
        }
        public async Task RemoveAsync(Guid id)
        {
            FilterDefinition<T> filterDefinition = filterBuilder.Eq(entity => entity.Id, id);
            await dbCollection.DeleteOneAsync(filterDefinition);
        }
    }
}
