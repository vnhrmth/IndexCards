using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CardsAPI.Models;
using CardsAPI.Repository;

namespace CardsAPI.Services
{
    public interface ITopicServices
    {
        Task<bool> DeleteTopic(string topicName, string currentLoggedUser);
        Task<bool> AddTopic(TopicUpsertion addDeviceUpsertion, string currentLoggedUser);
        Task<IList<Topic>> GetTopics(string emailId);
        Task<bool> UpdateTopic(TopicUpsertion topicUpsertion, string currentLoggedUser);
    }

    public class TopicService : ITopicServices
    {
        private readonly ITopicRepository _topicRepository;
        public TopicService(ITopicRepository topicRepository)
        {
            _topicRepository = topicRepository;
        }

        public async Task<bool> DeleteTopic(string topicName, string currentLoggedUser)
        {
            return await _topicRepository.DeleteTopic(topicName, currentLoggedUser);
        }

        public async Task<bool> AddTopic(TopicUpsertion topicUpsertion, string currentLoggedUser)
        {
            return await _topicRepository.AddTopic(topicUpsertion,currentLoggedUser);
        }

        public async Task<IList<Topic>> GetTopics(string emailId)
        {
            return await _topicRepository.GetTopics(emailId);
        }

        public async Task<bool> UpdateTopic(TopicUpsertion topicUpsertion, string currentLoggedUser)
        {
            return await _topicRepository.UpdateTopic(topicUpsertion, currentLoggedUser);
        }

    }
}
