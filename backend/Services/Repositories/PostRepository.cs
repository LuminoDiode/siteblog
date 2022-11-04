using backend.Repository;

namespace backend.Services.Repositories
{
	public class PostRepository
	{
		protected readonly PostDraftService _postDraftService;
		protected readonly BlogContext _blogContext;

		public PostRepository(
			BlogContext blogContext,
			PostDraftService postDraftService)
		{
			_blogContext = blogContext;
			_postDraftService = postDraftService;
		}
	}
}
