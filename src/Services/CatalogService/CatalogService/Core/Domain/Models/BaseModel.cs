﻿namespace CatalogService.Core.Domain.Models
{
    public abstract class BaseModel
    {
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public DateTime ModifiedDate { get; set; }
        public bool IsDeleted { get; set; }
    }
}
