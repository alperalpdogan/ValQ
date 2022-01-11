using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ValQ.API.Model.Request;
using ValQ.API.Model.Response;
using ValQ.Services.Games;

namespace ValQ.API.Controllers
{
    public class GameController : BaseController
    {
        private readonly IGameService _gameService;

        public GameController(IGameService gameService)
        {
            _gameService = gameService;
        }

        [HttpGet("start")]
        public async Task<IActionResult> StartGame()
        {
            var game = await _gameService.GenerateGameAsync();

            return Ok(game);
        }

        [HttpPost("finish")]
        public async Task<IActionResult> FinishGame(FinishGameRequest request)
        {
            var result = await _gameService.FinishGameAsync(request.NumberOfCorrectAnswers, request.NumberOfIncorrectAnswers);

            var response = new GameResult()
            {
                EloChange = result.EloChange,
                NumberOfCorrectAnswers = result.NumberOfCorrectAnswers,
                NewElo = result.NewElo,
                NumberOfIncorrectAnswers = result.NumberOfIncorrectAnswers,
                OldElo = result.OldElo,
                NewRank = new Rank()
                {
                    Name = result.NewRank.Name,
                    URL = result.NewRank.PictureURL
                },
                OldRank = new Rank()
                {
                    Name = result.OldRank.Name,
                    URL = result.OldRank.PictureURL
                }
            };

            return Ok(response);
        }
    }
}
