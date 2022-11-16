using LeaderboardAPI.Models;
using LeaderboardAPI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using System.Linq;

// The Web API controller uses the Player class to run CRUD operations.
// Contains action methods to support GET, POST, PUT, and DELETE HTTP requests.

namespace LeaderboardAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LeaderboardController : ControllerBase
{
    private readonly LeaderboardService _leaderboardService;
    public LeaderboardController(LeaderboardService leaderboardService) =>
        _leaderboardService = leaderboardService;
    #region Select Players
    [HttpGet("Select all players")]
    public async Task<List<Player>> Get() =>
        await _leaderboardService.GetAsync();

    [HttpGet("Select a player by ID {id:length(24)}")]
    public async Task<ActionResult<Player>> Get(string id)
    {
        var player = await _leaderboardService.GetAsync(id);

        if (player is null)
        {
            return NotFound();
        }
        return player;
    }
    #endregion
    #region Rank all Players
    [HttpGet("Rank all players")]
    public async Task<List<Player>> RankAllPlayers()
    {
        var playerList = await _leaderboardService.GetAsync();
        var rankedList = playerList.OrderBy(x => x.rank).ToList();
        return rankedList;
    }

    [HttpPost("Create a new player")]
    public async Task<IActionResult> Post(Player newPlayer)
    {
        await _leaderboardService.CreateAsync(newPlayer);

        return CreatedAtAction(nameof(Get), new { id = newPlayer.Id }, newPlayer);
    }
    #endregion
    #region Update Player
    [HttpPut("Update player details by id {id:length(24)}")]
    public async Task<IActionResult> Update(string id, Player updatedPlayer)
    {
        var player = await _leaderboardService.GetAsync(id);

        if (player is null)
        {
            return NotFound();
        }

        updatedPlayer.Id = player.Id;

        await _leaderboardService.UpdateAsync(id, updatedPlayer);

        return NoContent();
    }
    #endregion
    #region Delete Player
    [HttpDelete("Delete a player {id:length(24)}")]
    public async Task<IActionResult> Delete(string id)
    {
        var player = await _leaderboardService.GetAsync(id);

        if (player is null)
        {
            return NotFound();
        }

        await _leaderboardService.RemoveAsync(id);

        return NoContent();
    }
    #endregion
    #region Rank Characters
    [HttpGet("Rank all characters by playtime")]
    public async Task<List<Character>> RankCharactersByPlaytime()
    {
        var playerList = await _leaderboardService.GetAsync();
        HashSet<Character> totalPlaytime = new();
        foreach (var player in playerList)
        {
            foreach (var character in player.characters)
            {
                totalPlaytime.Add(new Character(character.name));
            }
        }

        foreach (var character in totalPlaytime)
        {
            foreach (var player in playerList)
            {
                foreach (var character2 in player.characters)
                {
                    if (character2.name == character.name)
                    {
                        character.playtime += character2.playtime;
                    }
                }
            }
        }
        return totalPlaytime.ToList();
    }
    #endregion
    #region Insert Character Methods
    [HttpPut("Insert one character")]
    // Insert two characters in one operation
    public async Task<IActionResult> InsertOneCharacter(string characterName)
    {
        var playerList = await _leaderboardService.GetAsync();

        if (playerList is null)
        {
            return NotFound();
        }
        Character newCharacter = new Character(characterName);
        foreach (var p in playerList)
        {
            p.characters.Add(newCharacter);
            await _leaderboardService.UpdateAsync(p.Id, p);
        }
        return NoContent();
    }
    [HttpPut("Insert two characters")]
    // Insert two characters in one operation
    public async Task<IActionResult> InsertTwoCHaracters(string characterOne, string characterTwo)
    {
        var playerList = await _leaderboardService.GetAsync();

        if (playerList is null)
        {
            return NotFound();
        }

        Character newCharacterOne = new Character(characterOne);
        Character newCharacterTwo = new Character(characterTwo);
        Player player = new Player();
        foreach (var p in playerList)
        {
            player = p;
            player.characters.Add(newCharacterOne);
            player.characters.Add(newCharacterTwo);

            await _leaderboardService.UpdateAsync(p.Id, player);
        }
        return NoContent();
    }
    #endregion
    #region Delete Character Methods
    [HttpPut("Delete a character")]
    public async Task<IActionResult> DeleteSingleCharacter(string characterName)
    {
        var playerList = await _leaderboardService.GetAsync();
        Player player = new Player();
        Character character = new Character(characterName);
        if (playerList is null)
        {
            return NotFound();
        }
        foreach (var playerItem in playerList)
        {
            player = playerItem;
            player.characters.Remove(character);
            await _leaderboardService.UpdateAsync(playerItem.Id, player);
        }
        return NoContent();
    }
    [HttpPut("Delete two characters")]
    public async Task<IActionResult> DeleteTwoCharacters(string characterNameOne, string characterNameTwo)
    {
        var playerList = await _leaderboardService.GetAsync();

        if (playerList is null)
        {
            return NotFound();
        }
        Character newCharacterOne = new Character(characterNameOne);
        Character newCharacterTwo = new Character(characterNameTwo);
        Player player = new Player();
        foreach (var playerItem in playerList)
        {
            player = playerItem;
            player.characters.Remove(newCharacterOne);
            player.characters.Remove(newCharacterTwo);
            await _leaderboardService.UpdateAsync(playerItem.Id, player);
        }
        return NoContent();
    }
    #endregion
}
