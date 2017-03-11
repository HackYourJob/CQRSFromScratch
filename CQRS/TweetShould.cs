using System;
using System.Collections.Generic;
using System.Linq;
using NFluent;
using Xunit;

namespace CQRS
{
    public class TweetShould
    {
        [Fact]
        public void RaiseTheEventTweetPublishedWhenTweet()
        {
            var tweet = new Tweet();
            var tweetPublished = tweet.Publish("s");

            Check.That(tweetPublished).Equals(new TweetPublished("s"));
        }

        [Fact]
        public void RaiseTheEventweetDeletedWhenDelete()
        {
            var tweet = new Tweet(new List<IEvent> { new TweetPublished("s")});
            var tweetDeleted = tweet.Delete();

            Check.That(tweetDeleted).Equals(new TweetDeleted());
        }

        [Fact]
        public void NotDeleteIfNotPublished()
        {
            var tweet = new Tweet();

            var tweetDeleted = tweet.Delete();

            Check.That(tweetDeleted).Equals(new DeleteNotPossible());
        }

        [Fact]
        public void NotDeleteWhenTweetAlreadyDeleted()
        {
            var tweet = new Tweet(new List<IEvent>
            {
                new TweetPublished("s"),
                new TweetDeleted()
            });

            var tweetDeleted = tweet.Delete();

            Check.That(tweetDeleted).Equals(new DeleteNotPossible());
        }


        public class Tweet
        {
            private bool _isPublished = false;
            private bool _isNotDeleted = true;

            public Tweet(IEnumerable<IEvent> history)
            {
                if (history.OfType<TweetPublished>().Any())
                {
                    WhenTweetPublished();
                }
                if (history.OfType<TweetDeleted>().Any())
                {
                    WhenTweetDeleted();
                }
            }

            private void WhenTweetDeleted()
            {
                _isNotDeleted = false;
            }

            private void WhenTweetPublished()
            {
                _isPublished = true;
            }

            public Tweet()
            {
                // TODO : to remove
            }

            public IEvent Publish(string s)
            {
                return RaiseEvent(new TweetPublished(s), WhenTweetPublished);
            }

            private IEvent RaiseEvent(IEvent @event, Action whenEvent)
            {
                whenEvent();
                return @event;
            }

            public IEvent Delete()
            {
                if (_isPublished && _isNotDeleted)
                {
                    WhenTweetDeleted();
                    return new TweetDeleted();
                }
                return new DeleteNotPossible();
            }
        }

    }

    public interface IEvent
    {
    }

    public struct TweetPublished : IEvent
    {
        private readonly string _s;

        public TweetPublished(string s)
        {
            _s = s;
        }
    }

    public struct DeleteNotPossible : IEvent
    {
    }

    public struct TweetDeleted : IEvent
    {
    }
}
