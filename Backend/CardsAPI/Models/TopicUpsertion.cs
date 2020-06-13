using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CardsAPI.Models
{
    public class TopicUpsertion
    {
        [Required(ErrorMessage = "Topic Name required")]
        public string Name { get; set; }
        public List<Card> Cards { get; set; }
        public string Tag { get; set; }
    }
}
