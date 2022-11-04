using backend.Models.Database;
using backend.Models.Runtime;
using backend.Repository;

namespace backend.Services
{
	public class PostDraftService : DraftService<PostDraft, BlogContext, int>
	{
		protected override DraftServiceSettings _settings => base._settingsProvider.PostDraftServiceSettings;

		public PostDraftService(SettingsProviderService settingsProvider, BlogContext context)
			: base(settingsProvider, context,
				  ctx => ctx.PostDrafts,
				  pst => pst.Id,
				  pst => pst.UpdatedDate,
				  (pst, date) => pst.UpdatedDate = date)
		{

		}
	}
}


