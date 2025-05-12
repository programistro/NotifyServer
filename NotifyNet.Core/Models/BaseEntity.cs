using System.ComponentModel.DataAnnotations;

namespace NotifyNet.Core.Models
{
	public class BaseEntity
	{
		public Guid Id { get; set; }

		[MaxLength(256)]
		public string Name { get; set; }

		public DateTime? Created { get; set; }

		public DateTime? Updated { get; set; }

		public string Description { get; set; }
	}
}
