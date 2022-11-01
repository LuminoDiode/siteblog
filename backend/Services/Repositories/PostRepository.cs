using backend.Repository;

namespace backend.Services.Repositories
{
	public class PostRepository
	{
		protected readonly PasswordsCryptographyService _passwordsCryptographyService;
		//protected readonly JwtService _jwtService;
		protected readonly EmailConfirmationService _emailConfirmationService;
		protected readonly BlogContext _blogContext;

		public PostRepository(
			BlogContext blogContext,
			PasswordsCryptographyService passwordsCryptographyService,
			EmailConfirmationService emailService)
		{
			_passwordsCryptographyService = passwordsCryptographyService;
			//_jwtService = jwtService;
			_emailConfirmationService = emailService;
			_blogContext = blogContext;

			throw new NotImplementedException();
		}
	}
}
