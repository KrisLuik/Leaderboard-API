using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace LeaderboardAPI.Models
{
    public class Player
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("gamertag")]
        public string gamertag { get; set; } = null!;

        [BsonElement("rank")]
        public int rank { get; set; }

        [BsonElement("characters")]
        public List<Character> characters { get; set; }
    }

    public class Character
    {
        [BsonElement("name")]
        public string? name { get; set; }
        [BsonElement("playtime")]
        public int playtime { get; set; }

        public Character(string Name, int Playtime)
        {
            name = Name;
            playtime = Playtime;    
        }
        public Character(string Name)
        {
            name = Name;
        }
        public override bool Equals(object? obj)
        {
            Character characterName = obj as Character;
            if (characterName == null)
            {
                return false;   
            }
            return characterName.name == name;
        }
        public override int GetHashCode()
        {
            return name.GetHashCode();
        }

    }
}
