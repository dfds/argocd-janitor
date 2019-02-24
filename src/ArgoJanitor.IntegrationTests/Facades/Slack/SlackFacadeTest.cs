using System;
using System.Net.Http;
using Xunit;
using System.Net;
using System.Threading.Tasks;
using Harald.WebApi.Infrastructure.Facades.Slack;
using Harald.WebApi.Infrastructure.Serialization;

namespace Harald.IntegrationTests.Facades.Slack
{
    public class SlackFacadeTest
    {
        [Fact]
        public async Task CreateChannel_Given_valid_input_Should_create_channel()
        {
            // Arrange
            var httpClient = GetHttpClient();
            var sut = new SlackFacade(httpClient, new JsonSerializer(), new SlackHelper());
            var channelName = "janie-test";
            
            // Act
            var createChannelResponse = await sut.CreateChannel(channelName);
            
            // Assert
             Assert.True(createChannelResponse.Ok);
            Assert.Equal(channelName, createChannelResponse.Channel.Name);
            Assert.NotEmpty(createChannelResponse.Channel.Id);
        }

        [Fact]
        public async Task InviteToChannel_Given_valid_input_Should_invite_to_channel()
        {
            // Arrange
            var httpClient = GetHttpClient();
            var sut = new SlackFacade(httpClient, new JsonSerializer(), new SlackHelper());
            var channelId = "CG3H7GARG";
            var userEmail = "janie@dfds.com";
            
            // Act
            await sut.InviteToChannel(userEmail, channelId);
            
            // Assert
        }

        [Fact]
        public async Task CreateUserGroup_And_Add_User_Given_valid_input_Should_create_group_with_user()
        {
            // Arrange
            var httpClient = GetHttpClient();
            var sut = new SlackFacade(httpClient, new JsonSerializer(), new SlackHelper());
            var groupName = "Harald Integration Test Group";
            var handle = "harald-int-test";
            var description = "Group created through integration test.";
            var userEmail = "janie@dfds.com";

            // Act
            var createUserGroupResponse = await sut.CreateUserGroup(name: groupName, handle: handle, description: description);
            await sut.AddUserGroupUser(userGroupId: createUserGroupResponse.UserGroup.Id, email: userEmail);

            // Assert
            Assert.True(createUserGroupResponse.Ok);
            Assert.Equal(groupName, createUserGroupResponse.UserGroup.Name);
        }

        private HttpClient GetHttpClient()
        {
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri("https://slack.com");
            var authToken = Environment.GetEnvironmentVariable("SLACK_API_AUTH_TOKEN");

            httpClient.DefaultRequestHeaders.Add(HttpRequestHeader.Authorization.ToString(), $"Bearer {authToken}");

            return httpClient;
        }
    }
}
