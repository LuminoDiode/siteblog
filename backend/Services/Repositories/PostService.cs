using backend.Models.API.Requests;
using backend.Models.Database;
using backend.Models.Runtime;
using backend.Repository;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using static System.Net.Mime.MediaTypeNames;

namespace backend.Services.Repositories
{
	public class PostService
	{
		protected readonly PostDraftService _postDraftService;
		protected readonly BlogContext _blogContext;
		protected readonly SettingsProviderService _settingsProviderService;
		protected virtual PostServiceSettings _settings => _settingsProviderService.PostServiceSettings;

		public PostConstraints PostConstraints => _settings._postConstraints;

		public PostService(
			SettingsProviderService settingsProviderService,
			BlogContext blogContext,
			PostDraftService postDraftService)
		{
			_blogContext = blogContext;
			_postDraftService = postDraftService;
			_settingsProviderService = settingsProviderService;
		}

		public IQueryable<Post> GetPosts()
		{
			return _blogContext.Posts.AsQueryable();
		}
		public async Task<Post?> TryFindAsync(int Id)
		{
			return await _blogContext.Posts.SingleOrDefaultAsync(x => x.Id.Equals(Id));
		}

		public IQueryable<Post> SearchFulltext(string query)
		{
			var queryVector = EF.Functions.WebSearchToTsQuery(query);
			return _blogContext.Posts.OrderByDescending(p => p.TitleVector.Rank(queryVector)).ThenByDescending(p => p.TextVector.Rank(queryVector));
		}

		protected int TrimedLength(string s)
		{
			int spaces = 0;
			for (int i = 0; i < s.Length; i++)
			{
				if (char.IsWhiteSpace(s[i])) spaces++;
			}
			for (int i = s.Length; i >= 0; i--)
			{
				if (char.IsWhiteSpace(s[i])) spaces++;
			}

			return Math.Max(0, s.Length - spaces);
		}
		public bool ValidateAndProcessPost(AddPostRequest newPost, out AddPostRequest? processed)
		{
			processed = null;
			int tl;

			if (_settings._postProcessingSettings.trimTitle)
			{
				tl = TrimedLength(newPost.Title);
				if (tl < _settings._postConstraints.titleMinLen
					|| tl > _settings._postConstraints.titleMaxLen)
					return false;
			}
			else
			{
				if (newPost.Title.Length < _settings._postConstraints.titleMinLen
					|| newPost.Title.Length > _settings._postConstraints.titleMinLen)
					return false;
			}

			if (_settings._postProcessingSettings.trimSubtitle)
			{
				tl = TrimedLength(newPost.Subtitle);
				if (tl < _settings._postConstraints.subtitleMinLen
					|| tl > _settings._postConstraints.subtitleMaxLen)
					return false;
			}
			else
			{
				if (newPost.Subtitle.Length < _settings._postConstraints.subtitleMinLen
					|| newPost.Subtitle.Length > _settings._postConstraints.subtitleMaxLen)
					return false;
			}

			if (_settings._postProcessingSettings.trimMainText)
			{
				tl = TrimedLength(newPost.HtmlText);
				if (tl < _settings._postConstraints.mainTextMinLen
					|| tl > _settings._postConstraints.mainTextMaxLen)
					return false;
			}
			else
			{
				if (newPost.HtmlText.Length < _settings._postConstraints.mainTextMinLen
					|| newPost.HtmlText.Length > _settings._postConstraints.mainTextMaxLen)
					return false;
			}

			processed = new()
			{
				Title = _settings._postProcessingSettings.trimTitle ? newPost.Title.Trim() : newPost.Title,
				Subtitle = _settings._postProcessingSettings.trimSubtitle ? newPost.Subtitle.Trim() : newPost.Subtitle,
				HtmlText = _settings._postProcessingSettings.trimSubtitle ? newPost.HtmlText.Trim() : newPost.HtmlText,
			};

			return true;
		}

		public async Task<EntityEntry<Post>> AddPostAsync(AddPostRequest newPost, int? ownerUserId = null)
		{
			Post toDb = new(newPost.Title,newPost.HtmlText)
			{
				CreatedDate = DateTime.UtcNow,
				UpdatedDate = DateTime.UtcNow,
				TopImageId = newPost.topImageId,
				OwnerUserId = ownerUserId,
				//Title = newPost.Title,
				//TextHTML = newPost.HtmlText,
				Subtile = newPost.Subtitle
			};

			var entry  = this._blogContext.Posts.Add(toDb);
			await this._blogContext.SaveChangesAsync();
			return entry;
		}

		public async Task<EntityEntry<Post>?> PatchPostIfExistsAsync(EditPostRequest editedPost, int? ownerUserId = null)
		{
			var found = await _blogContext.Posts.FindAsync(editedPost.Id);
			if (found is null) return null;

			var entry = _blogContext.Entry(found);

			entry.CurrentValues.SetValues(editedPost);
			entry.Entity.OwnerUserId = ownerUserId;
			await _blogContext.SaveChangesAsync();

			return entry;
		}
	}
}
