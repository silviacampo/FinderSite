using System;

namespace webGDPR.Models
{
	// Examples: sabiendo el peso, y el maximo peso de la categoria del correo, puedo saber si se pueden combinar en el mismo envio o si es otro envio   
	//	bundle base + collar + subscription monthly service (1 => 7, 2=>5 , 3...=> 3),
	//	additional collar,
	//	base replacement
	//	batteries supply or refer to a place to buy them

	public class Product
	{
		public string ProductId { get; set; }
		public string Model { get; set; }
		public string Name { get; set; }
		public string Text { get; set; }
		public string Description { get; set; } 
		public string Type { get; set; } //good (one shoot payment) - service (recurrent payments)
		public string Category { get; set; }
		public decimal Price { get; set; }
		public float? Weight { get; set; }
		public float? Width { get; set; }
		public float? Height { get; set; }
		public float? Length { get; set; }

		public int? QuantityInStock { get; set; }

		public DateTime CreationDate { get; set; }
		public bool IsActive { get; set; }
		public bool Deleted { get; set; }
	}
}
