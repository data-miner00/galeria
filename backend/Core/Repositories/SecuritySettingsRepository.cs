using Microsoft.Azure.Cosmos;
using Core.Models;

namespace Core.Repositories
{
    public class SecuritySettingsRepository : CosmosDbRepository<SecuritySettings, SecuritySettingsDocument>
    {
        public SecuritySettingsRepository(Container container) : base(container)
        {
        }

        protected override DocumentType DocumentType => DocumentType.SecuritySettings;

        protected override SecuritySettingsDocument ToDocument(SecuritySettings entity)
        {
            return SecuritySettingsDocument.From(entity);
        }

        protected override SecuritySettings ToEntity(SecuritySettingsDocument document)
        {
            return document.ToEntity();
        }
    }
}
