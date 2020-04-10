using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CardsAPI.Models
{
    public class DbTopic 
    {
        [Key]
        public int Id;
        public string Name;
        public List<DbCard> Cards;
        public string tag;
    }
}