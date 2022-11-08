using backend.Models.API.Common;
using backend.Models.Database;

namespace backend.Models.API.User
{
	public class UserInfoResponse : HumanResponse
	{
		public DateTime CreatedDate { get; set; }
		public int Id { get; set; }
		public string? Name { get; set; }
		public int[]? PostsId { get; set; }
		public string UserRole { get; set; } = "user"; // user,admin,moderator

		/// <summary>
		/// Shallow copies CreatedDate,Id,Name,UserRole
		/// </summary>
		/// <param name="usr"></param>
		/// <returns></returns>
		public UserInfoResponse FromUser(backend.Models.Database.User usr)
		{
			return new UserInfoResponse
			{
				CreatedDate = usr.CreatedDate,
				Id = usr.Id,
				Name = usr.Name,
				UserRole = usr.UserRole
			};
		}
	}
}

