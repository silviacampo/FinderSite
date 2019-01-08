using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace webGDPR.ViewModels.ShoppingCart
{
	//[Validator(typeof(WishlistEmailAFriendValidator))]
	public partial class WishlistEmailAFriendModel
	{
		//[AllowHtml]
		public string FriendEmail { get; set; }

		//[AllowHtml]
		public string YourEmailAddress { get; set; }

		//[AllowHtml]
		public string PersonalMessage { get; set; }

		public bool SuccessfullySent { get; set; }
		public string Result { get; set; }

		public bool DisplayCaptcha { get; set; }
	}

	//public class WishlistEmailAFriendValidator : AbstractValidator<WishlistEmailAFriendModel>
	//{
	//	public WishlistEmailAFriendValidator()
	//	{
	//		RuleFor(x => x.FriendEmail).NotEmpty().EmailAddress();
	//		RuleFor(x => x.YourEmailAddress).NotEmpty().EmailAddress();
	//	}
	//}
}
