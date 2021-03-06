﻿using System;

using System.Linq;

using Cafe.Repostory;
using Cafe.Tools.Games;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model.Entities;
using Model.UIGame;
using Newtonsoft.Json;

namespace Cafe.Controllers.Game
{
    [Authorize]
    public class GameController : Controller
    {
        PlayGameRepository repository;
        public GameController( )
        {
            repository = new PlayGameRepository();
        }
        public IActionResult Start(int id)
        {            
            ViewBag.GameId = id;
            return View();
        }

        [HttpPost]
        public string StartGame(int id)
        {
            var pg = repository.GetByColumName("GameId",id);
            return JsonConvert.SerializeObject(pg.FirstOrDefault());
        }

        [HttpPost]
        public string PossiblePlace(int x, int y, int z, int gameId)
        {
            var pg = repository.GetByColumName("GameId", gameId).FirstOrDefault();
            UICoordinate uICoordinate = new UICoordinate();
            UIPlayGame uIPlaygame = new SrzJson().desrz(pg);

            if (pg.Gamer1 == pg.Queue)
            {
                uICoordinate = new PossiblePlace(
                    uIPlaygame,
                    Convert.ToByte(x),
                    Convert.ToByte(y),
                    Convert.ToByte(z)).
                    White();
            }

            else if (pg.Gamer2 == pg.Queue)
            {
                uICoordinate = new PossiblePlace(
                    uIPlaygame,
                    Convert.ToByte(x),
                    Convert.ToByte(y),
                    Convert.ToByte(z)).
                    BLack(); 
            }
            return JsonConvert.SerializeObject(uICoordinate);
        }

        [HttpPost]
        public string Move(int gameID, int oldX, int oldY, int oldZ, int newX, int newY)
        {
            var pg = repository.GetByColumName("GameId", gameID).FirstOrDefault();
            var uiGame = new SrzJson().desrz(pg);
            var moveItem = new MoveItem(
                uiGame, 
                Convert.ToByte(oldX),
                Convert.ToByte(oldY),
                Convert.ToByte(oldZ),
                Convert.ToByte(newX),
                Convert.ToByte(newY)
                );

            uiGame.Move.DumX = 0; uiGame.Move.DumY = 0;
            if (uiGame.Gamer1 == uiGame.Queue)
            {
                uiGame = moveItem.MoveWhite();
                uiGame.Queue = uiGame.Gamer2;
            }
            else if (uiGame.Gamer2 == uiGame.Queue)
            {
                uiGame = moveItem.MoveBlack();
                uiGame.Queue = uiGame.Gamer1;
            }
            if (!repository.Update(new SrzJson().srz(uiGame), pg.Id)) return "";

            return JsonConvert.SerializeObject(uiGame.Move);
        }


        [HttpPost]
        public string DumMove(int gameID, int oldX, int oldY, int oldZ, int newX, int newY)
        {
            var pg = repository.GetByColumName("GameId", gameID).FirstOrDefault();
            var uiGame = new SrzJson().desrz(pg);
            var moveItem = new MoveItem(
                uiGame, 
                Convert.ToByte(oldX), 
                Convert.ToByte(oldY), 
                Convert.ToByte(oldZ), 
                Convert.ToByte(newX), 
                Convert.ToByte(newY));

            

            if (uiGame.Gamer1 == uiGame.Queue) uiGame = moveItem.DumWhite();
            else if (uiGame.Gamer2 == uiGame.Queue) uiGame = moveItem.DumBlack();

            if (!repository.Update(new SrzJson().srz(uiGame), pg.Id)) return "";

            return JsonConvert.SerializeObject(uiGame.Move);
        }

        [HttpPost]
        public string CkeckGame(int gameId)
        {
            var pg = repository.GetByColumName("GameId", gameId).FirstOrDefault();
            //any stone win
            if (!pg.BlackCoordinate.Contains("X")|| !pg.WhiteCoordinate.Contains("X")){
                var gamesRepostory = new GamesRepository();
                var g = gamesRepostory.GetByColumName("Id", gameId).FirstOrDefault();
                g.Status = GameStatus.Close;
                if(!pg.WhiteCoordinate.Contains("X"))g.WinUser = g.AcceptUser;
                if(!pg.BlackCoordinate.Contains("X"))g.WinUser = g.RequestUser;
                var b= gamesRepostory.Update(g,g.Id);
                return "Win User " + g.WinUser;
            }
            return JsonConvert.SerializeObject(pg);
        }

    }
}