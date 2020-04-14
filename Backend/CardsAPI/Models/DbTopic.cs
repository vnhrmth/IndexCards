using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CardsAPI.Models
{
    public class DbTopic 
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; }
        public List<DbCard> Cards { get; set; }
        public string Tag { get; set; }
    }
}