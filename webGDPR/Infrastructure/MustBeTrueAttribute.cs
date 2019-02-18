using System.ComponentModel.DataAnnotations;

namespace webGDPR.Infrastructure
{
	public class MustBeTrueAttribute : ValidationAttribute
	{
		public override bool IsValid(object value)
		{
			return value is bool && (bool)value;
		}
	}
}
