using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CardsAPI.Models;

namespace CardsAPI.Repository
{
    public interface ITopicRepository
    {
        Task<bool> AddTopic(TopicUpsertion topicUpsertion);
    }

    public class TopicRepository : ITopicRepository
    {
        private readonly UserDbContext _userDbContext;

        public TopicRepository(UserDbContext userDbContext)
        {
            _userDbContext = userDbContext;
        }

        public async Task<bool> AddTopic(TopicUpsertion topicUpsertion)
        {
            try
            {
                var existingTopicCount = _userDbContext.Topics.Where(x => x.Name == topicUpsertion.Name).Count();

                if (existingTopicCount == 0)
                { 
                    DbTopic dbTopic = new DbTopic();
                    dbTopic.Name = topicUpsertion.Name;

                    if(dbTopic.Cards == null)
                    {
                        dbTopic.Cards = new List<DbCard>();
                    }

                    foreach(Card card in topicUpsertion.Cards)
                    {
                        DbCard dbCard = new DbCard();
                        dbCard.Content = card.Content;
                        dbCard.Index = card.Index;
                        dbTopic.Cards.Add(dbCard);
                    }
                    _userDbContext.Add(dbTopic);                
                    await _userDbContext.SaveChangesAsync();
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
