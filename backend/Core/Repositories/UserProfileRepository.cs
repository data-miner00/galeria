using Microsoft.Azure.Cosmos;
using Core.Models;

namespace Core.Repositories
{
    public class UserProfileRepository : CosmosDbRepository<UserProfile, UserProfileDocument>
    {
        public UserProfileRepository(Container container) : base(container)
        {
        }

        protected override DocumentType DocumentType => DocumentType.UserProfile;

        protected override UserProfileDocument ToDocument(UserProfile entity)
        {
            return UserProfileDocument.FromEntity(entity);
        }

        protected override UserProfile ToEntity(UserProfileDocument document)
        {
            return document.ToEntity();
        }
    }
}
