using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Dashboard.DAL.Models
{
    public class News
    {
        [Key]
        public int Id { get; set; }

        public string? Title { get; set; }

        public string? Content { get; set; }

        public DateTime PublishedDate { get; set; }

        // Зовнішній ключ для категорії
        public int CategoryId { get; set; }

        // Навігаційна властивість для категорії
        [ForeignKey("CategoryId")]
        public required Category Category { get; set; }
    }
}
