using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ValQ.API.Model.Request;
using ValQ.API.Model.Response;
using ValQ.Core;
using ValQ.Services.Games;
using ValQ.Services.Localization;

namespace ValQ.API.Controllers
{
    public class UserController : BaseController
    {
        private readonly IWorkContext _workContext;
        private readonly IRankService _rankService;
        private readonly ILocalizationService _localizationService;
        private readonly IMatchService _matchService;

        public UserController(IWorkContext workContext,
                              IRankService rankService,
                              ILocalizationService localizationService,
                              IMatchService matchService)
        {
            _workContext = workContext;
            _rankService = rankService;
            _localizationService = localizationService;
            _matchService = matchService;
        }

        [HttpGet]
        [Route("details")]
        public async Task<IActionResult> Details()
        {
            var user = await _workContext.GetCurrentUserAsync();
            var rank = await _rankService.GetRankForElo(user.Elo);
            return Ok(new Profile()
            {
                CorrectAnswerRate = Convert.ToInt32(((double)user.TotalNumberOfCorrectAnswers / (double)(user.TotalNumberOfCorrectAnswers + user.TotalNumberOfIncorrectAnswers)) * 100),
                NumberOfCorrectAnswers = user.TotalNumberOfCorrectAnswers,
                NumberOfIncorrectAnswers = user.TotalNumberOfIncorrectAnswers,
                Elo = user.Elo,
                Name = user.NickName,
                Rank = new Rank()
                {
                    Name = await _localizationService.GetLocalizedAsync(rank, r => r.Name),
                    URL = rank.PictureURL
                }
            });
        }

        [HttpGet]
        [Route("matchHistory")]
        public async Task<IActionResult> MatchHistory([FromQuery] PagedRequest request) 
        {
            var user = await _workContext.GetCurrentUserAsync();
            var matchHistories = await _matchService.GetMatchHistoryForUser(user.Id, request.PageNumber, request.PageSize);

            var matchHistoryLines = new List<MatchHistoryLine>();

            foreach(var match in matchHistories)
            {
                matchHistoryLines.Add(new MatchHistoryLine()
                {
                    NumberOfCorrectAnswers = match.NumberOfCorrectAnswers,
                    NumberOfIncorrectAnswers = match.NumberOfIncorrectAnswers
                });
            }

            return Ok(matchHistoryLines);
        }
    }
}
