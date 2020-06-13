using System;
using System.Threading.Tasks;
using CardsAPI.Models;
using CardsAPI.Repository;

namespace CardsAPI.Services
{
    public interface ITopicServices
    {
        Task<bool> AddTopic(TopicUpsertion addDeviceUpsertion);
    }

    public class TopicService : ITopicServices
    {
        private readonly ITopicRepository _topicRepository;
        public TopicService(ITopicRepository topicRepository)
        {
            _topicRepository = topicRepository;
        }

        public async Task<bool> AddTopic(TopicUpsertion topicUpsertion)
        {
            return await _topicRepository.AddTopic(topicUpsertion);
        }
    }
}
