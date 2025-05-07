using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;
// using AXO.Core.Models.Guide;
// using AXO.Core.Models.Library;
using Microsoft.AspNetCore.Identity;
using MudBlazor;
using static MudBlazor.Colors;

namespace AXO.Core.Models
{
	public class Employee : IdentityUser<Guid>
	{
		[Key]
		public Guid Id { get; set; }
		
		public string Email { get; set; }
		
		public string PasswordHash { get; set; }
		
		public string UserName { get; set; }
		
		public bool? IsCollectiveAccount { get; set; } = false;

		/*** Переопределено для MudSelect multiple ***/
		// public override bool Equals(object o)
		// {
		// 	var other = o as Employee;
		// 	// return other?.Id == Id;
		// }
		
		public override bool Equals(object obj)
		{
			if (obj is Employee other)
			{
				return Id == other.Id;
			}
			return false;
		}

		public override int GetHashCode() => Id.GetHashCode();


		public Employee()
		{
			Orders = new ObservableCollection<Order>();
			Orders.CollectionChanged += (s, e) => OrdersChanged?.Invoke(Orders);
		}

		// public override int GetHashCode() => Id.GetHashCode();

		public override string ToString() => Name;

		/******************************************/

		public string ServiceNumber { get; set; }

		public Guid? PostId { get; set; }

		// public virtual GuidePost Post { get; set; }

		public Guid? DivisionId { get; set; }

		// public virtual GuideDivision Division { get; set; }

		public DateTime? DateOfBirth { get; set; }

		public DateTime? DateOfEmployment { get; set; }

		public Guid? GenderId { get; set; }

		// public virtual Gender Gender { get; set; }

		[Required]
		public string Name { get; set; }

		public string Photo { get; set; }

		public Guid? CooperationId { get; set; } = null;

		// public virtual Cooperation Cooperation { get; set; }

		public DateTime Created { get; set; }

		// public virtual ICollection<IdentityUserClaim<Guid>> Claims { get; set; }

		// public virtual ICollection<IdentityUserLogin<Guid>> Logins { get; set; }

		// public virtual ICollection<IdentityUserToken<Guid>> Tokens { get; set; }

		// public virtual ICollection<EmployeePermission> EmployeePermissions { get; set; }

		// public virtual IEnumerable<Order> Orders { get; set; }
		
		public event Action<ObservableCollection<Order>> OrdersChanged;
		
		public virtual ObservableCollection<Order> Orders { get; set; }


		// public virtual IEnumerable<OrderEmployee> OrderEmployees { get; set; }

		[NotMapped]
		public string ShortName
		{
			get
			{
				string[] str = Name?.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
				if (str?.Length == 3)
					return string.Format(CultureInfo.CurrentCulture, "{0} {1}. {2}.", str[0], str[1][0], str[2][0]);
				if (str?.Length == 2)
					return string.Format(CultureInfo.CurrentCulture, "{0} {1}.", str[0], str[1][0]);
				return Name;
			}
		}
	}
}
