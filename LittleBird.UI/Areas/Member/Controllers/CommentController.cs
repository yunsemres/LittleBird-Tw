﻿using LittleBird.Model.Option;
using LittleBird.Service.Option;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LittleBird.UI.Areas.Member.Controllers
{
    public class CommentController : Controller
    {
        CommentService _commentService;
        AppUserService _appUserService;
        LikeService _likeService;
        TweetService _articleService;

        public CommentController()
        {
            _commentService = new CommentService();
            _appUserService = new AppUserService();
            _likeService = new LikeService();
            _articleService = new TweetService();
        }

        public JsonResult AddComment(string userComment, Guid id)
        {

            Comment comment = new Comment();

            comment.AppUserID = _appUserService.FindByUserName(HttpContext.User.Identity.Name).ID;
            comment.TweetID = id;
            comment.Content = userComment;

            bool isAdded = false;
            try
            {
                _commentService.Add(comment);
                isAdded = true;
            }
            catch (Exception ex)
            {
                isAdded = false;
            }

            return Json(isAdded, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAtricleComment(string id)
        {
            Guid articleID = new Guid(id);

            Comment comment = _commentService.GetDefault(x => x.TweetID == articleID && x.Status == Core.Enum.Status.Active).LastOrDefault();

            return Json(new
            {
                AppUserImagePath = comment.AppUser.UserImage,
                FirstName = comment.AppUser.FirstName,
                LastName = comment.AppUser.LastName,
                CreatedDate = comment.CreatedDate.ToString(),
                Content = comment.Content,
                CommentCount = _commentService.GetDefault(x => x.TweetID == articleID && (x.Status == Core.Enum.Status.Active || x.Status == Core.Enum.Status.Updated)).Count(),
                LikeCount = _likeService.GetDefault(x => x.TweetID == articleID && (x.Status == Core.Enum.Status.Active || x.Status == Core.Enum.Status.Updated)).Count(),
            }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DeleteComment(Guid id)
        {
            Guid userID = _appUserService.FindByUserName(HttpContext.User.Identity.Name).ID;
            bool isDelete = false;


            if (_commentService.Any(x => x.AppUserID == userID))
            {
                isDelete = true;
                _commentService.Remove(id);
                return Json(isDelete, JsonRequestBehavior.AllowGet);
            }
            else
            {
                isDelete = false;
                return Json(isDelete, JsonRequestBehavior.AllowGet);
            }
        }
    }
}