
using System.Text.Json.Serialization;

namespace NotifyNet.Core.Models
{
	public class Order : BaseEntity
	{
		public int? OrderNumber { get; set; }

		/* Блок "Формирования записи" */
		public Guid? EmployeeApplicantId { get; set; }
		
		[JsonIgnore]
		public Employee? Employee { get; set; }

		// public virtual Employee EmployeeApplicant { get; set; }

		public Guid? BuildingId { get; set; }

		// public virtual GuideBuilding Building { get; set; }

		public Guid? DivisionId { get; set; }

		// public virtual GuideDivision Division { get; set; }

		public Guid? EquipmentId { get; set; }

		// public virtual GuideEquipment Equipment { get; set; }

		// public Guid? SupportId { get; set; }

		// public virtual Support Support { get; set; }

		public Guid? EventId { get; set; }

		// public virtual Event Event { get; set; } = new ();

		public Guid? ProcessId { get; set; }

		// public virtual Process Process { get; set; } = new ();

		public Guid? RecordId { get; set; }

		// public virtual Record Record { get; set; }

		/* Блок "Модерация" */
		public Guid? EmployeeDispatcherId { get; set; }

		// public virtual Employee EmployeeDispatcher { get; set; }

		public string? DescriptionDispathcer { get; set; }

		public Guid? EmployeeNotificationId { get; set; } // Ответственный руководитель (владелец процесса)

		// public virtual Employee EmployeeNotification { get; set; }

		public Guid? PriorityId { get; set; }

		// public virtual Priority Priority { get; set; }

		public DateTime? DateModeration { get; set; }

		/* Блок "Задача" */
		public Guid? EmployeeExecuterId { get; set; }

		// public virtual Employee EmployeeExecuter { get; set; } // Ответственный исполнитель

		public DateTime? DateOfExecution { get; set; }

		/* Блок "Исполнение" */
		public string? DescriptionOfWork { get; set; }

		public DateTime? DateWorkStatus { get; set; }

		/* Проверка и закрытие */
		public DateTime? DateOfClose { get; set; }

		// public virtual IEnumerable<Employee> Employees { get; set; }
		//
		// public virtual IEnumerable<OrderEmployee> OrderEmployees { get; set; }
		//
		// [NotMapped]
		// public IEnumerable<Employee> EmployeeExecuters { get; set; }
		//
		// /* Чат */
		// public virtual List<OrderChat> OrderChats { get; set; } = new List<OrderChat>();
	}
}
