using System;
using Harald.WebApi.Infrastructure.Facades.Slack;
using Xunit;

namespace Harald.Tests.Infrastructure.Facades.Slack
{
    public class SlackHelperTests
    {
        [Fact]
        public void FixChannelNameForSlack_PascalCasing_ReturnsValidSlackChannelName()
        {
            // Arrange
            var sut = new SlackHelper();
            var channelName = "PascalCasing";

            // Act
            var fixedChannelName = sut.FixChannelNameForSlack(channelName);

            // Assert
            AssertValidSlackChannelName(fixedChannelName);
        }

        [Fact]
        public void FixChannelNameForSlack_CamelCasing_ReturnsValidSlackChannelName()
        {
            // Arrange
            var sut = new SlackHelper();
            var channelName = "camelCasing";

            // Act
            var fixedChannelName = sut.FixChannelNameForSlack(channelName);

            // Assert
            AssertValidSlackChannelName(fixedChannelName);
        }

        [Fact]
        public void FixChannelNameForSlack_LowerCasing_ReturnsValidSlackChannelName()
        {
            // Arrange
            var sut = new SlackHelper();
            var channelName = "lowercase";

            // Act
            var fixedChannelName = sut.FixChannelNameForSlack(channelName);

            // Assert
            AssertValidSlackChannelName(fixedChannelName);
        }

        [Fact]
        public void FixChannelNameForSlack_DashSeparated_ReturnsValidSlackChannelName()
        {
            // Arrange
            var sut = new SlackHelper();
            var channelName = "dash-separated";

            // Act
            var fixedChannelName = sut.FixChannelNameForSlack(channelName);

            // Assert
            AssertValidSlackChannelName(fixedChannelName);
        }

        [Fact]
        public void FixChannelNameForSlack_CharAndNumbers_ReturnsValidSlackChannelName()
        {
            // Arrange
            var sut = new SlackHelper();
            var channelName = "Numbers123";

            // Act
            var fixedChannelName = sut.FixChannelNameForSlack(channelName);

            // Assert
            AssertValidSlackChannelName(fixedChannelName);
        }

        [Fact]
        public void FixChannelNameForSlack_CharsAndNumbersDashSeparated_ReturnsValidSlackChannelName()
        {
            // Arrange
            var sut = new SlackHelper();
            var channelName = "Num123-MoreNumbers456";

            // Act
            var fixedChannelName = sut.FixChannelNameForSlack(channelName);

            // Assert
            AssertValidSlackChannelName(fixedChannelName);
        }

        [Fact]
        public void FixChannelNameForSlack_NumbersAndCharsDashSeparated_ReturnsValidSlackChannelName()
        {
            // Arrange
            var sut = new SlackHelper();
            var channelName = "123Numbers-456MoreNumbers";

            // Act
            var fixedChannelName = sut.FixChannelNameForSlack(channelName);

            // Assert
            AssertValidSlackChannelName(fixedChannelName);
        }

        [Fact]
        public void FixChannelNameForSlack_LowerCaseAndNumbers_ReturnsValidSlackChannelName()
        {
            // Arrange
            var sut = new SlackHelper();
            var channelName = "numbers123";

            // Act
            var fixedChannelName = sut.FixChannelNameForSlack(channelName);

            // Assert
            AssertValidSlackChannelName(fixedChannelName);
        }

        [Fact]
        public void FixChannelNameForSlack_LowerCaseAndNumbersDashSeparated_ReturnsValidSlackChannelName()
        {
            // Arrange
            var sut = new SlackHelper();
            var channelName = "numbers123-moreNumbers456";

            // Act
            var fixedChannelName = sut.FixChannelNameForSlack(channelName);

            // Assert
            AssertValidSlackChannelName(fixedChannelName);
        }

        [Fact]
        public void FixChannelNameForSlack_NumbersAndLowerCaseDashSeparated_ReturnsValidSlackChannelName()
        {
            // Arrange
            var sut = new SlackHelper();
            var channelName = "123numbers-456moreNumbers";

            // Act
            var fixedChannelName = sut.FixChannelNameForSlack(channelName);

            // Assert
            AssertValidSlackChannelName(fixedChannelName);
        }

        [Fact]
        public void FixChannelNameForSlack_Numbers_ReturnsValidSlackChannelName()
        {
            // Arrange
            var sut = new SlackHelper();
            var channelName = "123";

            // Act
            var fixedChannelName = sut.FixChannelNameForSlack(channelName);

            // Assert
            AssertValidSlackChannelName(fixedChannelName);
        }

        [Fact]
        public void FixChannelNameForSlack_NumbersAndLowerCaseUnderscoreSeparated_ReturnsValidSlackChannelName()
        {
            // Arrange
            var sut = new SlackHelper();
            var channelName = "123numbers_456moreNumbers";

            // Act
            var fixedChannelName = sut.FixChannelNameForSlack(channelName);

            // Assert
            AssertValidSlackChannelName(fixedChannelName);
        }

        private void AssertValidSlackChannelName(string name)
        {
            Assert.True(name.Length <= 21);
            AssertValidSlackName(name);
        }

        private void AssertValidSlackName(string name)
        {
            var lowerCaseName = name.ToLower();

            Assert.Equal(lowerCaseName, name);
            Assert.DoesNotContain("_", name);
        }
    }
}
