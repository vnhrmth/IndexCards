using System.ComponentModel.DataAnnotations;

namespace CardsAPI.Models
{
    public class DbCard
    {
        [Key]
        public int Id { get; set; }
        [Required, StringLength(256)]//TODO check if it is feasible.
        public string Content { get; set; }
        public int Index { get; set; }
    }
}