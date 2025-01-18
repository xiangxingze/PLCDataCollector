using System.ComponentModel.DataAnnotations;

namespace PLCDataCollector.Models
{
    public class PLCDevice
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public string Name { get; set; }
        
        [Required]
        public string IPAddress { get; set; }
        
        public int Port { get; set; }
        
        public bool IsEnabled { get; set; } = true;
        
        public string Description { get; set; }

        public string Type { get; set; }
        
        // 可选：添加创建时间和更新时间
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
} 