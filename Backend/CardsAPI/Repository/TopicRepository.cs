using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CardsAPI.ExceptionHandling;
using CardsAPI.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CardsAPI.Repository
{
    public interface ITopicRepository
    {
        Task<bool> AddTopic(TopicUpsertion topicUpsertion, string currentLoggedUser);
        Task<List<Topic>> GetTopics(string emailId);
    }

    public class TopicRepository : ITopicRepository
    {
        private readonly UserDbContext _userDbContext;
        private readonly UserManager<IdentityUser> _userManager;
        
        public TopicRepository(UserManager<IdentityUser> userManager,
                                UserDbContext userDbContext)
        {
            _userManager = userManager;
            _userDbContext = userDbContext;
        }

        public async Task<bool> AddTopic(TopicUpsertion topicUpsertion,string loggedUser)
        {
            try
            {
                var currentUser = _userDbContext.NotesUsers.Where(x => x.Email == loggedUser).Single() as DbUser;
                if (currentUser.Topics == null)
                {
                    currentUser.Topics = new List<DbTopic>();
                }

                var existingTopicCount = currentUser.Topics.Where(x => x.Name == topicUpsertion.Name).Count();

                if (existingTopicCount == 0)
                {
                    DbTopic dbTopic = new DbTopic();
                    dbTopic.Name = topicUpsertion.Name;
                    dbTopic.Tag = topicUpsertion.Tag;

                    if (dbTopic.Cards == null)
                    {
                        dbTopic.Cards = new List<DbCard>();
                    }

                    foreach (Card card in topicUpsertion.Cards)
                    {
                        DbCard dbCard = new DbCard();
                        dbCard.Content = card.Content;
                        dbCard.Index = card.Index;
                        
                        dbTopic.Cards.Add(dbCard);
                    }
                    currentUser.Topics.Add(dbTopic);
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

        public Task<List<Topic>> GetTopics(string emailId)
        {
            try
            {
                var currentUser = _userDbContext.NotesUsers.Where(z=>z.Email == emailId)
                    .Include(x => x.Topics)
                        .ThenInclude(y => y.Cards).SingleOrDefault();





                List<Topic> topicList = new List<Topic>();
                if (currentUser.Topics.Count > 0)
                {
                    foreach (DbTopic dbTopic in currentUser.Topics)
                    {
                        Topic topic = new Topic();
                        topic.Name = dbTopic.Name;
                        topic.Tag = dbTopic.Tag;

                        if (dbTopic.Cards.Count > 0)
                        {
                            topic.Cards = new List<Card>();
                        }
                        else
                        {
                            return null;
                        }

                        foreach (DbCard dbCard in dbTopic.Cards)
                        {
                            Card card = new Card();
                            card.Content = dbCard.Content;
                            card.Index = dbCard.Index;
                            topic.Cards.Add(card);
                        }
                        topicList.Add(topic);
                    }

                    return Task.FromResult(topicList);
                }
                else
                    return null;
            }
            catch(Exception ex)
            {
                throw new LoginException("Failed getting topics");
            }
        }
    }
}
