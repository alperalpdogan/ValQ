using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ValQ.API.Framework.Models;
using ValQ.API.Model.Request;
using ValQ.API.Model.Response;
using ValQ.Core;
using ValQ.Core.Domain.Users;
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
        [ProducesResponseType(typeof(PagedResult<MatchHistoryLine>), 200)]
        public async Task<IActionResult> MatchHistory([FromQuery] PagedRequest request) 
        {
            var user = await _workContext.GetCurrentUserAsync();
            var matchHistories = await _matchService.GetMatchHistoryForUser(user.Id, request.PageNumber, request.PageSize);

            var matchHistoryLines = new PagedResult<MatchHistoryLine>()
            {
                CurrentPage = matchHistories.PageIndex,
                TotalCount = matchHistories.TotalCount,
                TotalPages = matchHistories.TotalPages
            };

            foreach (var match in matchHistories)
            {
                matchHistoryLines.Data.Add(new MatchHistoryLine()
                {
                    NumberOfCorrectAnswers = match.NumberOfCorrectAnswers,
                    NumberOfIncorrectAnswers = match.NumberOfIncorrectAnswers,
                    PlayedAt = match.PlayedAt,
                    EloChange = match.EloChange
                });
            }

            return Ok(matchHistoryLines);
        }


        [HttpGet]
        [Route("rankHistory")]
        [ProducesResponseType(typeof(PagedResult<RankHistory>), 200)]
        public async Task<IActionResult> RankHistory([FromQuery] PagedRequest request)
        {
            var histories = await _rankService.GetRankHistoryAsync(request.PageNumber, request.PageSize);

            var model = new PagedResult<RankHistory>()
            {
                CurrentPage = histories.PageIndex,
                TotalCount = histories.TotalCount,
                TotalPages = histories.TotalPages
            };

            foreach(var history in histories)
            {
                var oldRank = await _rankService.GetRankByIdAsync(history.OldRankId);
                var newRank = await _rankService.GetRankByIdAsync(history.NewRankId);
                model.Data.Add(new RankHistory()
                {
                    ChangedAt = history.ChangedAt,
                    NewRank = new Rank()
                    {
                        Name = newRank.Name,
                        URL = newRank.PictureURL
                    },
                    OldRank = new Rank()
                    {
                        Name = oldRank.Name,
                        URL = oldRank.PictureURL
                    }
                });
            }

            return Ok(model);
        }
    }
}
