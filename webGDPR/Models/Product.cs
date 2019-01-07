using System;

namespace webGDPR.Models
{
	public class Product
	{
		public string ProductId { get; set; }
		public string Model { get; set; }
		public string Name { get; set; }
		public string Text { get; set; }
		public string Description { get; set; }
		public string Category { get; set; }
		public decimal Price { get; set; }
		public float Weight { get; set; }
		public float Width { get; set; }
		public float Height { get; set; }
		public float Deep { get; set; }

		public DateTime CreationDate { get; set; }

		public bool Deleted { get; set; }
	}
}
