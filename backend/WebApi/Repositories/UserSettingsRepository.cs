using Microsoft.Azure.Cosmos;
using WebApi.Models;

namespace WebApi.Repositories
{
    public class UserSettingsRepository : CosmosDbRepository<UserSettings, UserSettingsDocument>
    {
        public UserSettingsRepository(Container container) : base(container)
        {
        }

        protected override DocumentType DocumentType => DocumentType.UserSettings;

        protected override UserSettingsDocument ToDocument(UserSettings entity)
        {
            return UserSettingsDocument.FromEntity(entity);
        }

        protected override UserSettings ToEntity(UserSettingsDocument document)
        {
            return document.ToEntity();
        }
    }
}
