using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Net.Http.Json;

namespace PlayAdvBlazor.Clients.AdventureServer
{

public class GameMoveResult  // You need to tell Json Deserializr not to be case sensative 
    {
        public string InstanceID { get; set;  }
        public string RoomName { get; set; }
        public string RoomMessage { get; set; }
        public string ItemsMessage { get; set; }
        public string HealthReport { get; set; }
        public string PlayerName { get; set; }

    }

    public class Game  // You need to tell Json Deserializr not to be case sensative 
    {
        public int Id { get; set;  }
        public string Name { get; set; }
        public string Desc { get; set; }
        public string Ver { get; set; }

    }

    public interface IAdventureServerClient
    {
        public Task<List<Game>> GetGamesAsync();
        public Task<GameMoveResult> StartAdventureAsync(int GameID);
        public Task<GameMoveResult> PlayGameAsync(string InstanceID, string Move);
        public void SaveLastMove(GameMoveResult gameMoveResult);
        public GameMoveResult RestoreLastMove();
    }

    public class AdventureServerClient : IAdventureServerClient
    {
        private readonly HttpClient _client;

        public string ApiHost { get; set; } = "adventureserver.whitepebble-8c9b7158.centralus.azurecontainerapps.io";
        public const string _gameListPath = "api/Adventure/list";
        public const string _newGamePath = "api/Adventure/"; // api/Adventure/{id}
        public const string _playGamePath = "api/Adventure";  //  Example "http://advsrv.azurewebsites.net/api/Adventure?InstanceID=1111111&Move=Go%20North"

        public AdventureServerClient(HttpClient Client)
        {
            _client = Client;

        }

        public Uri GetUri(string Scheme, string Host, string Path, string Query)
        {
            var ub = new UriBuilder
            {
                Host = Host,
                Scheme = Scheme,
                Path = Path
            };

            if ((Query != null) & (Query != ""))
            {
                ub.Query = Query;
            }

            return ub.Uri;
        }

        public async Task<List<Game>> GetGamesAsync() { return await _client.GetFromJsonAsync<List<Game>>(GetUri("https", ApiHost, _gameListPath, ""));}

        public async Task<GameMoveResult> StartAdventureAsync(int GameID) { return await _client.GetFromJsonAsync<GameMoveResult>(GetUri("https", ApiHost, _newGamePath, "").ToString() + GameID); }

        public async Task<GameMoveResult> PlayGameAsync(string InstanceID, string Move)
        {
            var gmr = new GameMoveResult();
            var JOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            };

            var url = GetUri("https", ApiHost, _playGamePath, "InstanceID=" + InstanceID + "&Move=" + Move).ToString();
            var _result = await _client.PostAsJsonAsync<GameMoveResult>(url,gmr,JOptions);
            var _moveResult = await _result.Content.ReadFromJsonAsync<GameMoveResult>(JOptions);
            return _moveResult;
  
        }

        // Save the last move if the use navigates away from the Play Adventure Page
        private GameMoveResult LastMove { get; set; } = new GameMoveResult();
        public void SaveLastMove(GameMoveResult gameMoveResult) { LastMove = gameMoveResult; }
        public GameMoveResult RestoreLastMove() { return LastMove; }
        

    }
    
    
}
