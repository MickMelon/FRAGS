using System.ComponentModel.DataAnnotations;

namespace Frags.Database
{
    public class BaseModel
    {
        [Key]
        public int Id { get; set; }
    }
}