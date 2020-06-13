using System;
using System.Collections.Generic;

namespace CardsAPI.Models
{
    public class Topic
    {
        public string Name { get; set; }
        public List<Card> Cards { get; set; }
        public string Tag { get; set; }
    }
}
