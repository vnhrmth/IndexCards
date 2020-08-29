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
        Task<bool> DeleteTopic(string topicName, string loggedUser);
        Task<bool> UpdateTopic(TopicUpsertion topicUpsertion,string currentLoggedUser);
    }

    public class TopicRepository : ITopicRepository
    {
        private readonly UserDbContext _userDbContext;
        
        public TopicRepository(UserManager<IdentityUser> userManager,
                                UserDbContext userDbContext)
        {
            _userDbContext = userDbContext;
        }

        public async Task<bool> DeleteTopic(string topicName, string loggedUser)
        {
            try
            {
                var selectedTopic = _userDbContext.Topics.Where(x => x.Name == topicName).SingleOrDefault();

                if (selectedTopic == null) return false;

                _userDbContext.Topics.Remove(selectedTopic);
                await _userDbContext.SaveChangesAsync();
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> UpdateTopic(TopicUpsertion topicUpsertion, string currentLoggedUser)
        {
            try
            {
                var selectedTopic = _userDbContext.Topics.Where(x => x.Name == topicUpsertion.Name).Include(y=>y.Cards).SingleOrDefault();

                if (selectedTopic == null) return false;

                selectedTopic.Name = topicUpsertion.Name;
                selectedTopic.Tag = topicUpsertion.Tag;

                
                if(selectedTopic.Cards == null)
                {
                    selectedTopic.Cards = new List<DbCard>();
                }

                foreach(Card card in topicUpsertion.Cards)
                {
                    var currentCard = selectedTopic.Cards.Where(x => x.Index == card.Index).SingleOrDefault();
                    if (currentCard == null)
                    {
                        currentCard = new DbCard();
                    }
                    currentCard.Content = card.Content;
                    currentCard.Index = card.Index;
                }

               selectedTopic.Cards.RemoveAll(x => !topicUpsertion.Cards.Any(z => z.Index == x.Index));
                
                await _userDbContext.SaveChangesAsync();
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }
        }


        public async Task<bool> AddTopic(TopicUpsertion topicUpsertion,string loggedUser)
        {
            try
            {

                var currentUser = _userDbContext.NotesUsers.Where(z => z.Email == loggedUser)
                        .Include(x => x.Topics).SingleOrDefault();

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
                            return Task.FromResult(new List<Topic>());
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
                    return Task.FromResult(new List<Topic>());
            }
            catch(Exception ex)
            {
                throw new LoginException("Failed getting topics");
            }
        }
    }
}
