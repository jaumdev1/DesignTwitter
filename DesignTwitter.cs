using System.Linq;

public class Twitter {

    internal class Tweet {
        public int id {get;set;}
        public int date {get;set;}
    }

    private Dictionary<int, LinkedList<Tweet>> UserTweets {get;set;}
    private Dictionary<int, HashSet<int>> UserFollowees {get;set;}
    private int GlobalTweetOrder {get;set;}

    private const int TWEET_COUNT_PER_USER = 10;

    public Twitter() {
        UserTweets = new Dictionary<int, LinkedList<Tweet>>();
        UserFollowees = new Dictionary<int, HashSet<int>>();
        GlobalTweetOrder = 0;
    }
    
    public void PostTweet(int userId, int tweetId) {
        if (!UserTweets.ContainsKey(userId)) {
            var list = new LinkedList<Tweet>();
            var tweet = new Tweet() {
                id = tweetId,
                date = GlobalTweetOrder
            };
            list.AddFirst(tweet);
            UserTweets.Add(userId, list);
        } else {
            UserTweets[userId].AddFirst(new Tweet() {
                id = tweetId,
                date = GlobalTweetOrder
            });
            
        }
        GlobalTweetOrder++;
    }
    
    public IList<int> GetNewsFeed(int userId) {

        var pq = new PriorityQueue<int, int>();

        if (UserFollowees.ContainsKey(userId)) {
            var followees = UserFollowees[userId];
            var tweetPool = new List<Tweet>();

            foreach (var followee in followees) {
                if (UserTweets.ContainsKey(followee)) {
                    var followeeTweets = UserTweets[followee].Take(TWEET_COUNT_PER_USER);
                    tweetPool.AddRange(followeeTweets);     
                }               
            } 

            foreach (var followeeTweet in tweetPool) {
                pq.Enqueue(followeeTweet.id, followeeTweet.date * -1);
            }
        }

        if (UserTweets.ContainsKey(userId)) {
            var userTweets = UserTweets[userId].Take(TWEET_COUNT_PER_USER);
            foreach (var tweet in userTweets) {
                pq.Enqueue(tweet.id, tweet.date * -1);
            }
        }
        var finalList = new List<int>();

        while (pq.Count > 0 && finalList.Count < 10) {
            finalList.Add(pq.Dequeue());
        }

        return finalList;
    }
    
    public void Follow(int followerId, int followeeId) {
        if (!UserFollowees.ContainsKey(followerId)) {
            UserFollowees.Add(followerId, new HashSet<int>() {followeeId});
        } else {
            UserFollowees[followerId].Add(followeeId);
        }
    }
    
    public void Unfollow(int followerId, int followeeId) {
        if (UserFollowees.ContainsKey(followerId)) {
            UserFollowees[followerId].Remove(followeeId);
        }
    }
}
